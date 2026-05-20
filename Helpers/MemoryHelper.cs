using System.Runtime.InteropServices;

namespace PrometheusAgent.Helpers;

public static class MemoryHelper {
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private sealed class MEMORYSTATUSEX {
        public uint dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));

        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

    public static (ulong Total, ulong Available) GetWindowsMemory() {
        var memoryStatus = new MEMORYSTATUSEX();

        if (!GlobalMemoryStatusEx(memoryStatus))
            throw new InvalidOperationException("Unable to read memory status.");

        return (
            memoryStatus.ullTotalPhys,
            memoryStatus.ullAvailPhys
        );
    }
}
