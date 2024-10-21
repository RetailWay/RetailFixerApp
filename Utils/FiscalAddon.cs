using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace RetailFixer.Utils;

public static class FiscalAddon
{
    
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr LoadLibrary(string lpPathName);

    public static bool LoadLib(string pathLib) => LoadLibrary(pathLib) == 0;
    
    public static bool TryGetPathDriver(string pathRegistry, out string path)
    {
        path = "";
        try
        {
#pragma warning disable CA1416
            using var reg = Registry.LocalMachine.OpenSubKey(pathRegistry);
            if (reg?.GetValue("INSTALL_DIR") is not string installDir) return false;
            path = Path.Combine(installDir, "bin");
            return true;
#pragma warning restore CA1416
        }
        catch
        {
            return false;
        }
    }
}