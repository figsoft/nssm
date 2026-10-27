using Microsoft.Win32;

namespace NssmSharp;

/// <summary>
/// 服务参数配置（与 NSSM 使用相同注册表键名）。
/// 参数键：HKLM\SYSTEM\CurrentControlSet\Services\&lt;服务名&gt;\Parameters
/// </summary>
public sealed class ServiceConfig
{
    // 应用程序
    public string? Application { get; set; }
    public string? AppDirectory { get; set; }
    public string? AppParameters { get; set; }

    // 退出动作：Restart | Ignore | Exit | Unclean
    public string AppExit { get; set; } = "Restart";
    public int AppRestartDelay { get; set; } = 0;
    public int AppThrottle { get; set; } = 1500;

    // I/O 重定向
    public string? AppStdout { get; set; }
    public string? AppStderr { get; set; }
    public string? AppStdin { get; set; }
    public int AppStdoutCopyAndTruncate { get; set; } = 0;
    public int AppStderrCopyAndTruncate { get; set; } = 0;
    public int AppTimestampLog { get; set; } = 0;

    // 日志轮转
    public int AppRotateFiles { get; set; } = 0;
    public int AppRotateOnline { get; set; } = 0;
    public int AppRotateSeconds { get; set; } = 0;
    public long AppRotateBytes { get; set; } = 0;
    public int AppRotateDelay { get; set; } = 0;

    // 进程
    public int AppPriority { get; set; } = 0x20;      // NORMAL_PRIORITY_CLASS
    public long AppAffinity { get; set; } = 0;         // 0 = 所有 CPU
    public int AppNoConsole { get; set; } = 0;         // 1 = 不创建控制台

    // 环境变量
    public string? AppEnvironment { get; set; }        // 完整替换环境
    public string? AppEnvironmentExtra { get; set; }   // 追加环境

    // 停止方式（位掩码，1<<0=Control-C, 1<<1=WM_CLOSE, 1<<2=WM_QUIT, 1<<3=Terminate）
    public int AppStopMethodSkip { get; set; } = 0;
    public int AppStopMethodConsole { get; set; } = 1500;
    public int AppStopMethodWindow { get; set; } = 1500;
    public int AppStopMethodThreads { get; set; } = 1500;
    public int AppKillProcessTree { get; set; } = 1;

    // 钩子
    public int AppRedirectHook { get; set; } = 0;
    public Dictionary<string, string> Hooks { get; } = new();  // "Event/Action" -> 命令

    // 服务级
    public string DisplayName { get; set; } = "";
    public string Description { get; set; } = "";
    public int StartType { get; set; } = 2;            // 2=自动 3=手动 4=禁用 5=延迟自动
    public List<string> Dependencies { get; set; } = new();
    public string? ObjectName { get; set; }            // 登录账户（null = LocalSystem）
    public string? Password { get; set; }
    public bool VirtualAccount { get; set; }           // 虚拟服务账户
    public bool Interactive { get; set; }              // 允许与桌面交互

    private static string ParamsKey(string serviceName)
        => $@"SYSTEM\CurrentControlSet\Services\{serviceName}\Parameters";

    private static string SvcKey(string serviceName)
        => $@"SYSTEM\CurrentControlSet\Services\{serviceName}";

    public static ServiceConfig Load(string serviceName)
    {
        var cfg = new ServiceConfig();
        using (var key = Registry.LocalMachine.OpenSubKey(ParamsKey(serviceName)))
        {
            if (key != null)
            {
                cfg.Application = key.GetValue("Application") as string;
                cfg.AppDirectory = key.GetValue("AppDirectory") as string;
                cfg.AppParameters = key.GetValue("AppParameters") as string;
                cfg.AppExit = key.GetValue("AppExit") as string ?? "Restart";
                cfg.AppRestartDelay = GetInt(key, "AppRestartDelay");
                cfg.AppThrottle = GetInt(key, "AppThrottle", 1500);
                cfg.AppStdout = key.GetValue("AppStdout") as string;
                cfg.AppStderr = key.GetValue("AppStderr") as string;
                cfg.AppStdin = key.GetValue("AppStdin") as string;
                cfg.AppStdoutCopyAndTruncate = GetInt(key, "AppStdoutCopyAndTruncate");
                cfg.AppStderrCopyAndTruncate = GetInt(key, "AppStderrCopyAndTruncate");
                cfg.AppTimestampLog = GetInt(key, "AppTimestampLog");
                cfg.AppRotateFiles = GetInt(key, "AppRotateFiles");
                cfg.AppRotateOnline = GetInt(key, "AppRotateOnline");
                cfg.AppRotateSeconds = GetInt(key, "AppRotateSeconds");
                cfg.AppRotateBytes = GetLong(key, "AppRotateBytes");
                cfg.AppRotateDelay = GetInt(key, "AppRotateDelay");
                cfg.AppPriority = GetInt(key, "AppPriority", 0x20);
                cfg.AppAffinity = GetLong(key, "AppAffinity");
                cfg.AppNoConsole = GetInt(key, "AppNoConsole");
                cfg.AppEnvironment = key.GetValue("AppEnvironment") as string;
                cfg.AppEnvironmentExtra = key.GetValue("AppEnvironmentExtra") as string;
                cfg.AppStopMethodSkip = GetInt(key, "AppStopMethodSkip");
                cfg.AppStopMethodConsole = GetInt(key, "AppStopMethodConsole", 1500);
                cfg.AppStopMethodWindow = GetInt(key, "AppStopMethodWindow", 1500);
                cfg.AppStopMethodThreads = GetInt(key, "AppStopMethodThreads", 1500);
                cfg.AppKillProcessTree = GetInt(key, "AppKillProcessTree", 1);
                cfg.AppRedirectHook = GetInt(key, "AppRedirectHook");
                LoadHooks(key, cfg.Hooks);
            }
        }

        using (var svc = Registry.LocalMachine.OpenSubKey(SvcKey(serviceName)))
        {
            if (svc != null)
            {
                cfg.DisplayName = svc.GetValue("DisplayName") as string ?? "";
                cfg.Description = svc.GetValue("Description") as string ?? "";
                cfg.StartType = GetInt(svc, "Start", 2);
                cfg.ObjectName = svc.GetValue("ObjectName") as string;
                var deps = svc.GetValue("DependOnService") as string[];
                if (deps != null) cfg.Dependencies.AddRange(deps);
                int type = GetInt(svc, "Type", 0x10);
                cfg.Interactive = (type & 0x100) != 0;
                cfg.VirtualAccount = (cfg.ObjectName ?? "").StartsWith("NT SERVICE\\", StringComparison.OrdinalIgnoreCase);
            }
        }
        return cfg;
    }

    public void Save(string serviceName)
    {
        using (var key = Registry.LocalMachine.CreateSubKey(ParamsKey(serviceName)))
        {
            key.SetValue("Application", Application ?? "");
            key.SetValue("AppDirectory", AppDirectory ?? "");
            key.SetValue("AppParameters", AppParameters ?? "");
            key.SetValue("AppExit", AppExit);
            key.SetValue("AppRestartDelay", AppRestartDelay);
            key.SetValue("AppThrottle", AppThrottle);
            key.SetValue("AppStdout", AppStdout ?? "");
            key.SetValue("AppStderr", AppStderr ?? "");
            key.SetValue("AppStdin", AppStdin ?? "");
            key.SetValue("AppStdoutCopyAndTruncate", AppStdoutCopyAndTruncate);
            key.SetValue("AppStderrCopyAndTruncate", AppStderrCopyAndTruncate);
            key.SetValue("AppTimestampLog", AppTimestampLog);
            key.SetValue("AppRotateFiles", AppRotateFiles);
            key.SetValue("AppRotateOnline", AppRotateOnline);
            key.SetValue("AppRotateSeconds", AppRotateSeconds);
            key.SetValue("AppRotateBytes", AppRotateBytes);
            key.SetValue("AppRotateDelay", AppRotateDelay);
            key.SetValue("AppPriority", AppPriority);
            key.SetValue("AppAffinity", AppAffinity);
            key.SetValue("AppNoConsole", AppNoConsole);
            key.SetValue("AppEnvironment", AppEnvironment ?? "");
            key.SetValue("AppEnvironmentExtra", AppEnvironmentExtra ?? "");
            key.SetValue("AppStopMethodSkip", AppStopMethodSkip);
            key.SetValue("AppStopMethodConsole", AppStopMethodConsole);
            key.SetValue("AppStopMethodWindow", AppStopMethodWindow);
            key.SetValue("AppStopMethodThreads", AppStopMethodThreads);
            key.SetValue("AppKillProcessTree", AppKillProcessTree);
            key.SetValue("AppRedirectHook", AppRedirectHook);
            SaveHooks(key, Hooks);
        }
    }

    public static void DeleteParameters(string serviceName)
    {
        try { Registry.LocalMachine.DeleteSubKeyTree(ParamsKey(serviceName), false); }
        catch { /* 键可能不存在 */ }
    }

    private static int GetInt(RegistryKey key, string name, int def = 0)
        => Convert.ToInt32(key.GetValue(name) ?? def);

    private static long GetLong(RegistryKey key, string name, long def = 0)
        => Convert.ToInt64(key.GetValue(name) ?? def);

    private static void LoadHooks(RegistryKey key, Dictionary<string, string> hooks)
    {
        using var events = key.OpenSubKey("AppEvents");
        if (events == null) return;
        foreach (var evt in events.GetSubKeyNames())
        {
            using var ek = events.OpenSubKey(evt);
            if (ek == null) continue;
            foreach (var action in ek.GetValueNames())
            {
                if (ek.GetValue(action) is string cmd) hooks[$"{evt}/{action}"] = cmd;
            }
        }
    }

    private static void SaveHooks(RegistryKey key, Dictionary<string, string> hooks)
    {
        try { key.DeleteSubKeyTree("AppEvents", false); } catch { }
        using var events = key.CreateSubKey("AppEvents");
        foreach (var (name, cmd) in hooks)
        {
            int slash = name.IndexOf('/');
            if (slash <= 0) continue;
            string evt = name[..slash];
            string action = name[(slash + 1)..];
            using var ek = events.CreateSubKey(evt);
            ek.SetValue(action, cmd);
        }
    }
}
