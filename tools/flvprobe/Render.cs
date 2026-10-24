using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using SoulsFormats;

class RenderTool
{
    // Renders every mesh of every wp_a_*partsbnd into a contact sheet (PPM), one cell per model.
    public static void Sheet(string partsDir, string outPpm, int cell, int cols)
    {
        var files = Directory.GetFiles(partsDir, "wp_a_*partsbnd.dcx")
            .Where(f => !f.Contains("_l.partsbnd"))
            .OrderBy(f => f).ToList();
        int rows = (files.Count + cols - 1) / cols;
        int W = cols * cell, H = rows * cell;
        var img = new byte[W * H * 3];
        for (int i = 0; i < img.Length; i++) img[i] = (byte)(i % 3 == 0 ? 24 : i % 3 == 1 ? 24 : 32);
        for (int idx = 0; idx < files.Count; idx++)
        {
            var tris = new List<(Vector3 a, Vector3 b, Vector3 c)>();
            try { LoadTris(files[idx], tris); } catch { }
            int cx = (idx % cols) * cell, cy = (idx / cols) * cell;
            DrawCell(img, W, H, cx, cy, cell, tris);
            Console.WriteLine((idx + 1) + "/" + files.Count + "  " + Path.GetFileName(files[idx]) + "  tris=" + tris.Count);
        }
        using (var fs = new FileStream(outPpm, FileMode.Create))
        using (var bw = new BinaryWriter(fs))
        {
            bw.Write(System.Text.Encoding.ASCII.GetBytes("P6\n" + W + " " + H + "\n255\n"));
            bw.Write(img);
        }
    }

    static void LoadTris(string path, List<(Vector3, Vector3, Vector3)> tris)
    {
        var raw = File.ReadAllBytes(path);
        if (DCX.Is(raw)) raw = DCX.Decompress(raw);
        var bnd = BND4.Read(raw);
        foreach (var bf in bnd.Files)
        {
            if (!bf.Name.ToLowerInvariant().EndsWith(".flver")) continue;
            var v = FLVER2.Read(bf.Bytes);
            foreach (var m in v.Meshes)
            {
                var idxs = m.FaceSets.Count > 0 ? m.FaceSets[0].Indices : new List<int>();
                if (idxs.Count < 3) continue;
                for (int i = 0; i + 2 < idxs.Count; i += 3)
                {
                    int a = idxs[i], b = idxs[i + 1], c = idxs[i + 2];
                    if (a >= m.Vertices.Count || b >= m.Vertices.Count || c >= m.Vertices.Count) continue;
                    tris.Add((m.Vertices[a].Position, m.Vertices[b].Position, m.Vertices[c].Position));
                }
            }
        }
    }

    static void DrawCell(byte[] img, int W, int H, int ox, int oy, int cell, List<(Vector3 a, Vector3 b, Vector3 c)> tris)
    {
        if (tris.Count == 0) return;
        var min = new Vector3(float.MaxValue); var max = new Vector3(float.MinValue);
        foreach (var t in tris) foreach (var p in new[] { t.a, t.b, t.c }) { min = Vector3.Min(min, p); max = Vector3.Max(max, p); }
        var c0 = (min + max) * 0.5f;
        float ext = Math.Max(Math.Max(max.X - min.X, max.Y - min.Y), max.Z - min.Z);
        if (ext <= 1e-6f) return;
        float scale = (cell * 0.42f) / ext;
        var zbuf = new float[cell * cell];
        for (int i = 0; i < zbuf.Length; i++) zbuf[i] = float.MinValue;
        var light = Vector3.Normalize(new Vector3(0.4f, 0.6f, 1f));
        foreach (var t in tris)
        {
            // view: x right, y up, z depth (orthographic looking down +z)
            Vector3 P(Vector3 p) => new Vector3((p.X - c0.X) * scale, -(p.Y - c0.Y) * scale, (p.Z - c0.Z) * scale);
            var n = Vector3.Cross(t.b - t.a, t.c - t.a);
            if (n.LengthSquared() < 1e-12f) continue;
            n = Vector3.Normalize(n);
            float b = 0.25f + 0.75f * Math.Abs(Vector3.Dot(n, light));
            byte col = (byte)Math.Clamp(40 + b * 200, 0, 255);
            var A = P(t.a); var B = P(t.b); var C = P(t.c);
            float minX = Math.Min(A.X, Math.Min(B.X, C.X)), maxX = Math.Max(A.X, Math.Max(B.X, C.X));
            float minY = Math.Min(A.Y, Math.Min(B.Y, C.Y)), maxY = Math.Max(A.Y, Math.Max(B.Y, C.Y));
            int x0 = (int)Math.Floor(minX + cell / 2f), x1 = (int)Math.Ceiling(maxX + cell / 2f);
            int y0 = (int)Math.Floor(minY + cell / 2f), y1 = (int)Math.Ceiling(maxY + cell / 2f);
            float area = (B.X - A.X) * (C.Y - A.Y) - (C.X - A.X) * (B.Y - A.Y);
            if (Math.Abs(area) < 1e-9f) continue;
            for (int y = Math.Max(0, y0); y <= Math.Min(cell - 1, y1); y++)
                for (int x = Math.Max(0, x0); x <= Math.Min(cell - 1, x1); x++)
                {
                    float px = x - cell / 2f + 0.5f, py = y - cell / 2f + 0.5f;
                    float w0 = ((B.X - px) * (C.Y - py) - (C.X - px) * (B.Y - py)) / area;
                    float w1 = ((C.X - px) * (A.Y - py) - (A.X - px) * (C.Y - py)) / area;
                    float w2 = 1 - w0 - w1;
                    if (w0 < -0.001f || w1 < -0.001f || w2 < -0.001f) continue;
                    float z = w0 * A.Z + w1 * B.Z + w2 * C.Z;
                    int zi = y * cell + x;
                    if (z <= zbuf[zi]) continue;
                    zbuf[zi] = z;
                    int px2 = (ox + x) * 3, py2 = (oy + y) * W * 3;
                    byte shade = (byte)Math.Clamp(col * (0.55f + 0.45f * Math.Clamp(z / (cell * 0.5f), -1f, 1f) * 0.5f + 0.2f), 0, 255);
                    img[py2 + px2] = shade; img[py2 + px2 + 1] = (byte)Math.Clamp(shade * 0.9f, 0, 255); img[py2 + px2 + 2] = (byte)Math.Clamp(shade * 0.75f, 0, 255);
                }
        }
    }
}
