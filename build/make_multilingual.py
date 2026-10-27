# -*- coding: utf-8 -*-
"""Rebuild messages.mc and nssm.rc as true 4-language resources.
Start from the ORIGINAL (English/French/Italian) backups and ADD Simplified
Chinese as a separate language block, instead of overwriting English.
"""
import os
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import localize      # GUI translations (MSG) + RC string map
import localize2     # MESSAGE/EVENT translations (MSG)

ZH = {}
ZH.update(localize.MSG)
ZH.update(localize2.MSG)

_HERE = os.path.dirname(os.path.abspath(__file__))
SRC = os.path.join(os.path.dirname(_HERE), "src")
BACKUP = os.path.join(_HERE, "backup")


def read_u16(p):
    with open(p, "r", encoding="utf-16") as f:
        return f.read()


def write_u16(p, text):
    with open(p, "w", encoding="utf-16-le", newline="\r\n") as f:
        f.write("\ufeff" + text)


def rebuild_messages():
    text = read_u16(BACKUP + r"\messages.mc.orig")
    lines = text.split("\n")

    # 1) Add Chinese to LanguageNames.
    out = []
    for line in lines:
        out.append(line)
        if line == "Italian=0x410:MSG00410":
            out.append("Chinese=0x0804:MSG00804")
    lines = out

    # 2) Insert "Language = Chinese" blocks after each Italian block.
    out = []
    cur = None
    in_italian = False
    for line in lines:
        if line.startswith("SymbolicName = "):
            cur = line[len("SymbolicName = "):].strip()
            in_italian = False
            out.append(line)
        elif line == "Language = Italian":
            in_italian = True
            out.append(line)
        elif line == "." and in_italian and cur in ZH:
            out.append(".")
            in_italian = False
            out.append("Language = Chinese")
            for zh in ZH[cur]:
                out.append(zh)
            out.append(".")
        else:
            out.append(line)

    write_u16(SRC + r"\messages.mc", "\n".join(out))
    print("messages.mc: Chinese blocks inserted for %d messages" % len(ZH))


def rebuild_rc():
    text = read_u16(BACKUP + r"\nssm.rc.orig")
    marker = "LANGUAGE LANG_FRENCH"
    idx = text.find(marker)
    if idx < 0:
        sys.exit("could not find %r in nssm.rc.orig" % marker)
    english_all = text[:idx]      # header + version + icon + English/neutral dialogs
    rest = text[idx:]             # French + Italian

    # Keep the header (defines/includes/version/icon) out of the duplicated
    # Chinese section, so includes like "messages.rc" only appear once.
    dialog_marker = "LANGUAGE LANG_ENGLISH, SUBLANG_ENGLISH_US"
    didx = english_all.find(dialog_marker)
    if didx < 0:
        sys.exit("could not find dialog start in nssm.rc.orig")
    header = english_all[:didx]
    english_dialogs = english_all[didx:]

    chinese = english_dialogs
    chinese = chinese.replace(
        "LANGUAGE LANG_ENGLISH, SUBLANG_ENGLISH_US",
        "LANGUAGE LANG_CHINESE, SUBLANG_CHINESE_SIMPLIFIED",
    )
    chinese = chinese.replace(
        "LANGUAGE LANG_NEUTRAL, SUBLANG_NEUTRAL",
        "LANGUAGE LANG_CHINESE, SUBLANG_CHINESE_SIMPLIFIED",
    )
    chinese = chinese.replace(
        "// English (US) resources", "// Chinese (PRC) resources"
    )
    for old, new in localize.RC:
        chinese = chinese.replace(old, new)

    write_u16(SRC + r"\nssm.rc", header + english_dialogs + chinese + rest)
    print("nssm.rc: Chinese dialog blocks added (header kept single)")


if __name__ == "__main__":
    rebuild_messages()
    rebuild_rc()
    print("Done.")
