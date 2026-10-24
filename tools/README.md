# flvprobe

Small C# console tool used to inspect and patch DS3 `partsbnd` bundles.

Build (needs .NET 8+ and `SoulsFormats.dll`, e.g. from
[soulsmods/SoulsFormatsNEXT](https://github.com/soulsmods/SoulsFormatsNEXT) or the copy shipped with `ds3tae`):

```bash
# drop SoulsFormats.dll next to flvprobe.csproj, the csproj already references it
dotnet build -c Release
```

Usage:

```
flvprobe <file.partsbnd.dcx> ...          # per-mesh: vertex count, bbox, material, dummy count
flvprobe --scan <partsDir>                # same, for every partsbnd in a folder (TSV)
flvprobe --bones <partsDir>               # file -> mesh/vert counts + bone names
flvprobe --list <file>                    # bundle entries, materials, MTD paths, bone list
flvprobe --fmg <file.fmg> <needle>        # item-name FMG dump / search
flvprobe --hidequiver <partsDir> <outDir> <id,id,...> [scale]   # shrink models (+ _l twins)
flvprobe --hidemesh <in> <out> <materialSubstr> <scale>         # shrink only matching meshes
flvprobe --sheet <partsDir> <out.ppm> [cell] [cols]             # render a contact sheet
flvprobe --memscan <processName> [regex]  # scan process memory for model-name strings
```

`--hidequiver` keeps bones, materials, layouts and dummies byte-identical and preserves the input's DCX type,
which is what makes the patched bundles loadable by the retail executable.
