using System;
using System.IO;
using System.Linq;
using System.Numerics;
using SoulsFormats;

class P
{
    static void Dump(string path)
    {
        string name = Path.GetFileName(path);
        byte[] raw;
        try { raw = File.ReadAllBytes(path); } catch (Exception e) { Console.WriteLine(name + "\tERR\t" + e.Message); return; }
        try
        {
            if (DCX.Is(raw)) raw = DCX.Decompress(raw);
            var bnd = BND4.Read(raw);
            foreach (var bf in bnd.Files)
            {
                if (!bf.Name.ToLowerInvariant().EndsWith(".flver")) continue;
                var v = FLVER2.Read(bf.Bytes);
                if (v.Meshes.Count == 0) continue;
                for (int mi = 0; mi < v.Meshes.Count; mi++)
                {
                    var m = v.Meshes[mi];
                    Vector3 lo = new Vector3(float.MaxValue), hi = new Vector3(float.MinValue);
                    foreach (var vert in m.Vertices)
                    {
                        var p = vert.Position;
                        lo = Vector3.Min(lo, p); hi = Vector3.Max(hi, p);
                    }
                    var size = hi - lo;
                    string mat = (m.MaterialIndex >= 0 && m.MaterialIndex < v.Materials.Count) ? v.Materials[m.MaterialIndex].Name : "?";
                    Console.WriteLine(string.Format("{0}\t{1}\tmesh{2}\tv={3}\t{4:F3}x{5:F3}x{6:F3}\tlo=({7:F2},{8:F2},{9:F2})\thi=({10:F2},{11:F2},{12:F2})\tmat={13}\tdummies={14}",
                        name, Path.GetFileName(bf.Name), mi, m.Vertices.Count, size.X, size.Y, size.Z, lo.X, lo.Y, lo.Z, hi.X, hi.Y, hi.Z, mat, v.Dummies.Count));
                }
            }
        }
        catch (Exception e) { Console.WriteLine(name + "\tERR\t" + e.GetType().Name + ": " + e.Message); }
    }

    static void Main(string[] a)
    {
        if (a.Length >= 2 && a[0] == "--memscan") { MemScan.Run(a[1], a.Length>2 ? a[2] : "WP_A_[0-9]{4}"); return; }
        if (a.Length >= 5 && a[0] == "--hidemesh") { PatchMesh.Run(a[1], a[2], a[3], float.Parse(a[4], System.Globalization.CultureInfo.InvariantCulture)); return; }
        if (a.Length >= 3 && a[0] == "--sheet") { RenderTool.Sheet(a[1], a[2], a.Length>3?int.Parse(a[3]):128, a.Length>4?int.Parse(a[4]):8); return; }
        if (a.Length >= 4 && a[0] == "--hidequiver") { var ids = Array.ConvertAll(a[3].Split(','), int.Parse); float sc = a.Length > 4 ? float.Parse(a[4], System.Globalization.CultureInfo.InvariantCulture) : 0.0001f; HideQuiver.Run(a[1], a[2], ids, sc, false); return; }
        if (a.Length >= 2 && a[0] == "--bones") { BonesTool.Run(a[1]); return; }
        if (a.Length >= 2 && a[0] == "--list") { ListTool.Run(a[1]); return; }
        if (a.Length >= 3 && a[0] == "--fmg") { FmgTool.Run(a[1], a[2]); return; }
        if (a.Length >= 2 && a[0] == "--scan")
        {
            foreach (var f in Directory.GetFiles(a[1], "*partsbnd.dcx").OrderBy(x => x)) Dump(f);
            return;
        }
        foreach (var f in a) Dump(f);
    }
}
