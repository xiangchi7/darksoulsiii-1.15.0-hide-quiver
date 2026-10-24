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

## Things to know first

- The arrow nocked on the bowstring and arrows in flight are the same models as the arrows on your back, so
  they disappear too. There is no way to separate them at the model layer; keeping the on-string arrow needs a
  runtime hook or a character dummy edit.
- NPC archers use the same models, so their quivers go away as well. Cosmetic only in single player.
- Restore the original files before going online.

## Other files

- `tools/flvprobe` — the small tool written for this mod: lists meshes/bones of a `partsbnd`, renders contact
  sheets, shrinks models by id, hides a single mesh by material name. Needs .NET 8+ and `SoulsFormats.dll`.
- `docs/technical-notes.md` — how the part ids were identified, including the wrong guesses.

## Credits and copyright

The `.dcx` files in `files` are modified FromSoftware game assets (© FromSoftware / Bandai Namco). They are
here for single-player modding and tinkering, not for commercial use. Scripts and tool sources: MIT.
