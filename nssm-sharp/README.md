# NSSM#（NssmSharp）

NSSM（Non-Sucking Service Manager）的 **C# 重写版**，用 **Windows 窗体（WinForms）+ .NET 9** 实现，功能与原版对齐。

> 一句话：把任意 exe、bat、php-cgi、nginx 等控制台程序包装成 Windows 系统服务，自带进程守护，程序崩溃自动重启。

## 为什么有它

原版 NSSM 是 C++ 编写，功能强大但代码较底层。本项目用 C# 以同样的「把自己注册成服务 → 运行时拉起并守护真正的程序」原理重写，便于理解、二次开发和维护，并提供了图形化配置界面。

## 功能

- **图形界面**：安装 / 编辑 / 卸载 / 启动 / 停止服务，11 个配置页签
- **进程守护**：程序崩溃或退出后按退出动作自动重启（Restart / Ignore / Exit / Unclean）
- **重启节流与延迟**
- **I/O 重定向**：stdout / stderr / stdin 到文件，支持截断与时间戳
- **日志轮转**：在线轮转，按文件大小 / 时间
- **多种停止方式**：Control-C / WM_CLOSE / WM_QUIT / 终止进程，各自独立超时，支持结束进程树
- **进程控制**：优先级、CPU 亲和、控制台窗口
- **环境变量**：替换或追加
- **事件钩子**：Start / Stop / Exit / Power / Rotate 事件触发自定义命令
- **依赖项**、**账户登录**（本地系统 / 虚拟账户 / 指定账户）、**桌面交互**
- **启动类型**：自动 / 延迟自动 / 手动 / 禁用
- **失败恢复动作**：服务异常退出时自动重启

## 构建

需要 .NET 9 SDK（Windows）。

```bash
cd nssm-sharp
dotnet build -c Release
# 产物：bin/Release/net9.0-windows/NssmSharp.exe
```

## 使用

1. 双击 `NssmSharp.exe`（会请求管理员权限）
2. 填「服务名称」和「应用程序」路径
3. 点「安装服务」→ 完成
4. 用「启动 / 停止」管理，用「编辑服务」修改配置，用「卸载服务」删除

## 架构

| 文件 | 职责 |
|------|------|
| `Program.cs` | 入口分发：交互式运行→GUI；被 SCM 拉起→服务宿主 |
| `NssmService.cs` | 服务宿主（ServiceBase）：守护、重启、停止方式、轮转、钩子 |
| `ServiceManager.cs` | SCM 操作：安装/卸载/启停（P/Invoke CreateService 等） |
| `ServiceConfig.cs` | 配置模型 + 注册表读写 |
| `MainForm.cs` | WinForms 图形界面（11 个页签） |
| `app.manifest` | 管理员权限清单 |

## 配置存储

与原版 NSSM **使用相同的注册表键**，两者可互相查看 / 迁移：

```
HKLM\SYSTEM\CurrentControlSet\Services\<服务名>\Parameters\
```

因此：用原版 NSSM 安装的服务，本工具能读；用本工具安装的服务，原版也能读。

## 与原版 NSSM 的差异说明

- **Control-C 停止方式为尽力而为**：原版会为子进程创建隐藏控制台以可靠送达 Ctrl+C；本版默认用 .NET `Process` 启动（无控制台），Ctrl+C 通常不会送达，会顺势落到 WM_CLOSE / 终止进程。其余功能完整对齐。
- **界面为中文**，语言固定（暂未做多语言切换）。
- 无命令行（CLI）模式，仅图形界面。

## 许可证

沿用原版 NSSM 的公有领域（Public Domain）声明。感谢原作者 Iain Patterson 的无私奉献。
