using System;
using System.Runtime.InteropServices;

namespace ProcessReader
{
    /// <summary>
    /// Contains the imports to the WinAPI functions
    /// </summary>
    public static class Imports
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess,
          IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        public const int PROCESS_WM_READ = 0x0010;
    }
}
