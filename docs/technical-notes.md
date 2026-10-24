# 箭袋模型 id 是怎么找到的

游戏：**Dark Souls III 1.15.0**，UXM 解包状态（`Game/parts/*.partsbnd.dcx` 是散文件，引擎直接读）。
除特别说明外，下面这些都进游戏验证过。

## 1. 这些物件分别是什么

| 物件 | 模型 | 远景 LOD | 说明 |
|---|---|---|---|
| 箭袋（玩家） | `wp_a_6399` | `6399_l` | 0.40 m 管状带口沿，2 个网格，425 顶点 |
| 弩箭匣（男） | `wp_a_6700` | `6700_l` | 0.14×0.26×0.17 m，1 个网格，700 顶点 |
| 弩箭匣（女） | `wp_a_9700` | `9700_l` | 尺寸同上，2 个网格 |
| 箭矢 | `wp_a_7000`–`7071` | 有 | 每个文件是一支箭的 3 级 LOD（箭杆 + 箭羽） |
| 大箭 | `wp_a_7300`–`7331` | 有 | |
| 弩箭 | `wp_a_7600`–`7661` | 有 | |

箭袋和弩箭匣都是独立的武器模型，既不属于护甲，也不属于弹药。

## 2. 编号规律（找对 id 的关键）

The series keeps part numbers across games, offset by **+5000**:

- DS1：箭袋 `wp_a_1399`，弩箭袋 `wp_a_1700`
- DS3：弩箭匣 `wp_a_6700` = 1700 + 5000，隐藏后确认有效
- 所以 DS3 的箭袋 = 1399 + 5000 = `wp_a_6399`，隐藏后确认有效

## 3. 数据里会误导人的地方

- `EquipParamWeapon` rows `98000`–`99100` carry `IsAutoEquip = 1` and a model id at **s16 @ `0xB8`**
  (`0x194` is `swordArtId`, and a `1401` there made a greatsword row look like a bow row).
  They point at `6400`, `9400`–`9404` (quiver-ish) and `6700`, `9700`–`9704` (bolt case).
  Only the **bolt-case half** is real. `6400`/`9400` are never drawn on the player.
- `wp_a_6400` even carries a bone called `BD_M_2300_quiver` and renders as a tube-with-arrows —
  it is a decoy. A bone name, a dimension match or a param row is **never proof** of what the engine draws.
- The DS3 part list on the wiki labels `6399` as "[Debug] Arrow" and does not contain `6400`/`9400` at all.
  Wiki part lists are incomplete; treat them as hints.
- `WepAbsorpPosParam` rows `44/45/46` (the ammo rows) point at back dummies `39/40/41`, but editing those
  fields — and parking the dummies in `c0000` — changes nothing about the quiver.
- `wp_a_9401`–`9404` and `9701`–`9704` are referenced by params but have **no loose file**;
  inventing invisible loose files at those ids changes nothing either.

## 4. 确认到底是哪个文件被画出来的方法

1. **Dump geometry for every candidate**: mesh count, vertex count, bbox, material, bone names
   (`--bones`, `--list` in `tools/flvprobe`). An arrow is 0.05×0.09×0.56 m, a bow 0.2×1.8×0.02 m,
   a quiver 0.4×0.1 m — dimensions separate the classes quickly.
2. **Bisect by hiding, one family per round.** Hiding `6700`/`9700` removed the bolt case but not the arrow
   quiver → the quiver is *not* in that family, and loose-file replacement demonstrably works.
3. **Marker substitution is the decisive test.** Copy an unmistakable model (a 2.7 m greatbow,
   `wp_a_1360`) over the candidate file names. If nothing changes on screen, that file is not what is drawn —
   one restart, no ambiguity. This is what killed the `6400`/`9400` theory.
4. Only then patch: scale every mesh vertex by `1e-4`, keep bones/materials/layouts byte-identical
   (`ds3tae flverdiff` reports SAME), keep the input's DCX type, and back the original up as
   `*.pre-hidequiver`.

## 5. 模型层的代价

- The ammunition models are also the arrow on the bowstring and the arrow in flight — hiding them removes
  those too. Not separable at this layer; a runtime hook or a dummy-parking patch would be needed.
- NPC archers share the same models, so their quivers vanish as well.
