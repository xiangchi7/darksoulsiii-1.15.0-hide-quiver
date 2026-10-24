using System;
using System.IO;
using System.Linq;
using SoulsFormats;

class BonesTool
{
    public static void Run(string dir)
    {
        foreach (var f in Directory.GetFiles(dir, "wp_a_*partsbnd.dcx").OrderBy(x => x))
        {
            try
            {
                var raw = File.ReadAllBytes(f);
                if (DCX.Is(raw)) raw = DCX.Decompress(raw);
                var bnd = BND4.Read(raw);
                foreach (var bf in bnd.Files)
                {
                    if (!bf.Name.ToLowerInvariant().EndsWith(".flver")) continue;
                    var v = FLVER2.Read(bf.Bytes);
                    var bn = v.Bones.Select(b => b.Name).Where(n => !string.IsNullOrEmpty(n));
                    Console.WriteLine(Path.GetFileName(f) + "\tmeshes=" + v.Meshes.Count + "\tverts=" + v.Meshes.Sum(m => m.Vertices.Count) + "\tbones=" + string.Join("|", bn));
                }
            }
            catch (Exception e) { Console.WriteLine(Path.GetFileName(f) + "\tERR\t" + e.Message); }
        }
    }
}
