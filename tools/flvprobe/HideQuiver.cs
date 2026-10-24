using System;
using System.IO;

class HideQuiver
{
    public static void Run(string partsDir, string outDir, int[] ids, float scale, bool writeInPlace)
    {
        Directory.CreateDirectory(outDir);
        foreach (var id in ids)
        {
            foreach (var suffix in new[] { "", "_l" })
            {
                var name = "wp_a_" + id + suffix + ".partsbnd.dcx";
                var src = Path.Combine(partsDir, name);
                if (!File.Exists(src)) { Console.WriteLine("MISSING " + name); continue; }
                var dst = Path.Combine(outDir, name);
                PatchTool.Run(src, dst, scale);
            }
        }
    }
}
