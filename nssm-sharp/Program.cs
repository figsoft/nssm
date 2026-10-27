using System.ServiceProcess;

namespace NssmSharp;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        if (Environment.UserInteractive)
        {
            // 交互式运行 → 显示配置窗口
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
        else
        {
            // 由 SCM 启动 → 作为服务宿主运行，args[0] 为服务名
            string serviceName = args.Length > 0 ? args[0] : "NssmSharp";
            ServiceBase.Run(new NssmService(serviceName));
        }
    }
}
