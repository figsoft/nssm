# -*- coding: utf-8 -*-
"""Localize NSSM GUI (English strings) to Simplified Chinese.
Operates on the UTF-16 encoded messages.mc and nssm.rc, translating only the
English-language blocks while leaving French/Italian intact.
"""
import sys

SRC = r"D:\nssm-2.24-101-g897c7ad\src"

# ---------------------------------------------------------------------------
# messages.mc: NSSM_GUI_* -> list of physical lines for the English block.
# ---------------------------------------------------------------------------
MSG = {
    "NSSM_GUI_CREATEDIALOG_FAILED": ["创建对话框失败：", "%s"],
    "NSSM_GUI_MISSING_SERVICE_NAME": ["未指定有效的服务名称！"],
    "NSSM_GUI_MISSING_PATH": ["未指定有效的可执行文件路径！"],
    "NSSM_GUI_INVALID_OPTIONS": ["未指定有效的参数！"],
    "NSSM_GUI_MISSING_USERNAME": ["缺少账户名！"],
    "NSSM_GUI_INVALID_USERNAME": ["账户名无效！"],
    "NSSM_GUI_MISSING_PASSWORD": ["密码缺失或不匹配！"],
    "NSSM_GUI_INVALID_PASSWORD": ["密码无效！"],
    "NSSM_GUI_INVALID_DISPLAYNAME": ["显示名称无效！"],
    "NSSM_GUI_INVALID_DESCRIPTION": ["描述无效！"],
    "NSSM_GUI_OUT_OF_MEMORY_FOR_IMAGEPATH": [
        "构造 ImagePath 时出错！\\n这不应该发生。您可能内存不足，",
        "或者世界末日即将来临，或发生了同样糟糕的事情。",
    ],
    "NSSM_GUI_INVALID_ENVIRONMENT": ["环境变量应采用 KEY=VALUE 形式。"],
    "NSSM_GUI_INVALID_DEPENDENCIES": ["依赖项无效！"],
    "NSSM_GUI_INSTALL_SERVICE_FAILED": ["无法创建服务！", "也许它已经安装了……"],
    "NSSM_GUI_CREATE_PARAMETERS_FAILED": ["无法设置服务的启动参数！", "正在删除该服务……"],
    "NSSM_GUI_EDIT_PARAMETERS_FAILED": ["无法设置服务的启动参数！"],
    "NSSM_GUI_ASK_REMOVE_SERVICE": ["确定要删除服务“%s”吗？"],
    "NSSM_GUI_SERVICE_NOT_INSTALLED": ["无法打开服务！", "也许它尚未安装……"],
    "NSSM_GUI_REMOVE_SERVICE_FAILED": [
        "无法删除服务！请确认服务已停止后重试。",
        "如果该错误仍然存在，您可能需要将服务设置为不自动启动，",
        "重启计算机后再尝试删除。",
    ],
    "NSSM_GUI_BROWSE_FILTER_APPLICATIONS": ["应用程序%0"],
    "NSSM_GUI_BROWSE_FILTER_DIRECTORIES": ["目录%0"],
    "NSSM_GUI_BROWSE_FILTER_ALL_FILES": ["所有文件%0"],
    "NSSM_GUI_BROWSE_TITLE": ["选择应用程序文件"],
    "NSSM_GUI_TAB_APPLICATION": ["应用程序%0"],
    "NSSM_GUI_TAB_NATIVE": ["服务%0"],
    "NSSM_GUI_TAB_DETAILS": ["详细信息%0"],
    "NSSM_GUI_TAB_LOGON": ["登录%0"],
    "NSSM_GUI_TAB_DEPENDENCIES": ["依赖项%0"],
    "NSSM_GUI_TAB_PROCESS": ["进程%0"],
    "NSSM_GUI_TAB_SHUTDOWN": ["关闭%0"],
    "NSSM_GUI_TAB_EXIT": ["退出动作%0"],
    "NSSM_GUI_TAB_IO": ["I/O%0"],
    "NSSM_GUI_TAB_ROTATION": ["文件轮转%0"],
    "NSSM_GUI_TAB_ENVIRONMENT": ["环境变量%0"],
    "NSSM_GUI_TAB_HOOKS": ["钩子%0"],
    "NSSM_GUI_STARTUP_AUTOMATIC": ["自动%0"],
    "NSSM_GUI_STARTUP_DELAYED": ["自动（延迟启动）%0"],
    "NSSM_GUI_STARTUP_MANUAL": ["手动%0"],
    "NSSM_GUI_STARTUP_DISABLED": ["已禁用%0"],
    "NSSM_GUI_EXIT_RESTART": ["重启应用程序%0"],
    "NSSM_GUI_EXIT_IGNORE": ["不采取任何动作（兼容 srvany）%0"],
    "NSSM_GUI_EXIT_REALLY": ["停止服务（单次模式）%0"],
    "NSSM_GUI_EXIT_UNCLEAN": ["模拟崩溃（Vista 之前）%0"],
    "NSSM_GUI_REALTIME_PRIORITY_CLASS": ["实时%0"],
    "NSSM_GUI_HIGH_PRIORITY_CLASS": ["高%0"],
    "NSSM_GUI_ABOVE_NORMAL_PRIORITY_CLASS": ["高于标准%0"],
    "NSSM_GUI_NORMAL_PRIORITY_CLASS": ["标准%0"],
    "NSSM_GUI_BELOW_NORMAL_PRIORITY_CLASS": ["低于标准%0"],
    "NSSM_GUI_IDLE_PRIORITY_CLASS": ["低%0"],
    "NSSM_GUI_WARN_AFFINITY": [
        "该服务配置的处理器亲和范围指定的 CPU 数量",
        "超过了本系统的 CPU 数量。编辑该服务将会移除多余的 CPU。",
    ],
    "NSSM_GUI_WARN_AFFINITY_NONE": ["未选择任何 CPU！"],
    "NSSM_GUI_WARN_STDIO": [
        "该服务配置的 I/O 重定向设置无法用此 GUI 的简化选项表示。",
        "编辑服务后请检查注册表以确认其 I/O 重定向设置。",
    ],
    "NSSM_GUI_WARN_ROTATE_BYTES": [
        "该服务配置了 64 位文件大小阈值的文件轮转。",
        "此 GUI 只能显示 32 位设置。编辑服务后请检查注册表以确认其文件轮转设置。",
    ],
    "NSSM_GUI_WARN_ENVIRONMENT": [
        "该服务同时配置了兼容 srvany 的环境变量块和额外的环境变量块。",
        "此 GUI 只能显示其中一个。编辑该服务将导致其中一个环境变量块被删除。",
    ],
    "NSSM_GUI_AFFINITY_CPU": ["CPU%0"],
    "NSSM_GUI_HOOK_EVENT_START": ["应用程序启动%0"],
    "NSSM_GUI_HOOK_EVENT_STOP": ["服务停止%0"],
    "NSSM_GUI_HOOK_EVENT_EXIT": ["应用程序退出%0"],
    "NSSM_GUI_HOOK_EVENT_POWER": ["电源事件%0"],
    "NSSM_GUI_HOOK_EVENT_ROTATE": ["日志轮转%0"],
    "NSSM_GUI_HOOK_ACTION_START_PRE": ["启动应用程序之前%0"],
    "NSSM_GUI_HOOK_ACTION_START_POST": ["应用程序启动成功之后%0"],
    "NSSM_GUI_HOOK_ACTION_STOP_PRE": ["关闭应用程序之前%0"],
    "NSSM_GUI_HOOK_ACTION_EXIT_POST": ["应用程序退出之后%0"],
    "NSSM_GUI_HOOK_ACTION_POWER_CHANGE": ["电源设置更改%0"],
    "NSSM_GUI_HOOK_ACTION_POWER_RESUME": ["从待机恢复%0"],
    "NSSM_GUI_HOOK_ACTION_ROTATE_PRE": ["在线日志轮转之前%0"],
    "NSSM_GUI_HOOK_ACTION_ROTATE_POST": ["在线日志轮转之后%0"],
}

# ---------------------------------------------------------------------------
# nssm.rc: English-section string replacements (quoted forms, replace-all).
# Only applied to the text before the first "LANGUAGE LANG_FRENCH".
# ---------------------------------------------------------------------------
RC = [
    # dialog captions
    ('"NSSM service installer"', '"NSSM 服务安装程序"'),
    ('"NSSM service remover"', '"NSSM 服务卸载程序"'),
    ('"NSSM service editor"', '"NSSM 服务编辑器"'),
    # common
    ('"Service name:"', '"服务名称："'),
    ('"Install service"', '"安装服务"'),
    ('"Cancel"', '"取消"'),
    ('"Remove service"', '"卸载服务"'),
    ('"Edit service"', '"编辑服务"'),
    # Application tab
    ('"Application"', '"应用程序"'),
    ('"Path:"', '"路径："'),
    ('"Startup directory:"', '"启动目录："'),
    ('"Arguments:"', '"参数："'),
    # Service (native) tab
    ('"Service"', '"服务"'),
    ('"Image path:"', '"映像路径："'),
    # Details tab
    ('"Details"', '"详细信息"'),
    ('"Display name:"', '"显示名称："'),
    ('"Description:"', '"描述："'),
    ('"Startup type:"', '"启动类型："'),
    # Log on tab
    ('"Log on as"', '"登录身份"'),
    ('"Local System account"', '"本地系统账户"'),
    ('"Allow service to interact with desktop"', '"允许服务与桌面交互"'),
    ('"Virtual service account"', '"虚拟服务账户"'),
    ('"This account:"', '"此账户："'),
    ('"Password:"', '"密码："'),
    # Dependencies tab
    ('"This service depends on the following system components"', '"此服务依赖以下系统组件"'),
    # Process tab
    ('"Process"', '"进程"'),
    ('"Priority:"', '"优先级："'),
    ('"Affinity:"', '"亲和性："'),
    ('"All processors"', '"所有处理器"'),
    ('"Console window"', '"控制台窗口"'),
    ('"CPUs:"', '"CPU："'),
    # Shutdown tab
    ('"Shutdown"', '"关闭"'),
    ('"Generate Control-C"', '"生成 Control-C"'),
    ('"Timeout:"', '"超时："'),
    ('"ms"', '"毫秒"'),
    ('"Send WM_CLOSE to windows"', '"向窗口发送 WM_CLOSE"'),
    ('"Post WM_QUIT to threads"', '"向线程投递 WM_QUIT"'),
    ('"Terminate process"', '"终止进程"'),
    ('"Kill process tree"', '"结束进程树"'),
    # Exit actions tab
    ('"Throttling"', '"节流"'),
    ('"Delay restart if application runs for less than"', '"若应用程序运行时间短于以下值则延迟重启"'),
    ('"Restart"', '"重启"'),
    ('"Action to take when application exits\\nother than in response to a controlled\\nservice shutdown:"',
     '"应用程序退出（非响应受控服务关闭）时采取的动作："'),
    ('"Delay restart by"', '"延迟重启"'),
    # I/O tab
    ('"I/O redirection"', '"I/O 重定向"'),
    ('"Input (stdin):"', '"输入（stdin）："'),
    ('"Output (stdout):"', '"输出（stdout）："'),
    ('"Error (stderr):"', '"错误（stderr）："'),
    ('"Timestamp"', '"时间戳"'),
    # Rotation tab
    ('"File rotation"', '"文件轮转"'),
    ('"Replace existing Output and/or Error files"', '"替换现有输出和/或错误文件"'),
    ('"Rotate files"', '"轮转文件"'),
    ('"Rotate while service is running"', '"服务运行时轮转"'),
    ('"Restrict rotation to files older than"', '"仅轮转早于以下时间的文件"'),
    ('"seconds"', '"秒"'),
    ('"Restrict rotation to files bigger than"', '"仅轮转大于以下大小的文件"'),
    ('"bytes"', '"字节"'),
    # Environment tab
    ('"Environment variables"', '"环境变量"'),
    ('"Replace default environment (srvany compatible)"', '"替换默认环境（兼容 srvany）"'),
    # Hooks tab
    ('"Event hooks"', '"事件钩子"'),
    ('"Event:"', '"事件："'),
    ('"Command:"', '"命令："'),
    ('"Redirect output from hooks"', '"重定向钩子输出"'),
]


def read_u16(path):
    with open(path, "r", encoding="utf-16") as f:
        return f.read()


def write_u16(path, text):
    with open(path, "w", encoding="utf-16-le", newline="\r\n") as f:
        f.write("\ufeff" + text)


def localize_messages(path):
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
    print("messages.mc: translated %d English GUI messages" % changed)


def localize_rc(path):
    text = read_u16(path)
    marker = "LANGUAGE LANG_FRENCH"
    idx = text.find(marker)
    if idx < 0:
        print("ERROR: could not find %r in nssm.rc" % marker)
        sys.exit(1)
    english = text[:idx]
    rest = text[idx:]
    applied = 0
    for old, new in RC:
        n = english.count(old)
        if n:
            english = english.replace(old, new)
            applied += n
    write_u16(path, english + rest)
    print("nssm.rc: %d English string replacements applied" % applied)


if __name__ == "__main__":
    localize_messages(SRC + r"\messages.mc")
    localize_rc(SRC + r"\nssm.rc")
    print("Done.")
