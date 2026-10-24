using System;
using System.IO;
using System.Linq;
using SoulsFormats;

class FmgTool
{
    public static void Run(string path, string needle)
    {
        var raw = File.ReadAllBytes(path);
        if (DCX.Is(raw)) raw = DCX.Decompress(raw);
        var fmg = FMG.Read(raw);
        Console.WriteLine("entries: " + fmg.Entries.Count);
        foreach (var e in fmg.Entries)
        {
            if (needle == "*" || (e.Text ?? "").IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0)
                Console.WriteLine(e.ID + "\t" + e.Text);
        }
    }
}
