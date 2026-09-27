#!/usr/bin/env python3
"""Generate the SyncData application icons (PNG + ICO) with Pillow.

Usage: python3 build/make-icons.py
"""
from pathlib import Path

from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parent.parent
ASSETS = ROOT / "SyncData.Gui" / "Assets"
ASSETS.mkdir(parents=True, exist_ok=True)

SIZE = 512
TOP = (0x2D, 0x6C, 0xDF)     # blue
BOTTOM = (0x14, 0x35, 0x9D)  # darker blue
WHITE = (255, 255, 255, 255)


def make_base() -> Image.Image:
    # Vertical gradient background
    gradient = Image.new("RGB", (SIZE, SIZE))
    draw = ImageDraw.Draw(gradient)
    for y in range(SIZE):
        t = y / (SIZE - 1)
        color = tuple(int(TOP[i] + (BOTTOM[i] - TOP[i]) * t) for i in range(3))
        draw.line([(0, y), (SIZE, y)], fill=color)

    # Rounded-corner mask
    mask = Image.new("L", (SIZE, SIZE), 0)
    ImageDraw.Draw(mask).rounded_rectangle([0, 0, SIZE - 1, SIZE - 1], radius=96, fill=255)

    image = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    image.paste(gradient, (0, 0), mask)
    return image


def draw_arrows(image: Image.Image) -> None:
    draw = ImageDraw.Draw(image)

    # Top arrow: pointing right
    draw.rounded_rectangle([130, 176, 300, 220], radius=22, fill=WHITE)
    draw.polygon([(300, 156), (300, 240), (380, 198)], fill=WHITE)

    # Bottom arrow: pointing left
    draw.rounded_rectangle([212, 292, 382, 336], radius=22, fill=WHITE)
    draw.polygon([(212, 272), (212, 356), (132, 314)], fill=WHITE)


def main() -> None:
    image = make_base()
    draw_arrows(image)

    png_path = ASSETS / "icon.png"
    image.save(png_path)

    ico_path = ASSETS / "icon.ico"
    image.save(
        ico_path,
        format="ICO",
        sizes=[(16, 16), (24, 24), (32, 32), (48, 48), (64, 64), (128, 128), (256, 256)],
    )

    print(f"Wrote {png_path}")
    print(f"Wrote {ico_path}")


if __name__ == "__main__":
    main()
