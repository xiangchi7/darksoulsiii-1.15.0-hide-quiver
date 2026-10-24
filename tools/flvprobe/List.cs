using System;
using System.IO;
using SoulsFormats;

class ListTool
{
    public static void Run(string path)
    {
        var raw = File.ReadAllBytes(path);
        if (DCX.Is(raw)) raw = DCX.Decompress(raw);
        var bnd = BND4.Read(raw);
        foreach (var bf in bnd.Files)
        {
            Console.WriteLine("ENTRY  " + bf.Name + "  (" + bf.Bytes.Length + " bytes)");
            if (!bf.Name.ToLowerInvariant().EndsWith(".flver")) continue;
            var v = FLVER2.Read(bf.Bytes);
            foreach (var mat in v.Materials)
                Console.WriteLine("   mat: " + mat.Name + "  mtd=" + mat.MTD);
            Console.WriteLine("   bones=" + v.Bones.Count + " meshes=" + v.Meshes.Count + " dummies=" + v.Dummies.Count);
            foreach (var b in v.Bones) Console.WriteLine("     bone " + b.Name);
        }
    }
}
