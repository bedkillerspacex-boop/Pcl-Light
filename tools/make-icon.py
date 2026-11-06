#!/usr/bin/env python3
"""生成 PCL Light 图标与启动闪屏图。

问题背景：WPF 的 SplashScreen 按「图片第一帧的像素尺寸」开窗。
原来闪屏直接指向 icon.ico，而 ico 的第一帧是 16px，
于是闪屏窗口只有 16 DIP —— 125% 缩放下实测 13×13，
在 1080p 屏幕上就是个点，用户形容「以为是蚂蚁」。

这里单独渲染一张 splash.png，尺寸是原来的 5 倍（16 × 5 = 80px），
画面内容与 icon.ico 完全一致，只是整体放大。

默认只写 splash.png；要连 icon.ico 一起重画，加 --write-ico。

用法：
    python tools/make-icon.py                  # 只生成 splash.png
    python tools/make-icon.py --write-ico      # 顺带重画 icon.ico
    python tools/make-icon.py --preview a.png  # 只出预览
"""

from __future__ import annotations

import argparse
import os

from PIL import Image, ImageDraw, ImageFilter, ImageFont

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.dirname(HERE)
IMAGES = os.path.join(ROOT, "Plain Craft Launcher 2", "Images")
ICON_PATH = os.path.join(IMAGES, "icon.ico")
ICON_PNG_PATH = os.path.join(IMAGES, "icon.png")
SPLASH_PATH = os.path.join(IMAGES, "splash.png")

# 白色底图，左上方更亮、右下方受光较少；LIGHT 字样使用黑色。
TILE_TOP = (255, 255, 255)
TILE_BOTTOM = (229, 232, 238)
TEXT = (12, 14, 18, 255)

SIZES = [16, 20, 24, 32, 40, 48, 64, 96, 128, 256]
SUPERSAMPLE = 8

FONT_BOLD = r"C:\Windows\Fonts\segoeuib.ttf"
FONT_LABEL = r"C:\Windows\Fonts\msyhbd.ttc"

TILE_RADIUS_RATIO = 0.22      # 与原图标一致
WORDMARK_TEXT = "LIGHT"
WORDMARK_WIDTH_RATIO = 0.75

# 闪屏：原来按 16px 开窗，放大 5 倍
SPLASH_BASE_SIZE = 16
SPLASH_SCALE = 5.0
SPLASH_SIZE = int(SPLASH_BASE_SIZE * SPLASH_SCALE)


def _rounded_tile(size: int) -> Image.Image:
    """白色圆角底图，光线从左上方照入，阴影落向右下方。"""
    w = size * SUPERSAMPLE
    grad = Image.new("RGBA", (1, w))
    for y in range(w):
        t = y / max(1, w - 1)
        grad.putpixel((0, y), (
            round(TILE_TOP[0] + (TILE_BOTTOM[0] - TILE_TOP[0]) * t),
            round(TILE_TOP[1] + (TILE_BOTTOM[1] - TILE_TOP[1]) * t),
            round(TILE_TOP[2] + (TILE_BOTTOM[2] - TILE_TOP[2]) * t),
            255,
        ))
    grad = grad.resize((w, w))

    mask = Image.new("L", (w, w), 0)
    ImageDraw.Draw(mask).rounded_rectangle(
        [0, 0, w - 1, w - 1], radius=int(w * TILE_RADIUS_RATIO), fill=255
    )

    pad = max(2, w // 22)
    # 左上方柔光从圆角块内部透出，右下方投影强化受光方向。
    glow = Image.new("RGBA", (w, w), (0, 0, 0, 0))
    glow_mask = mask.filter(ImageFilter.GaussianBlur(max(2, w // 24)))
    glow.paste((255, 255, 255, 150), (0, 0), glow_mask)

    shadow_mask = Image.new("L", (w, w), 0)
    shadow_mask.paste(mask, (max(2, w // 32), max(2, w // 26)))
    shadow_mask = shadow_mask.filter(ImageFilter.GaussianBlur(max(3, w // 26)))
    shadow = Image.new("RGBA", (w, w), (24, 30, 42, 0))
    shadow.putalpha(shadow_mask.point(lambda p: round(p * 0.62)))

    tile = Image.new("RGBA", (w, w), (0, 0, 0, 0))
    tile.alpha_composite(glow)
    tile.alpha_composite(shadow)
    tile.paste(grad, (0, 0), mask)

    # 左上角高光斑强化光源方向，蒙版限制在白色主体内。
    highlight = Image.new("RGBA", (w, w), (0, 0, 0, 0))
    highlight_draw = ImageDraw.Draw(highlight)
    highlight_draw.ellipse((pad - w * 0.12, pad - w * 0.16,
                            pad + w * 0.68, pad + w * 0.60), fill=(255, 255, 255, 145))
    highlight = highlight.filter(ImageFilter.GaussianBlur(max(2, w // 11)))
    transparent = Image.new("RGBA", (w, w), (0, 0, 0, 0))
    tile.alpha_composite(Image.composite(highlight, transparent, mask))

    # 细边缘高光把白色主体从透明背景中分离出来。
    edge = Image.new("RGBA", (w, w), (0, 0, 0, 0))
    edge_draw = ImageDraw.Draw(edge)
    edge_draw.rounded_rectangle([1, 1, w - 2, w - 2], radius=int(w * TILE_RADIUS_RATIO),
                                outline=(255, 255, 255, 230), width=max(1, w // 90))
    tile.alpha_composite(edge)
    return tile


def _font_for_width(text: str, target_width: float) -> ImageFont.FreeTypeFont:
    """二分找到让文字宽度最接近 target_width 的字号。"""
    lo, hi = 8, 4000
    best = ImageFont.truetype(FONT_BOLD, lo)
    while lo <= hi:
        mid = (lo + hi) // 2
        font = ImageFont.truetype(FONT_BOLD, mid)
        left, _, right, _ = font.getbbox(text)
        if right - left < target_width:
            lo = mid + 1
            best = font
        else:
            hi = mid - 1
    return best


def _draw_centered(tile: Image.Image, text: str, font: ImageFont.FreeTypeFont) -> None:
    draw = ImageDraw.Draw(tile)
    left, top, right, bottom = draw.textbbox((0, 0), text, font=font)
    w, h = tile.size
    # 黑色字样把阴影投向右下方，保持白底上的高对比度。
    x = (w - (right - left)) / 2 - left
    y = (h - (bottom - top)) / 2 - top + h * 0.018
    shadow = Image.new("RGBA", tile.size, (0, 0, 0, 0))
    shadow_draw = ImageDraw.Draw(shadow)
    shadow_draw.text((x + w * 0.014, y + h * 0.02), text, font=font,
                     fill=(22, 26, 33, 120), stroke_width=max(1, w // 180),
                     stroke_fill=(22, 26, 33, 100))
    shadow = shadow.filter(ImageFilter.GaussianBlur(max(1, w // 60)))
    tile.alpha_composite(shadow)
    draw.text(
        (x, y), text, font=font, fill=TEXT,
        stroke_width=max(1, w // 260), stroke_fill=(255, 255, 255, 220),
    )


def render(size: int) -> Image.Image:
    """按目标尺寸单独排版后渲染，不是拿大图直接缩，小尺寸才不会糊。"""
    tile = _rounded_tile(size)
    font = _font_for_width(WORDMARK_TEXT, tile.size[0] * WORDMARK_WIDTH_RATIO)
    _draw_centered(tile, WORDMARK_TEXT, font)
    return tile.resize((size, size), Image.LANCZOS)


def build() -> dict[int, Image.Image]:
    return {size: render(size) for size in SIZES}


def write_ico(images: dict[int, Image.Image], path: str) -> None:
    images[256].save(
        path,
        format="ICO",
        sizes=[(s, s) for s in SIZES],
        append_images=[images[s] for s in SIZES],
    )


def write_preview(path: str) -> None:
    pad = 18
    sheet = Image.new("RGB", (760, 230), (246, 247, 250))
    draw = ImageDraw.Draw(sheet)
    label = ImageFont.truetype(FONT_LABEL, 13)
    for x, y, bg, title in [(18, 18, (245, 246, 248), "浅色桌面"),
                            (390, 18, (30, 34, 42), "深色桌面")]:
        draw.rounded_rectangle([x, y, x + 350, y + 194], radius=18, fill=bg)
        draw.text((x + 16, y + 14), title, font=label,
                  fill=(120, 128, 140) if sum(bg) > 500 else (200, 206, 216))
        for i, size in enumerate([32, 48, 64, 96]):
            im = render(size)
            px = x + 25 + i * 78
            py = y + 58 + (96 - size) // 2
            sheet.paste(im, (px, py), im)
            draw.text((px + 5, y + 166), str(size), font=label,
                      fill=(120, 128, 140) if sum(bg) > 500 else (200, 206, 216))
    sheet.save(path)


def main() -> None:
    parser = argparse.ArgumentParser(description="生成启动闪屏图 / 图标")
    parser.add_argument("--write-ico", action="store_true", help="同时重画 icon.ico")
    parser.add_argument("--preview", help="只输出预览图到指定路径")
    args = parser.parse_args()

    if args.preview:
        write_preview(args.preview)
        print(f"preview -> {args.preview}")
        return

    render(SPLASH_SIZE).save(SPLASH_PATH)
    print(f"splash -> {SPLASH_PATH} ({SPLASH_SIZE}x{SPLASH_SIZE})")

    if args.write_ico:
        images = build()
        write_ico(images, ICON_PATH)
        images[256].save(ICON_PNG_PATH, format="PNG")
        print(f"ico -> {ICON_PATH} ({os.path.getsize(ICON_PATH)} bytes, {len(SIZES)} sizes)")
        print(f"png -> {ICON_PNG_PATH}")


if __name__ == "__main__":
    main()
