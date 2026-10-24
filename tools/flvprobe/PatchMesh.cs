using System;
using System.IO;
using System.Linq;
using SoulsFormats;

class PatchMesh
{
    // Shrink only the meshes whose material name contains <needle> (case-insensitive); keep the rest byte-identical.
    public static void Run(string inPath, string outPath, string needle, float scale)
    {
        var raw = File.ReadAllBytes(inPath);
        DCX.Type t = DCX.Type.None;
        if (DCX.Is(raw)) raw = DCX.Decompress(raw, out t);
        var bnd = BND4.Read(raw);
        int hit = 0, total = 0;
        foreach (var bf in bnd.Files)
        {
            if (!bf.Name.ToLowerInvariant().EndsWith(".flver")) continue;
            var v = FLVER2.Read(bf.Bytes);
            for (int i = 0; i < v.Meshes.Count; i++)
            {
                total++;
                var m = v.Meshes[i];
                string mat = (m.MaterialIndex >= 0 && m.MaterialIndex < v.Materials.Count) ? v.Materials[m.MaterialIndex].Name : "";
                Console.WriteLine("  mesh" + i + " v=" + m.Vertices.Count + " mat=" + mat);
                if (mat.IndexOf(needle, StringComparison.OrdinalIgnoreCase) < 0) continue;
                foreach (var vert in m.Vertices) vert.Position = vert.Position * scale;
                hit++;
            }
            bf.Bytes = v.Write();
        }
        var np = bnd.Write();
        File.WriteAllBytes(outPath, t == DCX.Type.None ? np : DCX.Compress(np, t));
        Console.WriteLine(Path.GetFileName(inPath) + " -> hidden meshes: " + hit + "/" + total + " (needle='"+needle+"')");
    }
}
