using System;

namespace PlainToolkit.CngProtectedData;

[Flags]
internal enum ProtectedFlags : byte
{
    None = 0,
    HasEntropy = 1 << 0
}