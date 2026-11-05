using System.Runtime.InteropServices;

namespace PlainToolkit.CngProtectedData;

[StructLayout(LayoutKind.Sequential)]
internal struct ProtectedHeader
{
    public const uint MagicValue = 0x5f445043; // CPD_
    
    public uint Magic;
    public byte Version;
    public byte Flags;
    public ushort Reserved;
}