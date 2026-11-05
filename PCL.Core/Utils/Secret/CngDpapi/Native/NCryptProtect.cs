using System;
using System.Runtime.InteropServices;

namespace PlainToolkit.CngProtectedData.Native;

// NCrypt DPAPI imports adapted from PlainToolkit.CngProtectedData 2.1.0.
internal static class NCryptProtect
{
    [DllImport("ncrypt.dll", CharSet = CharSet.Unicode)]
    internal static extern int NCryptCreateProtectionDescriptor(
        string? pwszDescriptorString,
        int dwFlags,
        out IntPtr phDescriptor);

    [DllImport("ncrypt.dll")]
    internal static extern int NCryptCloseProtectionDescriptor(IntPtr hDescriptor);

    [DllImport("ncrypt.dll")]
    internal static extern int NCryptProtectSecret(
        IntPtr hDescriptor,
        int dwFlags,
        IntPtr pbData,
        uint cbData,
        IntPtr pMemPara,
        IntPtr hWnd,
        out IntPtr ppbProtectedBlob,
        out uint pcbProtectedBlob);

    [DllImport("ncrypt.dll")]
    internal static extern int NCryptUnprotectSecret(
        out IntPtr phDescriptor,
        int dwFlags,
        IntPtr pbProtectedBlob,
        uint cbProtectedBlob,
        IntPtr pMemPara,
        IntPtr hWnd,
        out IntPtr ppbData,
        out uint pcbData);
}
