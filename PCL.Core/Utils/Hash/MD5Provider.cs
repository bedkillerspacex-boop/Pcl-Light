using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PCL.Core.Utils.Hash;

public class MD5Provider : IHashProvider {
    public static MD5Provider Instance { get; } = new();
    public int Length => 32;

    public byte[] ComputeHash(ReadOnlySpan<byte> input)
    {
        return MD5.HashData(input);
    }

    public byte[] ComputeHash(string input, Encoding? en = null)
    {
        return ComputeHash(en is null ? Encoding.UTF8.GetBytes(input) : en.GetBytes(input));
    }

    public byte[] ComputeHash(Stream input)
    {
        using var algorithm = MD5.Create();
        return algorithm.ComputeHash(input);
    }

    public async ValueTask<byte[]> ComputeHashAsync(Stream input, CancellationToken cancellationToken = default)
    {
        using var algorithm = MD5.Create();
        return await algorithm.ComputeHashAsync(input, cancellationToken).ConfigureAwait(false);
    }
}
