using System.ServiceProcess;

namespace NssmSharp;

/// <summary>
/// 主窗口：以图形界面完成服务的安装、编辑、卸载与启停，对齐 NSSM 的全部配置项。
/// </summary>
public sealed class MainForm : Form
{
    private readonly TextBox _nameBox = new() { Width = 200 };
    private readonly Button _installBtn = new() { Text = "安装服务" };
    private readonly Button _editBtn = new() { Text = "编辑服务" };
    private readonly Button _removeBtn = new() { Text = "卸载服务" };
    private readonly Button _startBtn = new() { Text = "启动" };
    private readonly Button _stopBtn = new() { Text = "停止" };
    private bool _editing = false;

    // 应用程序
    private readonly TextBox _appBox = new() { Width = 360 };
    private readonly TextBox _dirBox = new() { Width = 360 };
    private readonly TextBox _argsBox = new() { Width = 360 };
    // 详细信息
    private readonly TextBox _displayNameBox = new() { Width = 360 };
    private readonly TextBox _descBox = new() { Width = 360, Height = 56, Multiline = true };
    private readonly ComboBox _startTypeCombo = new() { Width = 160, DropDownStyle = ComboBoxStyle.DropDownList };
    // 登录
    private readonly RadioButton _localSystemRadio = new() { Text = "本地系统账户", Checked = true };
    private readonly CheckBox _interactCheck = new() { Text = "允许服务与桌面交互" };
    private readonly RadioButton _virtualRadio = new() { Text = "虚拟服务账户" };
    private readonly RadioButton _accountRadio = new() { Text = "此账户:" };
    private readonly TextBox _userBox = new() { Width = 200 };
    private readonly TextBox _passBox = new() { Width = 200, UseSystemPasswordChar = true };
    // 进程
    private readonly ComboBox _priorityCombo = new() { Width = 160, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _affinityBox = new() { Width = 160 };
    private readonly CheckBox _noConsoleCheck = new() { Text = "不创建控制台窗口" };
    // 关闭
    private readonly CheckBox _methodConsole = new() { Text = "生成 Control-C", Checked = true };
    private readonly NumericUpDown _methodConsoleMs = new() { Width = 100, Maximum = 86400000, Value = 1500 };
    private readonly CheckBox _methodWindow = new() { Text = "向窗口发送 WM_CLOSE", Checked = true };
    private readonly NumericUpDown _methodWindowMs = new() { Width = 100, Maximum = 86400000, Value = 1500 };
    private readonly CheckBox _methodThreads = new() { Text = "向线程投递 WM_QUIT", Checked = true };
    private readonly NumericUpDown _methodThreadsMs = new() { Width = 100, Maximum = 86400000, Value = 1500 };
    private readonly CheckBox _methodTerminate = new() { Text = "终止进程", Checked = true };
    private readonly CheckBox _killTreeCheck = new() { Text = "结束进程树", Checked = true };
    // 退出动作
    private readonly ComboBox _exitActionCombo = new() { Width = 160, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly NumericUpDown _restartDelayNum = new() { Width = 120, Maximum = 86400000 };
    private readonly NumericUpDown _throttleNum = new() { Width = 120, Maximum = 86400000, Value = 1500 };
    // I/O
    private readonly TextBox _stdoutBox = new() { Width = 360 };
    private readonly TextBox _stderrBox = new() { Width = 360 };
    private readonly TextBox _stdinBox = new() { Width = 360 };
    private readonly CheckBox _truncateOutCheck = new() { Text = "替换现有输出文件" };
    private readonly CheckBox _truncateErrCheck = new() { Text = "替换现有错误文件" };
    private readonly CheckBox _timestampCheck = new() { Text = "时间戳" };
    // 轮转
    private readonly CheckBox _rotateCheck = new() { Text = "轮转文件" };
    private readonly CheckBox _rotateOnlineCheck = new() { Text = "服务运行时轮转" };
    private readonly NumericUpDown _rotateSeconds = new() { Width = 120, Maximum = 86400000 };
    private readonly NumericUpDown _rotateBytes = new() { Width = 120, Maximum = long.MaxValue / 2 };
    private readonly NumericUpDown _rotateDelay = new() { Width = 120, Maximum = 86400000 };
    // 环境
    private readonly TextBox _envBox = new() { Width = 360, Height = 100, Multiline = true };
    private readonly CheckBox _envReplaceCheck = new() { Text = "替换默认环境" };
    // 依赖
    private readonly TextBox _depBox = new() { Width = 360, Height = 100, Multiline = true };
    // 钩子
    private readonly ComboBox _hookEventCombo = new() { Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _hookActionCombo = new() { Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _hookCmdBox = new() { Width = 280 };
    private readonly ListBox _hookList = new() { Width = 400, Height = 100 };
    private readonly Dictionary<string, string> _hooks = new();

    public MainForm()
    {
        Text = "NSSM# 服务管理器";
        Width = 700;
        Height = 620;
        MinimumSize = new Size(640, 540);
        BuildUi();
    }

    private void BuildUi()
    {
        var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(8, 6, 8, 0), WrapContents = false };
        top.Controls.Add(new Label { Text = "服务名称:", AutoSize = true, Margin = new Padding(0, 5, 4, 0) });
        top.Controls.Add(_nameBox);
        foreach (var b in new[] { _installBtn, _editBtn, _removeBtn, _startBtn, _stopBtn }) top.Controls.Add(b);
        Controls.Add(top);

        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(BuildApplicationTab());
        tabs.TabPages.Add(BuildDetailsTab());
        tabs.TabPages.Add(BuildLogonTab());
        tabs.TabPages.Add(BuildProcessTab());
        tabs.TabPages.Add(BuildShutdownTab());
        tabs.TabPages.Add(BuildExitTab());
        tabs.TabPages.Add(BuildIoTab());
        tabs.TabPages.Add(BuildRotationTab());
        tabs.TabPages.Add(BuildEnvTab());
        tabs.TabPages.Add(BuildDepsTab());
        tabs.TabPages.Add(BuildHooksTab());
        Controls.Add(tabs);

        _installBtn.Click += (_, _) => SafeRun(Install);
        _editBtn.Click += (_, _) => SafeRun(Edit);
        _removeBtn.Click += (_, _) => SafeRun(Remove);
        _startBtn.Click += (_, _) => SafeRun(() => ServiceManager.Start(GetName()));
        _stopBtn.Click += (_, _) => SafeRun(() => ServiceManager.Stop(GetName()));

        _startTypeCombo.Items.AddRange(new object[] { "自动", "延迟自动", "手动", "禁用" });
        _startTypeCombo.SelectedIndex = 0;
        _priorityCombo.Items.AddRange(new object[] { "实时", "高", "高于标准", "标准", "低于标准", "低" });
        _priorityCombo.SelectedIndex = 3;
        _exitActionCombo.Items.AddRange(new object[] { "重启应用程序", "不采取动作", "停止服务", "模拟崩溃" });
        _exitActionCombo.SelectedIndex = 0;
        _hookEventCombo.Items.AddRange(new object[] { "Start", "Stop", "Exit", "Power", "Rotate" });
        _hookEventCombo.SelectedIndex = 0;
        _hookActionCombo.Items.AddRange(new object[] { "Pre", "Post" });
        _hookActionCombo.SelectedIndex = 0;

        _localSystemRadio.CheckedChanged += (_, _) => ToggleLogon();
        _virtualRadio.CheckedChanged += (_, _) => ToggleLogon();
        _accountRadio.CheckedChanged += (_, _) => ToggleLogon();
        ToggleLogon();
    }

    private void ToggleLogon()
    {
        _interactCheck.Enabled = _localSystemRadio.Checked;
        _userBox.Enabled = _accountRadio.Checked;
        _passBox.Enabled = _accountRadio.Checked;
    }

    // ---------- 页签构建 ----------

    private TabPage BuildApplicationTab()
    {
        var p = Grid(3);
        AddRow(p, "应用程序:", _appBox, BrowseButton(_appBox, false));
        AddRow(p, "启动目录:", _dirBox, BrowseButton(_dirBox, true));
        AddRow(p, "参数:", _argsBox, null);
        return Wrap("应用程序", p);
    }

    private TabPage BuildDetailsTab()
    {
        var p = Grid(2);
        AddRow(p, "显示名称:", _displayNameBox);
        AddRow(p, "描述:", _descBox);
        AddRow(p, "启动类型:", _startTypeCombo);
        return Wrap("详细信息", p);
    }

    private TabPage BuildLogonTab()
    {
        var p = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), FlowDirection = FlowDirection.TopDown };
        p.Controls.Add(_localSystemRadio);
        p.Controls.Add(_interactCheck);
        p.Controls.Add(_virtualRadio);
        var row = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
        row.Controls.Add(_accountRadio);
        row.Controls.Add(_userBox);
        row.Controls.Add(new Label { Text = "密码:", AutoSize = true, Margin = new Padding(8, 5, 2, 0) });
        row.Controls.Add(_passBox);
        p.Controls.Add(row);
        return Wrap("登录", p);
    }

    private TabPage BuildProcessTab()
    {
        var p = Grid(2);
        AddRow(p, "优先级:", _priorityCombo);
        AddRow(p, "CPU 亲和:", _affinityBox);
        AddRow(p, "", _noConsoleCheck);
        return Wrap("进程", p);
    }

    private TabPage BuildShutdownTab()
    {
        var p = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), FlowDirection = FlowDirection.TopDown };
        p.Controls.Add(Row(_methodConsole, new Label { Text = "超时(ms):", AutoSize = true, Margin = new Padding(8, 5, 2, 0) }, _methodConsoleMs));
        p.Controls.Add(Row(_methodWindow, new Label { Text = "超时(ms):", AutoSize = true, Margin = new Padding(8, 5, 2, 0) }, _methodWindowMs));
        p.Controls.Add(Row(_methodThreads, new Label { Text = "超时(ms):", AutoSize = true, Margin = new Padding(8, 5, 2, 0) }, _methodThreadsMs));
        p.Controls.Add(_methodTerminate);
        p.Controls.Add(_killTreeCheck);
        return Wrap("关闭", p);
    }

    private TabPage BuildExitTab()
    {
        var p = Grid(2);
        AddRow(p, "退出动作:", _exitActionCombo);
        AddRow(p, "重启延迟(ms):", _restartDelayNum);
        AddRow(p, "节流阈值(ms):", _throttleNum);
        return Wrap("退出动作", p);
    }

    private TabPage BuildIoTab()
    {
        var p = Grid(3);
        AddRow(p, "stdout:", _stdoutBox, BrowseButton(_stdoutBox, false));
        AddRow(p, "stderr:", _stderrBox, BrowseButton(_stderrBox, false));
        AddRow(p, "stdin:", _stdinBox, BrowseButton(_stdinBox, false));
        AddRow(p, "", _truncateOutCheck, null);
        AddRow(p, "", _truncateErrCheck, null);
        AddRow(p, "", _timestampCheck, null);
        return Wrap("I/O 重定向", p);
    }

    private TabPage BuildRotationTab()
    {
        var p = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), FlowDirection = FlowDirection.TopDown };
        p.Controls.Add(_rotateCheck);
        p.Controls.Add(_rotateOnlineCheck);
        p.Controls.Add(Row(new Label { Text = "早于(秒):", AutoSize = true, Margin = new Padding(0, 5, 2, 0) }, _rotateSeconds));
        p.Controls.Add(Row(new Label { Text = "大于(字节):", AutoSize = true, Margin = new Padding(0, 5, 2, 0) }, _rotateBytes));
        p.Controls.Add(Row(new Label { Text = "轮转延迟(ms):", AutoSize = true, Margin = new Padding(0, 5, 2, 0) }, _rotateDelay));
        return Wrap("日志轮转", p);
    }

    private TabPage BuildEnvTab()
    {
        var p = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), FlowDirection = FlowDirection.TopDown };
        p.Controls.Add(new Label { Text = "环境变量（每行 KEY=VALUE）:", AutoSize = true });
        p.Controls.Add(_envBox);
        p.Controls.Add(_envReplaceCheck);
        return Wrap("环境变量", p);
    }

    private TabPage BuildDepsTab()
    {
        var p = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), FlowDirection = FlowDirection.TopDown };
        p.Controls.Add(new Label { Text = "依赖的服务（每行一个）:", AutoSize = true });
        p.Controls.Add(_depBox);
        return Wrap("依赖项", p);
    }

    private TabPage BuildHooksTab()
    {
        var p = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), FlowDirection = FlowDirection.TopDown };
        p.Controls.Add(Row(new Label { Text = "事件:", AutoSize = true, Margin = new Padding(0, 5, 4, 0) }, _hookEventCombo,
            new Label { Text = "动作:", AutoSize = true, Margin = new Padding(8, 5, 4, 0) }, _hookActionCombo));
        var addBtn = new Button { Text = "添加/更新钩子", AutoSize = true };
        var delBtn = new Button { Text = "删除选中钩子", AutoSize = true };
        p.Controls.Add(Row(new Label { Text = "命令:", AutoSize = true, Margin = new Padding(0, 5, 4, 0) }, _hookCmdBox, addBtn, delBtn));
        p.Controls.Add(_hookList);

        _hookEventCombo.SelectedIndexChanged += (_, _) => UpdateHookActions();
        UpdateHookActions();

        addBtn.Click += (_, _) =>
        {
            string key = $"{_hookEventCombo.Text}/{_hookActionCombo.Text}";
            string cmd = _hookCmdBox.Text.Trim();
            if (cmd.Length == 0) _hooks.Remove(key);
            else _hooks[key] = cmd;
            RefreshHooks();
        };
        delBtn.Click += (_, _) =>
        {
            if (_hookList.SelectedItem is string s) _hooks.Remove(s);
            RefreshHooks();
        };
        _hookList.SelectedIndexChanged += (_, _) =>
        {
            if (_hookList.SelectedItem is string s && _hooks.TryGetValue(s, out var cmd)) _hookCmdBox.Text = cmd;
        };

        return Wrap("事件钩子", p);
    }

    private void UpdateHookActions()
    {
        string evt = _hookEventCombo.Text;
        _hookActionCombo.Items.Clear();
        if (evt == "Start" || evt == "Rotate") _hookActionCombo.Items.AddRange(new object[] { "Pre", "Post" });
        else if (evt == "Stop") _hookActionCombo.Items.AddRange(new object[] { "Pre" });
        else if (evt == "Exit") _hookActionCombo.Items.AddRange(new object[] { "Post" });
        else if (evt == "Power") _hookActionCombo.Items.AddRange(new object[] { "Change", "Resume" });
        _hookActionCombo.SelectedIndex = 0;
    }

    private void RefreshHooks()
    {
        _hookList.Items.Clear();
        foreach (var key in _hooks.Keys) _hookList.Items.Add(key);
    }

    // ---------- 布局辅助 ----------

    private static TableLayoutPanel Grid(int cols)
    {
        var p = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = cols, Padding = new Padding(8) };
        p.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
        p.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        if (cols >= 3) p.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        return p;
    }

    private static void AddRow(TableLayoutPanel p, string label, Control ctrl, Control? extra = null)
    {
        p.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        p.Controls.Add(new Label { Text = label, AutoSize = true, Margin = new Padding(0, 8, 0, 0) });
        p.Controls.Add(ctrl);
        if (extra != null) p.Controls.Add(extra);
    }

    private static FlowLayoutPanel Row(params Control[] controls)
    {
        var p = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
        p.Controls.AddRange(controls);
        return p;
    }

    private static TabPage Wrap(string title, Control content)
    {
        var tp = new TabPage(title);
        tp.Controls.Add(content);
        return tp;
    }

    private static Button BrowseButton(TextBox target, bool folder)
    {
        var b = new Button { Text = "浏览...", AutoSize = true };
        b.Click += (_, _) =>
        {
            if (folder)
            {
                using var dlg = new FolderBrowserDialog { SelectedPath = target.Text };
                if (dlg.ShowDialog() == DialogResult.OK) target.Text = dlg.SelectedPath;
            }
            else
            {
                using var dlg = new OpenFileDialog { FileName = target.Text, Filter = "可执行文件 (*.exe;*.bat;*.cmd)|*.exe;*.bat;*.cmd|所有文件 (*.*)|*.*" };
                if (dlg.ShowDialog() == DialogResult.OK) target.Text = dlg.FileName;
            }
        };
        return b;
    }

    // ---------- 配置读写 ----------

    private string GetName()
    {
        var name = _nameBox.Text.Trim();
        if (name.Length == 0) throw new InvalidOperationException("请先填写服务名称。");
        return name;
    }

    private static int PriorityToConst(int idx) => idx switch
    {
        0 => 0x100, 1 => 0x80, 2 => 0x8000, 4 => 0x4000, 5 => 0x40, _ => 0x20,
    };

    private static int PriorityToIdx(int c) => c switch
    {
        0x100 => 0, 0x80 => 1, 0x8000 => 2, 0x4000 => 4, 0x40 => 5, _ => 3,
    };

    private static long ParseAffinity(string text)
    {
        long mask = 0;
        foreach (var part in text.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            if (int.TryParse(part.Trim(), out int cpu) && cpu >= 0 && cpu < 64) mask |= 1L << cpu;
        }
        return mask;
    }

    private static string FormatAffinity(long mask)
    {
        var cpus = new List<string>();
        for (int i = 0; i < 64; i++) if ((mask & (1L << i)) != 0) cpus.Add(i.ToString());
        return string.Join(",", cpus);
    }

    private ServiceConfig ReadConfig()
    {
        var cfg = new ServiceConfig
        {
            Application = _appBox.Text.Trim(),
            AppDirectory = _dirBox.Text.Trim(),
            AppParameters = _argsBox.Text,
            DisplayName = _displayNameBox.Text.Trim(),
            Description = _descBox.Text,
            StartType = _startTypeCombo.SelectedIndex switch { 1 => 5, 2 => 3, 3 => 4, _ => 2 },
            AppStdout = _stdoutBox.Text.Trim(),
            AppStderr = _stderrBox.Text.Trim(),
            AppStdin = _stdinBox.Text.Trim(),
            AppStdoutCopyAndTruncate = _truncateOutCheck.Checked ? 1 : 0,
            AppStderrCopyAndTruncate = _truncateErrCheck.Checked ? 1 : 0,
            AppTimestampLog = _timestampCheck.Checked ? 1 : 0,
            AppRotateFiles = _rotateCheck.Checked ? 1 : 0,
            AppRotateOnline = _rotateOnlineCheck.Checked ? 1 : 0,
            AppRotateSeconds = (int)_rotateSeconds.Value,
            AppRotateBytes = (long)_rotateBytes.Value,
            AppRotateDelay = (int)_rotateDelay.Value,
            AppPriority = PriorityToConst(_priorityCombo.SelectedIndex),
            AppAffinity = ParseAffinity(_affinityBox.Text.Trim()),
            AppNoConsole = _noConsoleCheck.Checked ? 1 : 0,
            AppExit = _exitActionCombo.SelectedIndex switch { 1 => "Ignore", 2 => "Exit", 3 => "Unclean", _ => "Restart" },
            AppRestartDelay = (int)_restartDelayNum.Value,
            AppThrottle = (int)_throttleNum.Value,
            AppStopMethodConsole = (int)_methodConsoleMs.Value,
            AppStopMethodWindow = (int)_methodWindowMs.Value,
            AppStopMethodThreads = (int)_methodThreadsMs.Value,
            AppKillProcessTree = _killTreeCheck.Checked ? 1 : 0,
            AppRedirectHook = 0,
        };

        int stopSkip = 0;
        if (!_methodConsole.Checked) stopSkip |= 1;
        if (!_methodWindow.Checked) stopSkip |= 2;
        if (!_methodThreads.Checked) stopSkip |= 4;
        if (!_methodTerminate.Checked) stopSkip |= 8;
        cfg.AppStopMethodSkip = stopSkip;

        cfg.AppEnvironment = _envReplaceCheck.Checked ? _envBox.Text : null;
        cfg.AppEnvironmentExtra = _envReplaceCheck.Checked ? null : _envBox.Text;

        cfg.Dependencies = new List<string>(_depBox.Text.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim()).Where(s => s.Length > 0));

        cfg.Interactive = _interactCheck.Checked;
        cfg.VirtualAccount = _virtualRadio.Checked;
        if (_accountRadio.Checked)
        {
            cfg.ObjectName = _userBox.Text.Trim();
            cfg.Password = _passBox.Text;
        }
        else cfg.ObjectName = null;

        // 钩子
        foreach (var (key, cmd) in _hooks) cfg.Hooks[key] = cmd;

        return cfg;
    }

    private void WriteConfig(ServiceConfig cfg)
    {
        _appBox.Text = cfg.Application ?? "";
        _dirBox.Text = cfg.AppDirectory ?? "";
        _argsBox.Text = cfg.AppParameters ?? "";
        _displayNameBox.Text = cfg.DisplayName ?? "";
        _descBox.Text = cfg.Description ?? "";
        _startTypeCombo.SelectedIndex = cfg.StartType switch { 5 => 1, 3 => 2, 4 => 3, _ => 0 };
        _stdoutBox.Text = cfg.AppStdout ?? "";
        _stderrBox.Text = cfg.AppStderr ?? "";
        _stdinBox.Text = cfg.AppStdin ?? "";
        _truncateOutCheck.Checked = cfg.AppStdoutCopyAndTruncate != 0;
        _truncateErrCheck.Checked = cfg.AppStderrCopyAndTruncate != 0;
        _timestampCheck.Checked = cfg.AppTimestampLog != 0;
        _rotateCheck.Checked = cfg.AppRotateFiles != 0;
        _rotateOnlineCheck.Checked = cfg.AppRotateOnline != 0;
        _rotateSeconds.Value = cfg.AppRotateSeconds;
        _rotateBytes.Value = cfg.AppRotateBytes;
        _rotateDelay.Value = cfg.AppRotateDelay;
        _priorityCombo.SelectedIndex = PriorityToIdx(cfg.AppPriority);
        _affinityBox.Text = FormatAffinity(cfg.AppAffinity);
        _noConsoleCheck.Checked = cfg.AppNoConsole != 0;
        _exitActionCombo.SelectedIndex = cfg.AppExit switch { "Ignore" => 1, "Exit" => 2, "Unclean" => 3, _ => 0 };
        _restartDelayNum.Value = cfg.AppRestartDelay;
        _throttleNum.Value = cfg.AppThrottle;
        _methodConsole.Checked = (cfg.AppStopMethodSkip & 1) == 0;
        _methodWindow.Checked = (cfg.AppStopMethodSkip & 2) == 0;
        _methodThreads.Checked = (cfg.AppStopMethodSkip & 4) == 0;
        _methodTerminate.Checked = (cfg.AppStopMethodSkip & 8) == 0;
        _methodConsoleMs.Value = cfg.AppStopMethodConsole;
        _methodWindowMs.Value = cfg.AppStopMethodWindow;
        _methodThreadsMs.Value = cfg.AppStopMethodThreads;
        _killTreeCheck.Checked = cfg.AppKillProcessTree != 0;

        if (!string.IsNullOrEmpty(cfg.AppEnvironment)) { _envBox.Text = cfg.AppEnvironment; _envReplaceCheck.Checked = true; }
        else { _envBox.Text = cfg.AppEnvironmentExtra ?? ""; _envReplaceCheck.Checked = false; }

        _depBox.Text = string.Join(Environment.NewLine, cfg.Dependencies);

        _interactCheck.Checked = cfg.Interactive;
        _virtualRadio.Checked = cfg.VirtualAccount;
        _accountRadio.Checked = !cfg.VirtualAccount && !string.IsNullOrEmpty(cfg.ObjectName) && !(cfg.ObjectName ?? "").StartsWith("LocalSystem");
        _localSystemRadio.Checked = !cfg.VirtualAccount && !_accountRadio.Checked;
        _userBox.Text = _accountRadio.Checked ? cfg.ObjectName ?? "" : "";
        _passBox.Text = _accountRadio.Checked ? cfg.Password ?? "" : "";
        ToggleLogon();

        _hooks.Clear();
        foreach (var (key, cmd) in cfg.Hooks) _hooks[key] = cmd;
        RefreshHooks();
    }

    // ---------- 操作 ----------

    private void Install()
    {
        var name = GetName();
        if (ServiceManager.Exists(name)) throw new InvalidOperationException($"服务 {name} 已存在，请使用“编辑服务”。");
        var cfg = ReadConfig();
        if (string.IsNullOrEmpty(cfg.Application)) throw new InvalidOperationException("请填写应用程序路径。");

        ServiceManager.Install(name, cfg);
        cfg.Save(name);
        MessageBox.Show($"服务 {name} 安装成功。", "NSSM#", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void Edit()
    {
        var name = GetName();
        if (!ServiceManager.Exists(name)) throw new InvalidOperationException($"服务 {name} 不存在。");

        if (!_editing)
        {
            WriteConfig(ServiceConfig.Load(name));
            _editing = true;
            _editBtn.Text = "保存修改";
        }
        else
        {
            var cfg = ReadConfig();
            if (string.IsNullOrEmpty(cfg.Application)) { MessageBox.Show("请填写应用程序路径。", "NSSM#", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            cfg.Save(name);
            ServiceManager.SetDescription(name, cfg.Description);
            MessageBox.Show($"服务 {name} 已更新。", "NSSM#", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _editing = false;
            _editBtn.Text = "编辑服务";
        }
    }

    private void Remove()
    {
        var name = GetName();
        if (!ServiceManager.Exists(name)) throw new InvalidOperationException($"服务 {name} 不存在。");
        if (MessageBox.Show($"确定要卸载服务 {name} 吗？", "NSSM#", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

        try { ServiceManager.Stop(name); } catch { }
        ServiceManager.Uninstall(name);
        ServiceConfig.DeleteParameters(name);
        MessageBox.Show($"服务 {name} 已卸载。", "NSSM#", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private static void SafeRun(Action action)
    {
        try { action(); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "NSSM#", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
