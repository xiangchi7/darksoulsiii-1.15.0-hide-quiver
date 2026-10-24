using System;
using System.IO;
using SoulsFormats;

class PatchTool
{
    // shrink every mesh vertex to a sub-pixel speck at the model origin
    public static void Run(string inPath, string outPath, float scale)
    {
        var raw = File.ReadAllBytes(inPath);
        DCX.Type t = DCX.Type.None;
        if (DCX.Is(raw)) raw = DCX.Decompress(raw, out t);
        var bnd = BND4.Read(raw);
        int meshes = 0, verts = 0;
        foreach (var bf in bnd.Files)
        {
            if (!bf.Name.ToLowerInvariant().EndsWith(".flver")) continue;
            var v = FLVER2.Read(bf.Bytes);
            foreach (var m in v.Meshes)
            {
                foreach (var vert in m.Vertices)
                {
                    vert.Position = vert.Position * scale;
                    verts++;
                }
                meshes++;
            }
            bf.Bytes = v.Write();
        }
        var np = bnd.Write();
        File.WriteAllBytes(outPath, t == DCX.Type.None ? np : DCX.Compress(np, t));
        Console.WriteLine(Path.GetFileName(inPath) + " -> " + Path.GetFileName(outPath) + "  meshes=" + meshes + " verts=" + verts + " dcx=" + t);
    }
}
