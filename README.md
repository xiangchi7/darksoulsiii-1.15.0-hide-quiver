# DS3 Hide Quiver

Dark Souls III **1.15.0**.

Equipping a bow or crossbow no longer puts a quiver, bolt case, or the arrows inside them on your character.
Bows, crossbows, damage, stamina cost, weight and saves are untouched.

This is a set of replacement `parts` model files, so the game has to be UXM-unpacked (`Game\parts` as loose
files). No DLL, no param archive edits, no Mod Engine needed.

## Models replaced

78 files, each with its `_l` LOD twin:

- Arrow quiver: `wp_a_6399`
- Bolt case: `wp_a_6700` (male), `wp_a_9700` (female)
- Arrows: `wp_a_7000` – `7071`
- Greatarrows: `wp_a_7300` – `7331`
- Bolts: `wp_a_7600` – `7661`

## Install

1. Run `install.cmd` (defaults to `D:\steam\steamapps\common\DARK SOULS III\Game`)
2. Different path: `install.cmd "X:\...\DARK SOULS III\Game"`
3. Restart the game — `parts` models are only read at process start

Originals are backed up as `parts\<name>.pre-hidequiver`.

## Uninstall

Run `restore.cmd`. Files with a backup are restored and the backup deleted, files that were pure additions are
removed, so `parts` ends up exactly as it was before install.

## Credits and copyright

The `.dcx` files in `files` are modified FromSoftware game assets (© FromSoftware / Bandai Namco). They are
here for single-player modding and tinkering, not for commercial use. Scripts: MIT.
