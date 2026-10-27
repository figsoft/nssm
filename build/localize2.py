# -*- coding: utf-8 -*-
"""Localize NSSM command-line (NSSM_MESSAGE_*) and event-log (NSSM_EVENT_*)
messages to Simplified Chinese. Only the English blocks are translated.
"""
import sys

SRC = r"D:\nssm-2.24-101-g897c7ad\src"

MSG = {
    # ------------------------------------------------------------------ MESSAGE
    "NSSM_MESSAGE_USAGE": [
        "NSSM：非吸吮式服务管理器",
        "版本 %s %s，%s",
        "用法：nssm <选项> [<参数> ...]",
        "",
        "显示服务安装界面：",
        "",
        "        nssm install [<服务名>]",
        "",
        "无需确认直接安装服务：",
        "",
        "        nssm install <服务名> <应用程序> [<参数> ...]",
        "",
        "显示服务编辑界面：",
        "",
        "        nssm edit <服务名>",
        "",
        "直接获取或编辑服务参数：",
        "",
        "        nssm dump <服务名>",
        "",
        "        nssm get <服务名> <参数> [<子参数>]",
        "",
        "        nssm set <服务名> <参数> [<子参数>] <值>",
        "",
        "        nssm reset <服务名> <参数> [<子参数>]",
        "",
        "显示服务卸载界面：",
        "",
        "        nssm remove [<服务名>]",
        "",
        "无需确认直接卸载服务：",
        "",
        "        nssm remove <服务名> confirm",
        "",
        "管理服务：",
        "",
        "        nssm start <服务名>",
        "",
        "        nssm stop <服务名>",
        "",
        "        nssm restart <服务名>",
        "",
        "        nssm status <服务名>",
        "",
        "        nssm statuscode <服务名>",
        "",
        "        nssm rotate <服务名>",
        "",
        "        nssm processes <服务名>",
    ],
    "NSSM_MESSAGE_NOT_ADMINISTRATOR_CANNOT_INSTALL": ["安装服务需要管理员权限。"],
    "NSSM_MESSAGE_NOT_ADMINISTRATOR_CANNOT_EDIT": ["编辑服务需要管理员权限。"],
    "NSSM_MESSAGE_NOT_ADMINISTRATOR_CANNOT_REMOVE": ["卸载服务需要管理员权限。"],
    "NSSM_MESSAGE_PRE_REMOVE_SERVICE": ["无需确认直接卸载服务：nssm remove <服务名> confirm"],
    "NSSM_MESSAGE_OUT_OF_MEMORY": ["为 %s 分配内存不足（位置：%s）！"],
    "NSSM_MESSAGE_OPEN_SERVICE_MANAGER_FAILED": ["打开服务管理器出错！"],
    "NSSM_MESSAGE_QUERYSERVICECONFIG_FAILED": ["查询服务“%s”出错！", "QueryServiceConfig()：%s"],
    "NSSM_MESSAGE_QUERYSERVICECONFIG2_FAILED": ["查询服务“%s”出错！", "QueryServiceConfig2(%s)：%s"],
    "NSSM_MESSAGE_INVALID_SERVICE": ["服务“%s”不是有效的 %s 服务！", "可执行文件为 %s"],
    "NSSM_MESSAGE_CANNOT_EDIT": ["服务“%s”不是 %s 服务！"],
    "NSSM_MESSAGE_PATH_TOO_LONG": ["%s 的完整路径过长！"],
    "NSSM_MESSAGE_FLAGS_TOO_LONG": ["程序参数过长！"],
    "NSSM_MESSAGE_OUT_OF_MEMORY_FOR_IMAGEPATH": ["为 ImagePath 分配内存不足！"],
    "NSSM_MESSAGE_CREATESERVICE_FAILED": ["创建服务出错！", "CreateService()：%s"],
    "NSSM_MESSAGE_GRANTED_LOGON_AS_SERVICE": ["已向账户 %s 授予“作为服务登录”权限。"],
    "NSSM_MESSAGE_GRANT_LOGON_AS_SERVICE_FAILED": ["向账户 %s 授予“作为服务登录”权限失败！"],
    "NSSM_MESSAGE_LSAOPENPOLICY_FAILED": ["LsaOpenPolicy()：%s"],
    "NSSM_MESSAGE_LSALOOKUPNAMES_FAILED": ["查找用户名 %s 的 SID 失败！", "LsaLookupNames()：%s"],
    "NSSM_MESSAGE_INITIALIZESID_FAILED": ["初始化用户名 %s 的 SID 失败！", "InitializeSid()：%s"],
    "NSSM_MESSAGE_LSAENUMERATEACCOUNTRIGHTS_FAILED": ["检查 %s 是否拥有“作为服务登录”权限失败！", "LsaEnumerateAccountRights()：%s"],
    "NSSM_MESSAGE_LSAADDACCOUNTRIGHTS_FAILED": ["LsaAddAccountRights()：%s"],
    "NSSM_MESSAGE_CHANGESERVICECONFIG_FAILED": ["编辑服务出错！", "ChangeServiceConfig()：%s"],
    "NSSM_MESSAGE_SETVALUE_FAILED": ["写入注册表值 %s 失败：", "RegSetValueEx()：%s"],
    "NSSM_MESSAGE_REGDELETEVALUE_FAILED": ["删除服务“%s”的注册表值 %s 出错！", "RegDeleteValue()：%s"],
    "NSSM_MESSAGE_INVALID_PARAMETER": ["参数“%s”无效。有效参数为："],
    "NSSM_MESSAGE_MISSING_SUBPARAMETER": ["参数“%s”需要子参数！"],
    "NSSM_MESSAGE_NATIVE_PARAMETER": ["参数“%s”仅对由 %s 管理的服务有效！"],
    "NSSM_MESSAGE_NO_DEFAULT_VALUE": ["参数“%s”没有有意义的默认值！"],
    "NSSM_MESSAGE_GET_SETTING_FAILED": ["获取服务“%s”的参数“%s”出错！"],
    "NSSM_MESSAGE_SET_SETTING_FAILED": ["设置服务“%s”的参数“%s”出错！"],
    "NSSM_MESSAGE_SET_SETTING": ["已为服务“%s”设置参数“%s”。"],
    "NSSM_MESSAGE_RESET_SETTING": ["已将服务“%s”的参数“%s”重置为默认值。"],
    "NSSM_MESSAGE_INVALID_EXIT_ACTION": ["退出动作“%s”无效。有效的退出动作为："],
    "NSSM_MESSAGE_INVALID_SERVICE_TYPE": ["服务类型“%s”无效。有效的类型为："],
    "NSSM_MESSAGE_SERVICE_CONFIG_DELAYED_AUTO_START_INFO_FAILED": ["为服务“%s”配置延迟启动出错。该服务将自动启动。", "ChangeServiceConfig2()：%s"],
    "NSSM_MESSAGE_INVALID_SERVICE_STARTUP": ["启动类型“%s”无效。有效的类型为："],
    "NSSM_MESSAGE_INVALID_PRIORITY": ["进程优先级“%s”无效。有效的优先级为："],
    "NSSM_MESSAGE_MISSING_PASSWORD": ["设置“%s”需要同时提供用户名和密码！"],
    "NSSM_MESSAGE_INTERACTIVE_NOT_LOCALSYSTEM": ["服务类型“%s”对服务“%s”无效。", "只有以 %s 账户运行的服务才能与桌面交互。"],
    "NSSM_MESSAGE_CREATE_PARAMETERS_FAILED": ["设置服务的启动参数出错！"],
    "NSSM_MESSAGE_SERVICE_INSTALLED": ["服务“%s”安装成功！"],
    "NSSM_MESSAGE_OPENSERVICE_FAILED": ["无法打开服务！", "OpenService()：%s"],
    "NSSM_MESSAGE_ENUMSERVICESSTATUS_FAILED": ["无法打开服务！", "EnumServicesStatus()：%s"],
    "NSSM_MESSAGE_DELETESERVICE_FAILED": ["删除服务出错！"],
    "NSSM_MESSAGE_SERVICE_REMOVED": ["服务“%s”卸载成功！"],
    "NSSM_MESSAGE_SERVICE_EDITED": ["服务“%s”编辑成功！"],
    "NSSM_MESSAGE_CANNOT_RENAME_SERVICE": ["无法重命名服务！"],
    "NSSM_MESSAGE_EFFECTIVE_AFFINITY_MASK": [
        "请求的处理器亲和范围 %s 不恰当。",
        "本系统的最大亲和范围为 %s。",
        "请求的亲和值将原样写入注册表。",
        "但请注意，实际生效的亲和值将为 %s。",
    ],
    "NSSM_MESSAGE_BOGUS_AFFINITY_MASK": [
        "亲和值规格“%s”无效。",
        "有效格式形如“0-2,4-6,10,15”",
        "本系统中标识符必须在 0-%d 范围内。",
    ],
    "NSSM_MESSAGE_BAD_CONTROL_RESPONSE": ["%s：响应 %s 控制时出现意外状态 %s。"],
    "NSSM_MESSAGE_LSALOOKUPSIDS_FAILED": ["查找 SID 对应的用户名失败。", "LsaLookupSids()：%s"],
    "NSSM_MESSAGE_CREATEWELLKNOWNSID_FAILED": ["创建 %s 的 SID 失败！"],
    "NSSM_MESSAGE_INVALID_HOOK_EVENT": ["无效的钩子事件。有效的钩子事件为："],
    "NSSM_MESSAGE_INVALID_HOOK_ACTION": ["事件 %s 的钩子动作无效。有效的钩子动作为："],
    "NSSM_MESSAGE_INVALID_HOOK_NAME": ["无效的钩子名称。名称应采用 <事件>/<动作> 格式指定。"],
    # -------------------------------------------------------------------- EVENT
    "NSSM_EVENT_DISPATCHER_FAILED": ["StartServiceCtrlDispatcher() 失败：", "%1"],
    "NSSM_EVENT_OPENSCMANAGER_FAILED": ["无法连接到服务管理器！", "也许您需要管理员权限……"],
    "NSSM_EVENT_OUT_OF_MEMORY": ["在 %2 中为 %1 分配内存不足！"],
    "NSSM_EVENT_GET_PARAMETERS_FAILED": ["获取服务 %1 的启动参数失败。"],
    "NSSM_EVENT_REGISTERSERVICECTRLHANDER_FAILED": ["RegisterServiceCtrlHandlerEx() 失败：", "%1"],
    "NSSM_EVENT_START_SERVICE_FAILED": ["无法为服务 %2 启动 %1。", "错误代码：%3。"],
    "NSSM_EVENT_RESTART_SERVICE_FAILED": ["为服务 %2 重启 %1 失败。", "进入休眠……"],
    "NSSM_EVENT_STARTED_SERVICE": ["已在 %4 中为服务 %3 启动 %1 %2。"],
    "NSSM_EVENT_REGISTERWAITFORSINGLEOBJECT_FAILED": [
        "当 %2 退出时，服务 %1 可能仍显示为运行中。",
        "RegisterWaitForSingleObject() 失败：",
        "%3",
    ],
    "NSSM_EVENT_CREATEPROCESS_FAILED": ["启动服务 %1 失败。无法启动程序 %2。", "CreateProcess() 失败：", "%3"],
    "NSSM_EVENT_TERMINATEPROCESS": ["正在终止进程 %2，因为服务 %1 正在停止。"],
    "NSSM_EVENT_PROCESS_ALREADY_STOPPED": ["已请求停止服务 %1。无需执行任何操作，因为程序 %2 未在运行。"],
    "NSSM_EVENT_ENDED_SERVICE": ["服务 %2 的程序 %1 已退出，返回代码 %3。"],
    "NSSM_EVENT_EXIT_RESTART": ["服务 %1 对退出代码 %2 的动作是 %3。", "正在尝试重启 %4。"],
    "NSSM_EVENT_EXIT_IGNORE": ["服务 %1 对退出代码 %2 的动作是 %3。", "不会采取任何动作重启 %4。"],
    "NSSM_EVENT_EXIT_REALLY": ["服务 %1 对退出代码 %2 的动作是 %3。", "正在退出。"],
    "NSSM_EVENT_OPENKEY_FAILED": ["无法打开注册表项 HKLM\\%1：", "%2"],
    "NSSM_EVENT_QUERYVALUE_FAILED": ["无法读取注册表值 %1：", "%2"],
    "NSSM_EVENT_SETVALUE_FAILED": ["无法写入注册表值 %1：", "%2"],
    "NSSM_EVENT_EXIT_UNCLEAN": ["服务 %1 对退出代码 %2 的动作是 %3。", "正在退出。"],
    "NSSM_EVENT_GRACEFUL_SUICIDE": [
        "服务 %1 的应用程序 %2 以退出代码 0 退出，但默认退出动作是 %3。",
        "执行 %4 动作会导致该服务被标记为失败并触发恢复动作。",
        "该服务将改为正常停止。要消除此消息，请将退出代码 0 的退出动作显式配置为 %5 或 %6。",
    ],
    "NSSM_EVENT_EXPANDENVIRONMENTSTRINGS_FAILED": ["无法展开注册表值 %1：", "%2"],
    "NSSM_EVENT_KILLING": ["正在终止服务 %1 的进程 %2 的进程树，退出代码 %3"],
    "NSSM_EVENT_CREATETOOLHELP32SNAPSHOT_PROCESS_FAILED": ["终止服务 %1 时无法创建运行中进程的快照：", "%2"],
    "NSSM_EVENT_PROCESS_ENUMERATE_FAILED": ["终止服务 %1 时无法枚举运行中的进程：", "%2"],
    "NSSM_EVENT_OPENPROCESS_FAILED": ["终止服务 %2 时无法打开 PID 为 %1 的进程句柄：", "%3"],
    "NSSM_EVENT_KILL_PROCESS_TREE": ["正在终止 PID %2 的进程树中的 PID %1，因为服务 %3 正在停止。"],
    "NSSM_EVENT_TERMINATEPROCESS_FAILED": ["为服务 %2 终止 PID 为 %1 的进程失败：", "%3"],
    "NSSM_EVENT_NO_FLAGS": ["服务 %2 的注册表项 %1 未设置。", "启动时不会向 %3 传递任何参数。"],
    "NSSM_EVENT_NO_DIR": ["服务 %2 的注册表项 %1 未设置。", "假定启动目录为 %3。"],
    "NSSM_EVENT_NO_DIR_AND_NO_FALLBACK": [
        "服务 %2 的注册表项 %1 未设置。",
        "此外，在尝试选择备用启动目录时 ExpandEnvironmentStrings(\"%%SYSTEMROOT%%\") 失败。",
    ],
    "NSSM_EVENT_CREATETOOLHELP32SNAPSHOT_THREAD_FAILED": ["终止服务 %1 时无法创建运行中线程的快照：", "%2"],
    "NSSM_EVENT_THREAD_ENUMERATE_FAILED": ["终止服务 %1 时无法枚举运行中的线程：", "%2"],
    "NSSM_EVENT_THROTTLED": ["服务 %1 运行时间少于 %2 毫秒。", "重启将延迟 %3 毫秒。"],
    "NSSM_EVENT_RESET_THROTTLE": ["请求恢复服务 %1。重启尝试的节流将被重置。"],
    "NSSM_EVENT_BOGUS_THROTTLE": [
        "用于指定服务 %1 被视为成功启动所需的最少毫秒数的注册表值 %2 不是 REG_DWORD 类型。将使用默认时间 %3 毫秒。",
    ],
    "NSSM_EVENT_CREATEWAITABLETIMER_FAILED": ["为服务 %1 创建可等待计时器失败：", "%2", "被节流的重启将无法被中断。"],
    "NSSM_EVENT_CREATEPROCESS_FAILED_INVALID_ENVIRONMENT": [
        "启动服务 %1 失败。无法启动程序 %2。",
        "CreateProcess() 失败并返回 ERROR_INVALID_PARAMETER，且注册表值 %3 中设置了进程环境。环境很可能是配置错误的。%3 应为 REG_MULTI_SZ 类型，由 KEY=VALUE 形式的字符串组成。",
    ],
    "NSSM_EVENT_INVALID_ENVIRONMENT_STRING_TYPE": ["服务 %2 的环境声明 %1 不是 REG_MULTI_SZ 类型，将被忽略。"],
    "NSSM_EVENT_SERVICE_CONTROL_HANDLED": ["服务 %1 收到 %2 控制消息，将予以处理。"],
    "NSSM_EVENT_SERVICE_CONTROL_NOT_HANDLED": ["服务 %1 收到不支持的 %2 控制消息，将不予处理。"],
    "NSSM_EVENT_SERVICE_CONTROL_UNKNOWN": ["服务 %1 收到未知的服务控制消息 %2，将被忽略。"],
    "NSSM_EVENT_SERVICE_CONFIG_FAILURE_ACTIONS_FAILED": [
        "为服务 %1 配置服务失败动作出错。如果该服务以非零退出代码正常退出，将不触发恢复动作。",
        "ChangeServiceConfig2() 失败：",
        "%2",
    ],
    "NSSM_EVENT_GETPROCESSTIMES_FAILED": ["GetProcessTimes() 失败：", "%1"],
    "NSSM_EVENT_ATTACHCONSOLE_FAILED": ["为服务 %1 附加控制台出错。", "AttachConsole() 失败：", "%2"],
    "NSSM_EVENT_SETCONSOLECTRLHANDLER_FAILED": ["为发送到服务 %1 的 Control-C 事件设置空处理器出错。", "SetConsoleCtrlHandler() 失败：", "%2"],
    "NSSM_EVENT_GENERATECONSOLECTRLEVENT_FAILED": ["为服务 %1 生成 Control-C 事件出错。", "GenerateConsoleCtrlEvent() 失败：", "%2"],
    "NSSM_EVENT_FREECONSOLE_FAILED": ["为服务 %1 分离控制台出错。", "FreeConsole() 失败：", "%2"],
    "NSSM_EVENT_CREATEFILE_FAILED": ["CreateFile() 无法打开 %1：", "%2"],
    "NSSM_EVENT_DUPLICATEHANDLE_FAILED": ["复制先前为 %1 打开的句柄到 %2 时出错。", "DuplicateHandle() 失败：", "%3"],
    "NSSM_EVENT_GET_OUTPUT_HANDLES_FAILED": ["设置一个或多个 I/O 文件句柄出错。服务 %1 将不会被启动。"],
    "NSSM_EVENT_BOGUS_STOP_METHOD_SKIP": [
        "用于指定 %3 在尝试停止服务 %1 时跳过的方法的注册表值 %2 不是 REG_DWORD 类型。将使用所有可用的方法。",
    ],
    "NSSM_EVENT_PROCESS_STILL_ACTIVE": [
        "服务 %1 正在停止，但 PID %2 仍在运行。",
        "通常 %3 会调用 TerminateProcess() 作为最后手段来确保进程被停止，但已设置注册表值 %4，且未尝试所有进程终止方法。",
        "将无法再尝试控制该应用程序，服务将报告为已停止状态。",
    ],
    "NSSM_EVENT_LOADLIBRARY_FAILED": ["加载 %1 DLL 出错！", "LoadLibrary() 失败：", "%2"],
    "NSSM_EVENT_GETPROCADDRESS_FAILED": ["GetProcAddress(%1) 失败：", "%2"],
    "NSSM_EVENT_BOGUS_KILL_CONSOLE_GRACE_PERIOD": [
        "用于指定发送 Control-C 事件后等待服务 %1 停止的最多毫秒数的注册表值 %2 不是 REG_DWORD 类型。将使用默认时间 %3 毫秒。",
    ],
    "NSSM_EVENT_BOGUS_KILL_WINDOW_GRACE_PERIOD": [
        "用于指定向应用程序管理的窗口发送 WM_CLOSE 消息后等待服务 %1 停止的最多毫秒数的注册表值 %2 不是 REG_DWORD 类型。将使用默认时间 %3 毫秒。",
    ],
    "NSSM_EVENT_BOGUS_KILL_THREADS_GRACE_PERIOD": [
        "用于指定向应用程序管理的线程的消息队列投递 WM_QUIT 消息后等待服务 %1 停止的最多毫秒数的注册表值 %2 不是 REG_DWORD 类型。将使用默认时间 %3 毫秒。",
    ],
    "NSSM_EVENT_AWAITING_SHUTDOWN": ["%1 已等待 %3/%5 毫秒，等待 %2 服务退出。", "将在 %4 毫秒后再次更新。"],
    "NSSM_EVENT_CREATETHREAD_FAILED": ["CreateThread() 失败：", "%1"],
    "NSSM_EVENT_STARTUP_DELAY_TOO_LONG": [
        "服务 %1 被视为成功启动所需的最少毫秒数设置为 %2。在服务更新其状态之前，Windows 服务控制管理器的访问将被阻塞，因此 %3 最多等待 %4 毫秒后才会将服务状态报告为运行中。如果服务运行时间少于完整的 %2 毫秒，将强制执行服务重启节流。",
    ],
    "NSSM_EVENT_SETENVIRONMENTVARIABLE_FAILED": ["SetEnvironmentVariable(%1=%2) 失败：", "%3"],
    "NSSM_EVENT_ROTATE_FILE_FAILED": ["为服务 %1 轮转输出文件 %2 失败。", "%3 对文件 %4 失败：", "%5"],
    "NSSM_EVENT_SERVICE_CONFIG_DESCRIPTION_FAILED": ["为服务 %1 设置描述出错。", "ChangeServiceConfig2() 失败：", "%2"],
    "NSSM_EVENT_SERVICE_CONFIG_DELAYED_AUTO_START_INFO_FAILED": ["为服务 %1 配置延迟启动出错。该服务将自动启动。", "ChangeServiceConfig2() 失败：", "%2"],
    "NSSM_EVENT_BOGUS_PRIORITY": [
        "用于指定服务 %1 优先级类的注册表值 %2 无效。",
        "%2 应为 REG_DWORD 类型，且对应 SetPriorityClass() 函数的有效参数。",
        "服务 %1 将以正常优先级启动。",
    ],
    "NSSM_EVENT_BOGUS_AFFINITY_MASK": ["请求的亲和范围 %2 无效。", "服务 %1 将被允许在任意 CPU 上运行。"],
    "NSSM_EVENT_EFFECTIVE_AFFINITY_MASK": [
        "请求的处理器亲和范围 %2 不恰当。",
        "本系统的最大亲和范围为 %3。",
        "服务 %1 将以 %4 的亲和范围运行。",
    ],
    "NSSM_EVENT_GETPROCESSAFFINITYMASK_FAILED": ["无法为服务 %1 确定合适的亲和掩码。", "GetProcessAffinityMask()：%2"],
    "NSSM_EVENT_SETPROCESSAFFINITYMASK_FAILED": ["无法为服务 %1 设置请求的亲和掩码。", "SetProcessAffinityMask()：%2"],
    "NSSM_EVENT_BOGUS_RESTART_DELAY": [
        "用于指定服务 %1 的重启延迟毫秒数的注册表值 %2 不是 REG_DWORD 类型。",
        "将不采用强制延迟。",
    ],
    "NSSM_EVENT_RESTART_DELAY": ["服务 %1 的重启将延迟 %2 毫秒。"],
    "NSSM_EVENT_CREATEPIPE_FAILED": ["无法建立管道以读取服务 %1 的输出。", "在服务运行期间将无法轮转日志文件 %2。", "CreatePipe()：%3"],
    "NSSM_EVENT_READFILE_FAILED": ["读取服务 %1 的输出失败。", "如果该错误持续存在，将不再向 %2 写入更多数据。", "ReadFile()：%3"],
    "NSSM_EVENT_WRITEFILE_FAILED": ["将服务 %1 的输出写入文件 %2 失败。", "如果该错误持续存在，可能会丢失部分数据。", "WriteFile()：%3"],
    "NSSM_EVENT_SOMEBODY_SET_UP_US_THE_BOM": [
        "检测到服务 %1 的输出为 UTF-16 格式，但写入相应字节顺序标记（BOM）的尝试失败。",
        "后续向 %2 写入数据的尝试很可能会失败。即使成功，读取该文件的应用程序也可能无法将其识别为",
        "UTF-16 格式。",
        "WriteFile()：%3",
    ],
    "NSSM_EVENT_ROTATED": ["已将服务 %1 的输出文件 %2 轮转为 %3。"],
    "NSSM_EVENT_AWAITING_SINGLE_HANDLE": ["%1 已等待 %3/%5 毫秒，等待 %2 句柄。", "将在 %4 毫秒后再次更新。"],
    "NSSM_EVENT_PRESTART_HOOK_ABORT": ["服务 %3 的 %1/%2 钩子返回退出代码 %4。", "服务启动将被中止。"],
    "NSSM_EVENT_HOOK_CREATEPROCESS_FAILED": ["无法运行服务 %3 的 %1/%2 钩子。无法启动程序 %4。", "CreateProcess() 失败：", "%5"],
    "NSSM_EVENT_GET_HOOK_FAILED": ["在注册表中找不到服务 %3 的 %1/%2 钩子的命令。"],
}


def read_u16(path):
    with open(path, "r", encoding="utf-16") as f:
        return f.read()


def write_u16(path, text):
    with open(path, "w", encoding="utf-16-le", newline="\r\n") as f:
        f.write("\ufeff" + text)


def localize(path):
    text = read_u16(path)
    lines = text.split("\n")
    out = []
    cur = None
    changed = 0
    i = 0
    while i < len(lines):
        line = lines[i]
        if line.startswith("SymbolicName = "):
            cur = line[len("SymbolicName = "):].strip()
            out.append(line)
            i += 1
        elif line == "Language = English" and cur in MSG:
            out.append(line)
            i += 1
            while i < len(lines) and lines[i] != ".":
                i += 1
            for zh in MSG[cur]:
                out.append(zh)
            out.append(".")
            changed += 1
            i += 1
        else:
            out.append(line)
            i += 1
    write_u16(path, "\n".join(out))
    print("translated %d messages (MESSAGE + EVENT)" % changed)


if __name__ == "__main__":
    localize(SRC + r"\messages.mc")
    print("Done.")
