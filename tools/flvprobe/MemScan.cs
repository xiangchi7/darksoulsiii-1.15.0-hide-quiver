using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

class MemScan
{
    [DllImport("kernel32.dll")] static extern IntPtr OpenProcess(int access, bool inherit, int pid);
    [DllImport("kernel32.dll")] static extern bool ReadProcessMemory(IntPtr h, IntPtr addr, byte[] buf, int size, out IntPtr read);
    [DllImport("kernel32.dll")] static extern int VirtualQueryEx(IntPtr h, IntPtr addr, out MEMORY_BASIC_INFORMATION64 mbi, IntPtr len);
    [DllImport("kernel32.dll")] static extern bool CloseHandle(IntPtr h);

    [StructLayout(LayoutKind.Sequential)]
    struct MEMORY_BASIC_INFORMATION64
    {
        public ulong BaseAddress, AllocationBase, AllocationProtect;
        public ulong RegionSize;
        public uint State, Protect, Type;
    }
    const int PROCESS_VM_READ = 0x10, PROCESS_QUERY_INFORMATION = 0x400;
    const uint MEM_COMMIT = 0x1000;

    static bool Readable(uint protect)
    {
        int p = (int)(protect & 0xff);
        if ((protect & 0x100) != 0) return true; // PAGE_GUARD
        return p == 0x02 || p == 0x04 || p == 0x20 || p == 0x40 || p == 0x08 || p == 0x80;
    }

    public static void Run(string procName, string pattern)
    {
        var procs = Process.GetProcessesByName(procName);
        if (procs.Length == 0) { Console.WriteLine("not running: " + procName); return; }
        var proc = procs[0];
        var h = OpenProcess(PROCESS_VM_READ | PROCESS_QUERY_INFORMATION, false, proc.Id);
        if (h == IntPtr.Zero) { Console.WriteLine("OpenProcess failed"); return; }
        var counts = new Dictionary<string, int>();
        var rx = new Regex(pattern, RegexOptions.Compiled);
        ulong addr = 0; int regions = 0; long bytes = 0;
        var buf = new byte[4 << 20];
        while (true)
        {
            MEMORY_BASIC_INFORMATION64 mbi;
            if (VirtualQueryEx(h, (IntPtr)(long)addr, out mbi, (IntPtr)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION64))) == 0) break;
            ulong baseAddr = mbi.BaseAddress; ulong size = mbi.RegionSize;
            if (mbi.State == MEM_COMMIT && Readable(mbi.Protect) && size > 0 && size < (1UL << 33))
            {
                regions++;
                for (ulong off = 0; off < size; off += (ulong)buf.Length)
                {
                    int want = (int)Math.Min((ulong)buf.Length, size - off);
                    IntPtr read;
                    if (!ReadProcessMemory(h, (IntPtr)(long)(baseAddr + off), buf, want, out read)) break;
                    if ((long)read <= 0) break;
                    bytes += (long)read;
                    string s = Encoding.ASCII.GetString(buf, 0, (int)read);
                    foreach (Match m in rx.Matches(s))
                    {
                        string k = m.Value; counts[k] = counts.TryGetValue(k, out var c) ? c + 1 : 1;
                    }
                }
            }
            addr = baseAddr + size; if (addr <= baseAddr) break; if (size == 0) break;
        }
        CloseHandle(h);
        Console.WriteLine("regions=" + regions + " scannedMB=" + (bytes >> 20) + " distinct=" + counts.Count);
        var list = new List<KeyValuePair<string,int>>(counts);
        list.Sort((a,b) => string.Compare(a.Key, b.Key, StringComparison.Ordinal));
        foreach (var kv in list) Console.WriteLine(kv.Key + "\t" + kv.Value);
    }
}
