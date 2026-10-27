using System.ComponentModel;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace NssmSharp;

/// <summary>
/// 服务控制管理器操作：安装/卸载/启动/停止，以及账户、依赖、恢复等配置。
/// 安装时把「自身」注册为服务宿主（ImagePath 指向本 exe + 服务名参数）。
/// </summary>
public static class ServiceManager
{
    private const uint SC_MANAGER_CONNECT = 0x0001;
    private const uint SC_MANAGER_CREATE_SERVICE = 0x0002;
    private const uint SERVICE_WIN32_OWN_PROCESS = 0x00000010;
    private const uint SERVICE_INTERACTIVE_PROCESS = 0x00000100;
    private const uint SERVICE_ERROR_NORMAL = 0x00000001;
    private const uint SERVICE_ALL_ACCESS = 0xF01FF;
    private const uint DELETE = 0x00010000;
    private const uint SERVICE_CONFIG_DESCRIPTION = 1;
    private const uint SERVICE_CONFIG_FAILURE_ACTIONS = 2;
    private const uint SERVICE_CONFIG_DELAYED_AUTO_START_INFO = 3;

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr OpenSCManager(string? machineName, string? databaseName, uint access);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr CreateService(
        IntPtr hSCManager, string lpServiceName, string lpDisplayName, uint dwDesiredAccess,
        uint dwServiceType, uint dwStartType, uint dwErrorControl, string lpBinaryPathName,
        string? lpLoadOrderGroup, IntPtr lpdwTagId, string? lpDependencies,
        string? lpServiceStartName, string? lpPassword);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool ChangeServiceConfig2(IntPtr hService, uint dwInfoLevel, IntPtr lpInfo);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool DeleteService(IntPtr hService);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool CloseServiceHandle(IntPtr hSCObject);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct SERVICE_DESCRIPTION { public IntPtr lpDescription; }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct SERVICE_FAILURE_ACTIONS
    {
        public uint dwResetPeriod;
        public IntPtr lpRebootMsg;
        public IntPtr lpCommand;
        public uint cActions;
        public IntPtr lpsaActions;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SC_ACTION { public int Type; public uint Delay; }

    public static bool Exists(string serviceName)
    {
        using var sc = new ServiceController(serviceName);
        try { _ = sc.Status; return true; }
        catch (InvalidOperationException) { return false; }
        catch (Win32Exception) { return false; }
    }

    public static void Install(string serviceName, ServiceConfig cfg)
    {
        string exePath = Environment.ProcessPath
            ?? throw new InvalidOperationException("无法确定程序路径");
        string imagePath = $"\"{exePath}\" {serviceName}";

        string displayName = string.IsNullOrEmpty(cfg.DisplayName) ? serviceName : cfg.DisplayName;
        uint serviceType = SERVICE_WIN32_OWN_PROCESS | (cfg.Interactive ? SERVICE_INTERACTIVE_PROCESS : 0);
        uint startType = cfg.StartType == 5 ? 2u : (uint)cfg.StartType; // 延迟自动先按自动装

        string? deps = null;
        if (cfg.Dependencies.Count > 0) deps = string.Join("\0", cfg.Dependencies) + "\0";

        string? startName = null, password = null;
        if (cfg.VirtualAccount) startName = $"NT SERVICE\\{serviceName}";
        else if (!string.IsNullOrEmpty(cfg.ObjectName)) { startName = cfg.ObjectName; password = cfg.Password; }

        IntPtr scm = OpenSCManager(null, null, SC_MANAGER_CREATE_SERVICE | SC_MANAGER_CONNECT);
        if (scm == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error(), "OpenSCManager 失败");
        try
        {
            IntPtr svc = CreateService(scm, serviceName, displayName, SERVICE_ALL_ACCESS,
                serviceType, startType, SERVICE_ERROR_NORMAL, imagePath,
                null, IntPtr.Zero, deps, startName, password);
            if (svc == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error(), "CreateService 失败");
            try
            {
                SetDescription(svc, cfg.Description);
                if (cfg.StartType == 5) SetDelayedAutoStart(svc, true);
                SetFailureActions(svc);
            }
            finally { CloseServiceHandle(svc); }
        }
        finally { CloseServiceHandle(scm); }
    }

    public static void SetDescription(string serviceName, string description)
    {
        IntPtr scm = OpenSCManager(null, null, SC_MANAGER_CONNECT);
        if (scm == IntPtr.Zero) return;
        try
        {
            IntPtr svc = OpenService(scm, serviceName, SERVICE_ALL_ACCESS);
            if (svc == IntPtr.Zero) return;
            try { SetDescription(svc, description); }
            finally { CloseServiceHandle(svc); }
        }
        finally { CloseServiceHandle(scm); }
    }

    private static void SetDescription(IntPtr svc, string description)
    {
        if (string.IsNullOrEmpty(description)) return;
        IntPtr ptr = Marshal.StringToHGlobalUni(description);
        try
        {
            var sd = new SERVICE_DESCRIPTION { lpDescription = ptr };
            IntPtr buf = Marshal.AllocHGlobal(Marshal.SizeOf(sd));
            Marshal.StructureToPtr(sd, buf, false);
            ChangeServiceConfig2(svc, SERVICE_CONFIG_DESCRIPTION, buf);
            Marshal.FreeHGlobal(buf);
        }
        finally { Marshal.FreeHGlobal(ptr); }
    }

    private static void SetDelayedAutoStart(IntPtr svc, bool delayed)
    {
        IntPtr buf = Marshal.AllocHGlobal(sizeof(int));
        Marshal.WriteInt32(buf, delayed ? 1 : 0);
        ChangeServiceConfig2(svc, SERVICE_CONFIG_DELAYED_AUTO_START_INFO, buf);
        Marshal.FreeHGlobal(buf);
    }

    private static void SetFailureActions(IntPtr svc)
    {
        var action = new SC_ACTION { Type = 1 /* 重启服务 */, Delay = 0 };
        IntPtr actionPtr = Marshal.AllocHGlobal(Marshal.SizeOf(action));
        Marshal.StructureToPtr(action, actionPtr, false);

        var fa = new SERVICE_FAILURE_ACTIONS
        {
            dwResetPeriod = 86400,
            lpRebootMsg = IntPtr.Zero,
            lpCommand = IntPtr.Zero,
            cActions = 1,
            lpsaActions = actionPtr,
        };
        IntPtr buf = Marshal.AllocHGlobal(Marshal.SizeOf(fa));
        Marshal.StructureToPtr(fa, buf, false);
        ChangeServiceConfig2(svc, SERVICE_CONFIG_FAILURE_ACTIONS, buf);
        Marshal.FreeHGlobal(buf);
        Marshal.FreeHGlobal(actionPtr);
    }

    public static void Uninstall(string serviceName)
    {
        IntPtr scm = OpenSCManager(null, null, SC_MANAGER_CONNECT);
        if (scm == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error(), "OpenSCManager 失败");
        try
        {
            IntPtr svc = OpenService(scm, serviceName, DELETE);
            if (svc == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error(), "OpenService 失败");
            try
            {
                if (!DeleteService(svc)) throw new Win32Exception(Marshal.GetLastWin32Error(), "DeleteService 失败");
            }
            finally { CloseServiceHandle(svc); }
        }
        finally { CloseServiceHandle(scm); }
    }

    public static void Start(string serviceName)
    {
        using var sc = new ServiceController(serviceName);
        if (sc.Status == ServiceControllerStatus.Stopped) sc.Start();
        sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
    }

    public static void Stop(string serviceName)
    {
        using var sc = new ServiceController(serviceName);
        if (sc.Status == ServiceControllerStatus.Running)
        {
            sc.Stop();
            sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(60));
        }
    }

    public static ServiceControllerStatus GetStatus(string serviceName)
    {
        using var sc = new ServiceController(serviceName);
        return sc.Status;
    }
}
