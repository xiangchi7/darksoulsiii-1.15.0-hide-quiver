# Technical notes — how the quiver was actually identified

Game: **Dark Souls III 1.15.0**, UXM-unpacked (`Game/parts/*.partsbnd.dcx` are loose files the engine reads).
Everything below is verified in game unless marked otherwise.

## 1. What the objects are

| object | model | LOD twin | notes |
|---|---|---|---|
| arrow quiver (player) | `wp_a_6399` | `6399_l` | 0.40 m tube with a rim, 2 meshes, 425 verts |
| bolt case (male) | `wp_a_6700` | `6700_l` | 0.14 × 0.26 × 0.17 m, 1 mesh, 700 verts |
| bolt case (female) | `wp_a_9700` | `9700_l` | same size, 2 meshes |
| arrows | `wp_a_7000`–`7071` | yes | one arrow × 3 LODs (shaft + fletching) per file |
| greatarrows | `wp_a_7300`–`7331` | yes | |
| bolts | `wp_a_7600`–`7661` | yes | |

The quiver is a **separate weapon-part model**, not part of the armour and not part of the ammunition.

## 2. The numbering rule that found it

The series keeps part numbers across games, offset by **+5000**:

- DS1: arrow quiver `wp_a_1399`, bolt quiver `wp_a_1700`
- DS3: bolt case `wp_a_6700` = 1700 + 5000  →  confirmed by hiding it
- therefore DS3 arrow quiver = **1399 + 5000 = `wp_a_6399`**  →  confirmed by hiding it

## 3. What the data *lied* about (do not repeat these)

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

## 4. How to identify "which file is actually drawn" (the method that worked)

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

## 5. Trade-offs of the model layer

- The ammunition models are also the arrow on the bowstring and the arrow in flight — hiding them removes
  those too. Not separable at this layer; a runtime hook or a dummy-parking patch would be needed.
- NPC archers share the same models, so their quivers vanish as well.
