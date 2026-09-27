#!/usr/bin/env python3
"""Localization guardrails.

Verifies that every localization key referenced in the code or XAML exists in
the neutral (English) .resx files, and reports keys that are defined but never
referenced.

Exit code 1 if any referenced key is missing.
"""
import os
import re
import sys
import xml.etree.ElementTree as ET
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent

RESOURCES = {
    "core": ROOT / "SyncData.Core" / "Localization" / "Strings.resx",
    "gui": ROOT / "SyncData.Gui" / "Localization" / "Strings.resx",
}

# Explicit call patterns.
CS_PATTERNS = [
    re.compile(r'CoreLocalizer\.(?:Get|Format)\(\s*"([^"]+)"'),
    re.compile(r'Localizer\.Instance\.Format\(\s*"([^"]+)"'),
    re.compile(r'Localizer\.Instance\[\s*"([^"]+)"\s*\]'),
    re.compile(r'Localizer\.Format\(\s*"([^"]+)"'),
    re.compile(r'Localizer\[\s*"([^"]+)"\s*\]'),
]

# Any string literal shaped like a known key prefix (catches literals used
# indirectly, e.g. assigned to a variable before use).
KEY_PREFIXES = r"App|Status|Action|Option|Label|Watermark|Update|Validator|Log|Sync|Ftp"
KEY_LITERAL = re.compile(r'"((?:' + KEY_PREFIXES + r')_[A-Za-z0-9_]+)"')

# XAML keys are referenced through {ReflectionBinding Path=[Key], ...}.
XAML_PATTERN = re.compile(r"Path=\[\s*([A-Za-z0-9_]+)\s*\]")

SOURCE_SUFFIXES = {".cs", ".axaml"}
SKIP_DIRS = {"bin", "obj", ".git", "artifacts", "SyncData.Tests"}


def neutral_keys(path: Path) -> set:
    keys = set()
    for data in ET.parse(path).getroot().findall("data"):
        name = data.get("name")
        if name:
            keys.add(name)
    return keys


def scan_references(root: Path) -> dict:
    references = {}
    for dirpath, dirnames, filenames in os.walk(root):
        dirnames[:] = [d for d in dirnames if d not in SKIP_DIRS]
        for filename in filenames:
            suffix = Path(filename).suffix
            if suffix not in SOURCE_SUFFIXES:
                continue
            path = Path(dirpath) / filename
            try:
                text = path.read_text(encoding="utf-8")
            except (UnicodeDecodeError, OSError):
                continue

            for lineno, line in enumerate(text.splitlines(), start=1):
                found = set()
                if suffix == ".cs":
                    for pattern in CS_PATTERNS:
                        found.update(pattern.findall(line))
                    found.update(KEY_LITERAL.findall(line))
                else:
                    found.update(XAML_PATTERN.findall(line))

                for key in found:
                    references.setdefault(key, []).append(f"{path.relative_to(root)}:{lineno}")

    return references


def main() -> int:
    all_keys = set()
    for path in RESOURCES.values():
        if not path.exists():
            print(f"ERROR: resource not found: {path}", file=sys.stderr)
            return 1
        all_keys |= neutral_keys(path)

    references = scan_references(ROOT)

    missing = sorted(key for key in references if key not in all_keys)
    unused = sorted(key for key in all_keys if key not in references)

    for key in missing:
        locations = ", ".join(references[key])
        print(f"ERROR: key '{key}' is used but not defined in any neutral .resx ({locations})")

    if unused:
        print("WARNING: keys defined but not referenced:")
        for key in unused:
            print(f"  - {key}")

    if missing:
        print(f"\n{len(missing)} missing key(s).", file=sys.stderr)
        return 1

    print(f"OK: {len(references)} referenced keys, all defined "
          f"({len(all_keys)} total). {len(unused)} unused key(s).")
    return 0


if __name__ == "__main__":
    sys.exit(main())
