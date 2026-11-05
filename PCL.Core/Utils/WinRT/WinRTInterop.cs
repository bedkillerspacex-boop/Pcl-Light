using System;
using System.Runtime.InteropServices;

namespace PCL.Core.Utils.WinRT;

public static partial class WinRTInterop
{
    // roapi.h
    [DllImport("combase.dll")]
    private static unsafe extern int RoGetActivationFactory(IntPtr activatableClassId, Guid* iid, IntPtr* factory);
    [DllImport("combase.dll")]
    private static unsafe extern int RoActivateInstance(IntPtr activatableClassId, IntPtr* instance);

    public static unsafe IntPtr ActivateInstance(ReadOnlySpan<char> activatableClassId)
    {
        var instance = IntPtr.Zero;

        fixed (char* pName = activatableClassId)
        {
            HStringHeader header;
            IntPtr hstring;

            Marshal.ThrowExceptionForHR(
                WindowsCreateStringReference(
                    (ushort*)pName,
                    activatableClassId.Length,
                    (IntPtr*)&header,
                    &hstring));

            Marshal.ThrowExceptionForHR(
                RoActivateInstance(
                    hstring,
                    &instance));
        }

        return instance;
    }

    public static unsafe IntPtr GetActivationFactory(ReadOnlySpan<char> activatableClassId, Guid iid)
    {
        var factory = IntPtr.Zero;

        fixed (char* pName = activatableClassId)
        {
            HStringHeader header;
            IntPtr hstring;

            Marshal.ThrowExceptionForHR(
                WindowsCreateStringReference(
                    (ushort*)pName,
                    activatableClassId.Length,
                    (IntPtr*)&header,
                    &hstring));

            Marshal.ThrowExceptionForHR(
                RoGetActivationFactory(
                    hstring,
                    &iid,
                    &factory));
        }

        return factory;
    }
    
    // winstring.h
    [DllImport("combase.dll")]
    internal static unsafe extern int WindowsCreateString(ushort* sourceString, int length, IntPtr* hstring);
    [DllImport("combase.dll")]
    internal static unsafe extern int WindowsCreateStringReference(ushort* sourceString, int length,
        IntPtr* hstringHeader, IntPtr* hstring);
    [DllImport("combase.dll")]
    internal static unsafe extern int WindowsDeleteString(IntPtr hstring);
    [DllImport("combase.dll")]
    internal static unsafe extern char* WindowsGetStringRawBuffer(IntPtr hstring, uint* length);
    
    // hstring.h
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct HStringHeader
    {
        private fixed byte _data[24];
    }
}
