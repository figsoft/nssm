using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;

namespace NssmSharp;

/// <summary>
/// 服务宿主：由 SCM 启动，读取配置、拉起并守护真正的应用程序。
/// 支持崩溃重启、多种停止方式、日志轮转、优先级/亲和、环境变量、事件钩子。
/// </summary>
public sealed class NssmService : ServiceBase
{
    // 停止方式位掩码（与 NSSM 一致）
    private const int STOP_METHOD_CONSOLE = 1 << 0;
    private const int STOP_METHOD_WINDOW = 1 << 1;
    private const int STOP_METHOD_THREADS = 1 << 2;
    private const int STOP_METHOD_TERMINATE = 1 << 3;

    private const uint WM_QUIT = 0x0012;
    private const uint CTRL_C_EVENT = 0;

    private readonly string _serviceName;
    private Process? _process;
    private CancellationTokenSource? _cts;
    private DateTime _startedAt;
    private readonly List<LogPump> _pumps = new();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AttachConsole(uint dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FreeConsole();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool PostThreadMessage(uint idThread, uint Msg, IntPtr wParam, IntPtr lParam);

    public NssmService(string serviceName)
    {
        _serviceName = serviceName;
        ServiceName = serviceName;
        CanStop = true;
        CanShutdown = true;
        CanHandlePowerEvent = true;
    }

    protected override void OnStart(string[] args)
    {
        _cts = new CancellationTokenSource();
        Task.Run(() => MonitorLoop(_cts.Token));
    }

    protected override void OnStop() => StopProcess();
    protected override void OnShutdown() => StopProcess();

    protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
    {
        if (powerStatus == PowerBroadcastStatus.ResumeSuspend)
            RunHook(_serviceName, "Power", "Resume");
        else if (powerStatus is PowerBroadcastStatus.PowerStatusChange or PowerBroadcastStatus.Suspend)
            RunHook(_serviceName, "Power", "Change");
        return base.OnPowerEvent(powerStatus);
    }

    protected override void OnCustomCommand(int command)
    {
        if (command == 128) // NSSM_SERVICE_CONTROL_ROTATE
        {
            lock (_pumps) { foreach (var p in _pumps) p.Rotate(); }
        }
    }

    private void StopProcess()
    {
        _cts?.Cancel();
        var proc = _process;
        if (proc == null || proc.HasExited) return;

        var cfg = ServiceConfig.Load(_serviceName);
        int skip = cfg.AppStopMethodSkip;

        if ((skip & STOP_METHOD_CONSOLE) == 0 && TryConsoleStop(proc, cfg.AppStopMethodConsole)) return;
        if ((skip & STOP_METHOD_WINDOW) == 0 && TryWindowStop(proc, cfg.AppStopMethodWindow)) return;
        if ((skip & STOP_METHOD_THREADS) == 0 && TryThreadsStop(proc, cfg.AppStopMethodThreads)) return;
        if ((skip & STOP_METHOD_TERMINATE) == 0) Kill(proc, cfg.AppKillProcessTree != 0);
    }

    private void MonitorLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var cfg = ServiceConfig.Load(_serviceName);
            if (string.IsNullOrEmpty(cfg.Application))
            {
                if (token.WaitHandle.WaitOne(3000)) break;
                continue;
            }

            RunHook(_serviceName, "Start", "Pre");
            _startedAt = DateTime.Now;
            _process = StartProcess(cfg);
            if (_process != null) RunHook(_serviceName, "Start", "Post");
            else if (token.WaitHandle.WaitOne(3000)) break;

            if (_process != null)
            {
                try { _process.WaitForExit(); } catch { }
                try { _process.Dispose(); } catch { }
                _process = null;
            }

            lock (_pumps) { _pumps.Clear(); }
            if (token.IsCancellationRequested) break;

            RunHook(_serviceName, "Exit", "Post");

            string exitAction = (cfg.AppExit ?? "Restart").ToLowerInvariant();
            if (exitAction == "ignore") break;
            if (exitAction == "exit") { Stop(); break; }
            // "unclean" 与 restart 同样重启（NSSM 里 unclean 用于触发恢复动作）

            int elapsed = (int)(DateTime.Now - _startedAt).TotalMilliseconds;
            int throttleWait = Math.Max(0, cfg.AppThrottle - elapsed);
            if (throttleWait > 0 && token.WaitHandle.WaitOne(throttleWait)) break;
            if (cfg.AppRestartDelay > 0 && token.WaitHandle.WaitOne(cfg.AppRestartDelay)) break;
        }
    }

    private Process? StartProcess(ServiceConfig cfg)
    {
        var psi = new ProcessStartInfo
        {
            FileName = cfg.Application!,
            Arguments = cfg.AppParameters ?? "",
            WorkingDirectory = string.IsNullOrEmpty(cfg.AppDirectory)
                ? Path.GetDirectoryName(cfg.Application) ?? ""
                : cfg.AppDirectory,
            UseShellExecute = false,
            CreateNoWindow = cfg.AppNoConsole != 0,
        };

        ApplyEnvironment(psi, cfg.AppEnvironment, cfg.AppEnvironmentExtra);

        bool redirectOut = !string.IsNullOrEmpty(cfg.AppStdout);
        bool redirectErr = !string.IsNullOrEmpty(cfg.AppStderr);
        if (redirectOut) psi.RedirectStandardOutput = true;
        if (redirectErr) psi.RedirectStandardError = true;

        var proc = new Process { StartInfo = psi };
        if (!proc.Start()) return null;

        try { proc.PriorityClass = PriorityToClass(cfg.AppPriority); } catch { }
        if (cfg.AppAffinity != 0) { try { proc.ProcessorAffinity = (IntPtr)cfg.AppAffinity; } catch { } }

        if (redirectOut) AddPump(proc.StandardOutput, cfg.AppStdout!, cfg, cfg.AppStdoutCopyAndTruncate != 0);
        if (redirectErr) AddPump(proc.StandardError, cfg.AppStderr!, cfg, cfg.AppStderrCopyAndTruncate != 0);

        return proc;
    }

    private void AddPump(StreamReader reader, string path, ServiceConfig cfg, bool truncate)
    {
        var pump = new LogPump(path, truncate,
            cfg.AppRotateFiles != 0, cfg.AppRotateBytes, cfg.AppRotateSeconds, cfg.AppTimestampLog != 0);
        lock (_pumps) _pumps.Add(pump);
        Task.Run(() => pump.Run(reader));
    }

    private void Kill(Process proc, bool tree)
    {
        try { proc.Kill(entireProcessTree: tree); }
        catch { }
    }

    private static bool TryWindowStop(Process proc, int graceMs)
    {
        if (!proc.CloseMainWindow()) return false;
        return proc.WaitForExit(Math.Max(0, graceMs));
    }

    private static bool TryConsoleStop(Process proc, int graceMs)
    {
        // 仅当子进程有自己的控制台时有效
        if (!AttachConsole((uint)proc.Id)) return false;
        GenerateConsoleCtrlEvent(CTRL_C_EVENT, 0);
        FreeConsole();
        return proc.WaitForExit(Math.Max(0, graceMs));
    }

    private static bool TryThreadsStop(Process proc, int graceMs)
    {
        try
        {
            foreach (ProcessThread t in proc.Threads)
                PostThreadMessage((uint)t.Id, WM_QUIT, IntPtr.Zero, IntPtr.Zero);
        }
        catch { return false; }
        return proc.WaitForExit(Math.Max(0, graceMs));
    }

    private static ProcessPriorityClass PriorityToClass(int priority) => priority switch
    {
        0x40 => ProcessPriorityClass.Idle,
        0x4000 => ProcessPriorityClass.BelowNormal,
        0x8000 => ProcessPriorityClass.AboveNormal,
        0x80 => ProcessPriorityClass.High,
        0x100 => ProcessPriorityClass.RealTime,
        _ => ProcessPriorityClass.Normal,
    };

    private static void ApplyEnvironment(ProcessStartInfo psi, string? full, string? extra)
    {
        if (!string.IsNullOrEmpty(full))
        {
            psi.Environment.Clear();
            foreach (var (k, v) in ParseEnv(full)) psi.Environment[k] = v;
        }
        if (!string.IsNullOrEmpty(extra))
        {
            foreach (var (k, v) in ParseEnv(extra)) psi.Environment[k] = v;
        }
    }

    private static IEnumerable<(string, string)> ParseEnv(string text)
    {
        foreach (var line in text.Split('\n'))
        {
            var s = line.Trim();
            int eq = s.IndexOf('=');
            if (eq <= 0) continue;
            yield return (s[..eq].Trim(), s[(eq + 1)..].Trim());
        }
    }

    private static void RunHook(string serviceName, string evt, string action)
    {
        var cfg = ServiceConfig.Load(serviceName);
        if (!cfg.Hooks.TryGetValue($"{evt}/{action}", out var cmd) || string.IsNullOrEmpty(cmd)) return;
        try
        {
            using var p = Process.Start(new ProcessStartInfo("cmd.exe", $"/c {cmd}")
            { UseShellExecute = false, CreateNoWindow = true });
            p?.WaitForExit(60000);
        }
        catch { /* 钩子执行失败不影响主流程 */ }
    }

    /// <summary>把进程输出泵到日志文件，支持按大小/时间在线轮转。</summary>
    private sealed class LogPump
    {
        private readonly string _path;
        private readonly bool _truncate;
        private readonly bool _rotate;
        private readonly long _rotateBytes;
        private readonly int _rotateSeconds;
        private readonly bool _timestamp;
        private FileStream? _fs;
        private StreamWriter? _writer;
        private long _written;
        private DateTime _openedAt;

        public LogPump(string path, bool truncate, bool rotate, long rotateBytes, int rotateSeconds, bool timestamp)
        {
            _path = path;
            _truncate = truncate;
            _rotate = rotate;
            _rotateBytes = rotateBytes;
            _rotateSeconds = rotateSeconds;
            _timestamp = timestamp;
        }

        public void Run(StreamReader reader)
        {
            try
            {
                Open();
                var buf = new char[4096];
                int n;
                while ((n = reader.Read(buf, 0, buf.Length)) > 0)
                {
                    if (ShouldRotate()) Rotate();
                    Write(buf, n);
                }
                Close();
            }
            catch { /* 日志不可写时静默 */ }
        }

        private void Open()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(_path))!);
            _fs = new FileStream(_path, _truncate ? FileMode.Create : FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            _writer = new StreamWriter(_fs, new UTF8Encoding(false)) { AutoFlush = true };
            _written = 0;
            _openedAt = DateTime.Now;
        }

        private void Close()
        {
            try { _writer?.Dispose(); } catch { }
            try { _fs?.Dispose(); } catch { }
            _writer = null; _fs = null;
        }

        private void Write(char[] buf, int count)
        {
            if (_writer == null) return;
            if (_timestamp) _writer.Write($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} ");
            _writer.Write(buf, 0, count);
            _written += count;
        }

        private bool ShouldRotate()
        {
            if (!_rotate) return false;
            if (_rotateBytes > 0 && _written >= _rotateBytes) return true;
            if (_rotateSeconds > 0 && (DateTime.Now - _openedAt).TotalSeconds >= _rotateSeconds) return true;
            return false;
        }

        public void Rotate()
        {
            Close();
            try
            {
                string backup = $"{_path}.{DateTime.Now:yyyyMMddHHmmssfff}";
                if (File.Exists(_path)) File.Move(_path, backup);
            }
            catch { }
            Open();
        }
    }
}
