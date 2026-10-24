# 黑魂3 隐藏箭袋 / 弩箭袋

适用版本：Dark Souls III **1.15.0**。

装上之后，角色身上背的箭袋、腰上的弩箭匣，以及里面插着的箭和弩箭都不再显示。弓和弩本体没动，
伤害、射速、负重、存档都不受影响。

游戏需要是 UXM 解包状态（`Game\parts` 是散文件），因为这就是一套替换 `parts` 模型文件的 Mod，
不写 DLL，也不改参数归档，所以不需要 Mod Engine。

## 会隐藏哪些模型

一共 78 个 `parts` 文件，每项都带 `_l` 远景版本：

- 箭袋：`wp_a_6399`
- 弩箭袋：`wp_a_6700`（男角色）、`wp_a_9700`（女角色）
- 箭矢：`wp_a_7000` ~ `7071`
- 大箭：`wp_a_7300` ~ `7331`
- 弩箭：`wp_a_7600` ~ `7661`

## 安装

1. 双击 `install.cmd`，默认装到 `D:\steam\steamapps\common\DARK SOULS III\Game`
2. 装在别处的话用 `install.cmd "X:\...\DARK SOULS III\Game"`
3. 装完重启游戏，模型只在启动时读取

原来的文件会备份成 `parts\<文件名>.pre-hidequiver`。

## 卸载

双击 `restore.cmd`。有备份的还原成原版并删掉备份，纯追加的文件直接删掉，`parts` 目录会回到装之前的样子。

## 需要先知道的

- 拉弓时搭在弦上的箭、射出去飞行的箭，跟背上那一束箭是同一批模型，所以也会一起看不见。
  模型层没办法只去掉背上那部分，要保留弦上的箭得动运行时或者角色挂点。
- NPC 弓箭手的箭袋和箭用的是同一批模型，也会一起消失。单机玩没有影响。
- 联机前先换回原版，避免软 ban。

## 文件里的其他东西

- `tools/flvprobe`：做这个 Mod 时写的小工具，能列模型网格/骨骼、渲染缩略图、按 id 缩点、只隐藏某个网格。
  需要 .NET 8 以上和 SoulsFormats.dll。
- `docs/technical-notes.md`：箭袋的模型 id 是怎么找出来的，以及中间试错的过程。

## 版权

`files` 里的 `.dcx` 是《黑暗之魂3》原始资源的修改版，版权归 FromSoftware / Bandai Namco，只用于单机 Mod
和自娱自乐，别拿去做商业用途。脚本和工具源码用 MIT。

---

**English**: hides the equipped arrow quiver, bolt case and carried ammunition in Dark Souls III 1.15.0 by
replacing 78 `parts` models (vertices scaled to 1e-4, everything else untouched). Requires an UXM-unpacked
install. Run `install.cmd` to apply, `restore.cmd` to revert. The arrow on the bowstring and arrows in flight
share the same models and disappear too, as do NPC archers' quivers.
