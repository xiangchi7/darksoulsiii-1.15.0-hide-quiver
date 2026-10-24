# How the quiver part id was found

Game: **Dark Souls III 1.15.0**, UXM-unpacked (`Game/parts/*.partsbnd.dcx` are loose files the engine reads).
Everything below was checked in game unless noted.

## 1. The objects

| object | model | LOD twin | notes |
|---|---|---|---|
| arrow quiver (player) | `wp_a_6399` | `6399_l` | 0.40 m tube with a rim, 2 meshes, 425 verts |
| bolt case (male) | `wp_a_6700` | `6700_l` | 0.14 × 0.26 × 0.17 m, 1 mesh, 700 verts |
| bolt case (female) | `wp_a_9700` | `9700_l` | same size, 2 meshes |
| arrows | `wp_a_7000`–`7071` | yes | one arrow per file, 3 LODs (shaft + fletching) |
| greatarrows | `wp_a_7300`–`7331` | yes | |
| bolts | `wp_a_7600`–`7661` | yes | |

The quiver and the bolt case are separate weapon models, not part of the armour and not part of the ammunition.

## 2. The numbering rule that found it

Part numbers carry over between games with a **+5000** offset:

- DS1: arrow quiver `wp_a_1399`, bolt quiver `wp_a_1700`
- DS3: bolt case `wp_a_6700` = 1700 + 5000 — confirmed by hiding it
- therefore DS3 arrow quiver = 1399 + 5000 = `wp_a_6399` — confirmed by hiding it

## 3. Where the data misleads you

- `EquipParamWeapon` rows `98000`–`99100` have `IsAutoEquip = 1` and a model id at **s16 @ `0xB8`**
  (`0x194` is `swordArtId`, and a `1401` there once made a greatsword row look like a bow row). They point at
  `6400`, `9400`–`9404` and `6700`, `9700`–`9704`. Only the bolt-case half is real; `6400`/`9400` are never
  drawn on the player.
- `wp_a_6400` even has a bone named `BD_M_2300_quiver` and renders as a tube with arrows. It is a decoy.
  A bone name, a matching size or a param row never proves what the engine draws.
- The community DS3 part list calls `6399` a "[Debug] Arrow" and does not list `6400`/`9400` at all. Such
  lists are incomplete; treat them as hints.
- `WepAbsorpPosParam` rows `44/45/46` point at back dummies `39/40/41`, but editing those fields, or parking
  the dummies in `c0000`, does nothing to the quiver.
- `wp_a_9401`–`9404` and `9701`–`9704` are referenced by params but have no loose file. Adding invisible loose
  files at those ids changed nothing either.

## 4. Telling which file is actually drawn

1. Dump geometry for every candidate: mesh count, vertex count, bbox, material, bone names (`--bones` and
   `--list` in `tools/flvprobe`). An arrow is 0.05×0.09×0.56 m, a bow 0.2×1.8×0.02 m, a quiver 0.4×0.1 m.
2. Bisect by hiding one family per round. Hiding `6700`/`9700` removed the bolt case but left the quiver, which
   proved both that loose-file replacement works and that the quiver is not in that family.
3. Marker substitution settles it. Copy an unmistakable model (the 2.7 m `wp_a_1360` greatbow) over the
   candidate file names. If the screen does not change, that file is not what the game draws. One restart,
   no ambiguity — this is what killed the `6400`/`9400` theory.
4. Then patch: scale every mesh vertex by `1e-4`, keep bones/materials/layouts byte-identical
   (`ds3tae flverdiff` reports SAME), keep the input's DCX type, back the original up as
   `*.pre-hidequiver`.

## 5. Cost of staying at the model layer

- The ammunition models are also the arrow on the string and the arrow in flight, so hiding them removes those
  as well. Not separable here; a runtime hook or dummy parking would be needed.
- NPC archers share the same models, so their quivers disappear too.
