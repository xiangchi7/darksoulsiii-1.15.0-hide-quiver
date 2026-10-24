# DS3 Hide Quiver — 隐藏箭袋 / 弩箭袋 / 箭矢

**Dark Souls III 1.15.0** · model-layer mod · no DLL, no param repack, no Mod Engine required.

隐藏玩家身上背着的**箭袋**、**弩箭袋（弩箭匣）**，以及挂在里面的**箭矢 / 弩箭 / 大箭**。
纯模型层实现：只把相关模型的顶点缩成一个亚像素点，骨骼、材质、layout、dummy 全部保持原样。

| | 效果 |
|---|---|
| 箭袋 | ✔ 消失（`wp_a_6399` + `_l`） |
| 弩箭袋 / 箭匣 | ✔ 消失（`wp_a_6700` 男 / `wp_a_9700` 女，各含 `_l`） |
| 背上的箭 / 弩箭 / 大箭 | ✔ 消失（`7000`–`7071`、`7300`–`7331`、`7600`–`7661`） |
| 弓 / 弩本体、伤害、射速、存档 | 不受影响 |

共 **78** 个 `parts` 模型文件。

## 安装

前提：游戏是 **UXM 解包**状态（`Game\parts` 是散文件）。本 mod 直接替换这些散文件。

1. 双击 `install.cmd`（默认目标 `D:\steam\steamapps\common\DARK SOULS III\Game`）
2. 或指定路径：`install.cmd "X:\...\DARK SOULS III\Game"`
3. **重启游戏**（`parts` 模型只在进程启动时读取）

安装时会把原文件备份成 `parts\<文件名>.pre-hidequiver`。

## 卸载

双击 `restore.cmd` —— 有备份的还原原版并删除备份；纯追加的文件直接删除。卸载后 `Game\parts` 回到原样。

## 已知代价

- **拉弓时弦上的箭、飞行中的箭也会看不见**：它们和背上的箭是同一批模型，模型层无法只去掉一半。
  想要保留弦上的箭，只能走运行时/挂点方案（改 `c0000` 的挂点或写 DLL）。
- **NPC 弓箭手的箭袋和箭也会一起消失**（共用同一模型）。单机无影响；想要只对玩家生效需要 DLL 方案。

## 原理（一句话）

DS3 里箭袋 / 弩箭袋是**独立的武器模型**，编号沿用系列规律：DS1 箭袋 `wp_a_1399` / 弩箭袋 `wp_a_1700`，
DS3 是 **+5000** → 弩箭袋 `6700`（已实测确认）、箭袋 **`6399`**。把它们和弹药模型的顶点缩到 1/10000，
就等于不绘制。详细记录（含踩过的坑）见 [`docs/technical-notes.md`](docs/technical-notes.md)。

## 重新生成 / 自己改

`tools/flvprobe` 是一个小型 C# 工具（.NET 8+，依赖 [SoulsFormats](https://github.com/soulsmods/SoulsFormatsNEXT)），
可以列出 / 渲染 / 缩点 `partsbnd` 里的 FLVER：

```bash
# 隐藏指定模型（连同 _l LOD 版）
flvprobe --hidequiver "<Game>\parts" "<out>" "6399,6700,9700" 0.0001
```

`docs/technical-notes.md` 里有如何定位"到底是哪个模型被画出来"的完整方法（含标记替换法）。

## 版权与免责

- 本仓库中的 `.dcx` 文件是《Dark Souls III》(© FromSoftware / Bandai Namco) 原始资源的修改版，
  仅用于单机 Mod 制作与学习交流，请勿用于商业用途。
- 脚本与工具源码：MIT。
- 使用 Mod 时请**离线**游玩，避免联机软 ban 风险。
