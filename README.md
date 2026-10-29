# DS3 Hide Quiver · 隐藏箭袋 / 箭矢

Dark Souls III **1.15.0**

**[中文](#中文) · [English](#english)**

> 模型文件在 **Releases** 里（`HideQuiver-1.0.zip`），仓库只放说明和安装脚本。
> The model files live in **Releases** (`HideQuiver-1.0.zip`); the repository holds the description and the scripts only.

## 中文

装弓或弩时，角色身上不再出现箭袋、弩矢盒或插在里面的箭。弓、弩的属性、伤害、精力消耗、重量与存档都不受影响。

替换的是一组 `parts` 模型，所以游戏需为 **UXM 解包**状态（`Game\parts` 是散开的文件）。不涉及 DLL、不改 params、不需要 Mod Engine。

### 替换的模型

39 个模型、共 78 个文件（每个模型带一个 `_l` LOD）：

- 箭袋：`wp_a_6399`
- 弩矢盒：`wp_a_6700`（男）、`wp_a_9700`（女）
- 箭：`wp_a_7000` – `7071`
- 大箭：`wp_a_7300` – `7331`
- 弩矢：`wp_a_7600` – `7661`

### 安装

1. 到 [Releases](../../releases/latest) 下载 `HideQuiver-1.0.zip` 并解压。
2. 运行 `install.cmd`（默认目标 `D:\steam\steamapps\common\DARK SOULS III\Game`）；路径不同就传进去：`install.cmd "X:\...\DARK SOULS III\Game"`。
3. 重启游戏 —— `parts` 模型只在进程启动时读取。

原文件会备份成 `parts\<名字>.pre-hidequiver`。

### 卸载

运行 `restore.cmd`：有备份的还原并删掉备份，纯粹新增的文件直接删除，`parts` 回到安装前的样子。

### 版权

`files` 里的 `.dcx` 是修改过的 FromSoftware 游戏资源（© FromSoftware / Bandai Namco），仅供单机 mod 修改使用，不可商用。脚本：MIT。

## English

Equipping a bow or crossbow no longer puts a quiver, bolt case, or the arrows inside them on your character.
Bows, crossbows, damage, stamina cost, weight and saves are untouched.

This is a set of replacement `parts` model files, so the game has to be UXM-unpacked (`Game\parts` as loose
files). No DLL, no param archive edits, no Mod Engine needed.

### Models replaced

39 models, 78 files in total, each model with its `_l` LOD twin:

- Arrow quiver: `wp_a_6399`
- Bolt case: `wp_a_6700` (male), `wp_a_9700` (female)
- Arrows: `wp_a_7000` – `7071`
- Greatarrows: `wp_a_7300` – `7331`
- Bolts: `wp_a_7600` – `7661`

### Install

1. Download `HideQuiver-1.0.zip` from [Releases](../../releases/latest) and extract it.
2. Run `install.cmd` (defaults to `D:\steam\steamapps\common\DARK SOULS III\Game`); different path: `install.cmd "X:\...\DARK SOULS III\Game"`.
3. Restart the game — `parts` models are only read at process start.

Originals are backed up as `parts\<name>.pre-hidequiver`.

### Uninstall

Run `restore.cmd`. Files with a backup are restored and the backup deleted, files that were pure additions are
removed, so `parts` ends up exactly as it was before install.

### Credits and copyright

The `.dcx` files in the release zip are modified FromSoftware game assets (© FromSoftware / Bandai Namco). They are
for single-player modding and tinkering, not for commercial use. Scripts: MIT.
