// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
//
// 代码修改自 System.Security.Cryptography.ProtectedData
// 源代码：https://github.com/dotnet/runtime/blob/main/src/libraries/System.Security.Cryptography.ProtectedData/src/System/Security/Cryptography/ProtectedData.cs

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using PlainToolkit.CngProtectedData.Native;

namespace PlainToolkit.CngProtectedData;

public static class CngProtectedData
{
    private static readonly byte[] s_nonEmpty = new byte[1];
    private static readonly byte Version = 1;

    public static byte[] Protect(byte[] userData, byte[]? optionalEntropy, DataProtectionScope scope)
    {
        CheckPlatformSupport();

        ArgumentNullException.ThrowIfNull(userData);

        byte[]? outputData;
        bool result = TryProtectOrUnprotect(
            userData,
            optionalEntropy,
            scope,
            protect: true,
            allocateArray: true,
            bytesWritten: out _,
            outputData: out outputData);
        Debug.Assert(result);
        Debug.Assert(outputData != null);
        return outputData;
    }

#if NET
    /// <summary>
    /// Encrypts the data in a specified byte span and returns a byte array that contains the encrypted data.
    /// </summary>
    /// <param name="userData">A buffer that contains the data to encrypt.</param>
    /// <param name="scope">One of the enumeration values that specifies the scope of encryption.</param>
    /// <param name="optionalEntropy">
    /// An optional additional byte span used to increase the complexity of the encryption,
    /// or empty for no additional complexity.
    /// </param>
    /// <returns>A byte array representing the encrypted data.</returns>
    /// <exception cref="CryptographicException">The encryption failed.</exception>
    /// <exception cref="NotSupportedException">The operating system does not support this method.</exception>
    /// <exception cref="OutOfMemoryException">The system ran out of memory while encrypting the data.</exception>
    /// <exception cref="PlatformNotSupportedException">
    /// Calls to the Protect method are supported on Windows operating systems only.
    /// </exception>
    public static byte[] Protect(
        ReadOnlySpan<byte> userData,
        DataProtectionScope scope,
        ReadOnlySpan<byte> optionalEntropy = default)
    {
        CheckPlatformSupport();

        byte[]? outputData;
        bool result = TryProtectOrUnprotect(
            userData,
            optionalEntropy,
            scope,
            protect: true,
            allocateArray: true,
            bytesWritten: out _,
            outputData: out outputData);
        Debug.Assert(result);
        Debug.Assert(outputData != null);
        return outputData;
    }

    /// <summary>
    /// Encrypts the data in a specified buffer and writes the encrypted data to a destination buffer.
    /// </summary>
    /// <param name="userData">A buffer that contains data to encrypt.</param>
    /// <param name="scope">One of the enumeration values that specifies the scope of encryption.</param>
    /// <param name="destination">The buffer to receive the encrypted data.</param>
    /// <param name="bytesWritten">
    /// When this method returns, contains the number of bytes
    /// written to <paramref name="destination"/>.
    /// </param>
    /// <param name="optionalEntropy">
    /// An optional additional buffer used to increase the complexity of the encryption,
    /// or empty for no additional complexity.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="destination"/> was large enough to receive the decrypted data;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// The buffer in <paramref name="destination"/> is too small to hold the encrypted data.
    /// </exception>
    /// <exception cref="CryptographicException">The encryption failed.</exception>
    /// <exception cref="NotSupportedException">The operating system does not support this method.</exception>
    /// <exception cref="OutOfMemoryException">The system ran out of memory while encrypting the data.</exception>
    /// <exception cref="PlatformNotSupportedException">
    /// Calls to the Protect method are supported on Windows operating systems only.
    /// </exception>
    public static bool TryProtect(
        ReadOnlySpan<byte> userData,
        DataProtectionScope scope,
        Span<byte> destination,
        out int bytesWritten,
        ReadOnlySpan<byte> optionalEntropy = default)
    {
        CheckPlatformSupport();

        return TryProtectOrUnprotect(
            userData,
            optionalEntropy,
            scope,
            protect: true,
            allocateArray: false,
            outputSpan: destination,
            bytesWritten: out bytesWritten,
            outputData: out _);
    }

    /// <summary>
    /// Encrypts the data in a specified buffer and writes the encrypted data to a destination buffer.
    /// </summary>
    /// <param name="userData">A buffer that contains data to encrypt.</param>
    /// <param name="scope">One of the enumeration values that specifies the scope of encryption.</param>
    /// <param name="destination">The buffer to receive the encrypted data.</param>
    /// <param name="optionalEntropy">
    /// An optional additional buffer used to increase the complexity of the encryption,
    /// or empty for no additional complexity.
    /// </param>
    /// <returns>The total number of bytes written to <paramref name="destination"/></returns>
    /// <exception cref="ArgumentException">
    /// The buffer in <paramref name="destination"/> is too small to hold the encrypted data.
    /// </exception>
    /// <exception cref="CryptographicException">The encryption failed.</exception>
    /// <exception cref="NotSupportedException">The operating system does not support this method.</exception>
    /// <exception cref="OutOfMemoryException">The system ran out of memory while encrypting the data.</exception>
    /// <exception cref="PlatformNotSupportedException">
    /// Calls to the Protect method are supported on Windows operating systems only.
    /// </exception>
    public static int Protect(
        ReadOnlySpan<byte> userData,
        DataProtectionScope scope,
        Span<byte> destination,
        ReadOnlySpan<byte> optionalEntropy = default)
    {
        CheckPlatformSupport();

        int bytesWritten;
        if (!TryProtectOrUnprotect(
                userData,
                optionalEntropy,
                scope,
                protect: true,
                allocateArray: false,
                outputSpan: destination,
                bytesWritten: out bytesWritten,
                outputData: out _))
        {
            throw new ArgumentException("Destination is too short.", nameof(destination));
        }

        return bytesWritten;
    }
#endif

    public static byte[] Unprotect(byte[] encryptedData, byte[]? optionalEntropy, DataProtectionScope scope)
    {
        CheckPlatformSupport();

        ArgumentNullException.ThrowIfNull(encryptedData);

        byte[]? outputData;
        bool result = TryProtectOrUnprotect(
            encryptedData,
            optionalEntropy,
            scope,
            protect: false,
            allocateArray: true,
            bytesWritten: out _,
            outputData: out outputData);

        Debug.Assert(result);
        Debug.Assert(outputData != null);
        return outputData;
    }

#if NET
    /// <summary>
    /// Decrypts the data in a specified byte array and returns a byte array that contains the decrypted data.
    /// </summary>
    /// <param name="encryptedData">A buffer that contains data to decrypt.</param>
    /// <param name="scope">One of the enumeration values that specifies the scope of encryption.</param>
    /// <param name="optionalEntropy">
    /// An optional additional buffer used to increase the complexity of the encryption,
    /// or empty for no additional complexity.
    /// </param>
    /// <returns>A byte array representing the encrypted data.</returns>
    /// <exception cref="CryptographicException">The encryption failed.</exception>
    /// <exception cref="NotSupportedException">The operating system does not support this method.</exception>
    /// <exception cref="OutOfMemoryException">The system ran out of memory while decrypting the data.</exception>
    /// <exception cref="PlatformNotSupportedException">
    /// Calls to the Unprotect method are supported on Windows operating systems only.
    /// </exception>
    public static byte[] Unprotect(
        ReadOnlySpan<byte> encryptedData,
        DataProtectionScope scope,
        ReadOnlySpan<byte> optionalEntropy = default)
    {
        CheckPlatformSupport();

        byte[]? outputData;
        bool result = TryProtectOrUnprotect(
            encryptedData,
            optionalEntropy,
            scope,
            protect: false,
            allocateArray: true,
            bytesWritten: out _,
            outputData: out outputData);

        Debug.Assert(result);
        Debug.Assert(outputData != null);
        return outputData;
    }

    /// <summary>
    /// Decrypts the data in a specified buffer and writes the decrypted data to a destination buffer.
    /// </summary>
    /// <param name="encryptedData">A buffer that contains data to decrypt.</param>
    /// <param name="scope">One of the enumeration values that specifies the scope of encryption.</param>
    /// <param name="destination">The buffer to receive the decrypted data.</param>
    /// <param name="bytesWritten">
    /// When this method returns, contains the number of bytes
    /// written to <paramref name="destination"/>.
    /// </param>
    /// <param name="optionalEntropy">
    /// An optional additional buffer used to increase the complexity of the encryption,
    /// or empty for no additional complexity.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="destination"/> was large enough to receive the decrypted data;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// The buffer in <paramref name="destination"/> is too small to hold the decrypted data.
    /// </exception>
    /// <exception cref="CryptographicException">The encryption failed.</exception>
    /// <exception cref="NotSupportedException">The operating system does not support this method.</exception>
    /// <exception cref="OutOfMemoryException">The system ran out of memory while encrypting the data.</exception>
    /// <exception cref="PlatformNotSupportedException">
    /// Calls to the Unprotect method are supported on Windows operating systems only.
    /// </exception>
    public static bool TryUnprotect(
        ReadOnlySpan<byte> encryptedData,
        DataProtectionScope scope,
        Span<byte> destination,
        out int bytesWritten,
        ReadOnlySpan<byte> optionalEntropy = default)
    {
        CheckPlatformSupport();

        return TryProtectOrUnprotect(
            encryptedData,
            optionalEntropy,
            scope,
            protect: false,
            allocateArray: false,
            outputSpan: destination,
            bytesWritten: out bytesWritten,
            outputData: out _);
    }

    /// <summary>
    /// Decrypts the data in a specified buffer and writes the decrypted data to a destination buffer.
    /// </summary>
    /// <param name="encryptedData">A buffer that contains data to decrypt.</param>
    /// <param name="scope">One of the enumeration values that specifies the scope of encryption.</param>
    /// <param name="destination">The buffer to receive the decrypted data.</param>
    /// <param name="optionalEntropy">
    /// An optional additional buffer used to increase the complexity of the encryption,
    /// or empty for no additional complexity.
    /// </param>
    /// <returns>The total number of bytes written to <paramref name="destination"/></returns>
    /// <exception cref="ArgumentException">
    /// The buffer in <paramref name="destination"/> is too small to hold the decrypted data.
    /// </exception>
    /// <exception cref="CryptographicException">The encryption failed.</exception>
    /// <exception cref="NotSupportedException">The operating system does not support this method.</exception>
    /// <exception cref="OutOfMemoryException">The system ran out of memory while encrypting the data.</exception>
    /// <exception cref="PlatformNotSupportedException">
    /// Calls to the Unprotect method are supported on Windows operating systems only.
    /// </exception>
    public static int Unprotect(
        ReadOnlySpan<byte> encryptedData,
        DataProtectionScope scope,
        Span<byte> destination,
        ReadOnlySpan<byte> optionalEntropy = default)
    {
        CheckPlatformSupport();

        int bytesWritten;
        if (!TryProtectOrUnprotect(
                encryptedData,
                optionalEntropy,
                scope,
                protect: false,
                allocateArray: false,
                outputSpan: destination,
                bytesWritten: out bytesWritten,
                outputData: out _))
        {
            throw new ArgumentException("Destination is too short.", nameof(destination));
        }

        return bytesWritten;
    }
#endif

    private static bool TryProtectOrUnprotect(
        ReadOnlySpan<byte> inputData,
        ReadOnlySpan<byte> optionalEntropy,
        DataProtectionScope scope,
        bool protect,
        out int bytesWritten,
        out byte[]? outputData,
        bool allocateArray,
        Span<byte> outputSpan = default)
    {
        var descriptorRule = scope switch
        {
            DataProtectionScope.CurrentUser => "LOCAL=user",
            DataProtectionScope.LocalMachine => "LOCAL=machine",
            _ => throw new ArgumentException("Invalid DataProtectionScope", nameof(scope))
        };
        
        unsafe
        {
            var relevantData = inputData.IsEmpty ? s_nonEmpty : inputData;
            var processedData = protect ? Pack(relevantData, optionalEntropy) : relevantData;

            fixed (byte* pInputData = processedData)
            {
                int hresult;
                var hDescriptor = IntPtr.Zero;
                var pbOutput = IntPtr.Zero;
                uint cbOutput;
                
                try
                {
                    if (protect)
                    {
                        // 创建保护描述符
                        hresult = NCryptProtect.NCryptCreateProtectionDescriptor(descriptorRule, 0, out hDescriptor);
                        if (hresult != 0)
                        {
                            throw new CryptographicException($"NCryptCreateProtectionDescriptor failed: {hresult:X}");
                        }
                        
                        // Protect
                        hresult = NCryptProtect.NCryptProtectSecret(
                            hDescriptor,
                            0,
                            (IntPtr)pInputData,
                            (uint)processedData.Length,
                            IntPtr.Zero,
                            IntPtr.Zero,
                            out pbOutput,
                            out cbOutput);
                    }
                    else
                    {
                        // Unprotect
                        hresult = NCryptProtect.NCryptUnprotectSecret(
                            out hDescriptor,
                            0,
                            (IntPtr)pInputData,
                            (uint)processedData.Length,
                            IntPtr.Zero,
                            IntPtr.Zero,
                            out pbOutput,
                            out cbOutput);
                    }

                    if (hresult != 0)
                    {
                        throw new CryptographicException($"{(protect ? "Protect" : "Unprotect")} failed: {hresult:X}");
                    }
                    
                    var length = (int)cbOutput;

                    
                    
                    // 分配新数组
                    if (allocateArray)
                    {
                        if (protect)
                        {
                            outputData = new byte[length];
                            Marshal.Copy(pbOutput, outputData, 0, length);
                            bytesWritten = length;
                        }
                        else
                        {
                            outputData = Unpack(new ReadOnlySpan<byte>((byte*)pbOutput, length), optionalEntropy);
                            bytesWritten = outputData.Length;
                        }
                       
                        return true;
                    }
                    
                    // 如果不分配新数组，则写入目标缓冲区
                    outputData = null;

                    if (protect)
                    {
                        // 判断输出缓冲区是否足够大
                        if (length > outputSpan.Length)
                        {
                            bytesWritten = 0;
                            return false;
                        }

                        // 从内存拷贝到目标缓冲区
                        new ReadOnlySpan<byte>((void*)pbOutput, length).CopyTo(outputSpan);
                        bytesWritten = length;
                    }
                    else
                    {
                        var unpacked = Unpack(
                            new ReadOnlySpan<byte>((void*)pbOutput, length),
                            optionalEntropy);

                        // 判断输出缓冲区是否足够大
                        if (unpacked.Length > outputSpan.Length)
                        {
                            bytesWritten = 0;
                            return false;
                        }

                        unpacked.CopyTo(outputSpan);
                        bytesWritten = unpacked.Length;
                    }
                    
                    return true;
                }
                finally
                {
                    // 清理资源
                    if (pbOutput != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(pbOutput);
                    }
                    
                    // 关闭保护描述符
                    if (hDescriptor != IntPtr.Zero)
                    {
                        hresult = NCryptProtect.NCryptCloseProtectionDescriptor(hDescriptor);
                        if (hresult != 0)
                        {
                            Debug.Fail($"NCryptCloseProtectionDescriptor failed: {hresult:X}");
                        }
                    }
                }
            }
        }
    }

    private static void CheckPlatformSupport()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new PlatformNotSupportedException();
        }
    }

    // PlainToolkit.CngProtectedData 的自定义包装数据如下：
    // 无可选熵：
    // [header(8 byte)][data(? byte)]
    // 有可选熵（使用 AES-GCM 实现可选熵）：
    // [header(8 byte)][nonce(12 byte)][ciphertext(? byte)][tag(16 byte)]
    
    private static byte[] Pack(ReadOnlySpan<byte> data, ReadOnlySpan<byte> optionalEntropy = default)
    {
        var flags = optionalEntropy.IsEmpty ? ProtectedFlags.None : ProtectedFlags.HasEntropy;
        
        var header = new ProtectedHeader
        {
            Magic = ProtectedHeader.MagicValue,
            Version = Version,
            Flags = (byte)flags,
            Reserved = 0
        };

        byte[] buffer;

        // 判断是否有可选熵
        if ((flags & ProtectedFlags.HasEntropy) != 0)
        {
            // 由于 CNG DPAPI 并不支持可选熵，因此使用自定义的 Header 与 AES-GCM 模拟可选熵，最终在调用端无感知
            
            // 初始化缓冲区
            buffer = new byte[8 + 12 + data.Length + 16]; // header + nonce + ciphertext + tag
            
            // 将 header 写入缓冲区
            MemoryMarshal.Write(buffer, ref header);
            
            // 根据可选熵生成 Key
            Span<byte> key = stackalloc byte[32];
            SHA256.HashData(optionalEntropy, key);
            
            // 随机生成 Nonce
            var nonce = buffer.AsSpan(8, 12);
            RandomNumberGenerator.Fill(nonce);
            
            // 将 buffer 对应位置转为 Span
            var cipher = buffer.AsSpan(20, data.Length);
            var tag = buffer.AsSpan(20 + data.Length, 16);
            
            using var aes = new AesGcm(key);
            aes.Encrypt(nonce, data, cipher, tag, buffer.AsSpan(0, 8));
            
            // 清空 Key 内存
            CryptographicOperations.ZeroMemory(key);
        }
        else
        {
            // 无可选熵则直接写入 header 与 data
            buffer = new byte[8 + data.Length]; // header + data
            MemoryMarshal.Write(buffer, ref header);
            data.CopyTo(buffer.AsSpan(8));
        }
        
        return buffer;
    }

    private static byte[] Unpack(ReadOnlySpan<byte> data, ReadOnlySpan<byte> optionalEntropy = default)
    {
        if (data.Length < 8)
        {
            // 数据可能是 1.x 的 CngProtectedData 生成，完全无 Pack，直接返回
            return data.ToArray();
        }
        
        var header = MemoryMarshal.Read<ProtectedHeader>(data);
        
        if (header.Magic != ProtectedHeader.MagicValue)
        {
            // 数据可能是 1.x 的 CngProtectedData 生成，完全无 Pack，直接返回
            return data.ToArray();
        }

        byte[] buffer;
        
        if ((header.Flags & (byte)ProtectedFlags.HasEntropy) != 0)
        {
            // 有可选熵，解密

            // 验证可选熵是否为空
            if (optionalEntropy.IsEmpty)
            {
                throw new ArgumentException("Optional entropy is required.", nameof(optionalEntropy));
            }
            
            // 提取 Nonce, Ciphertext, Tag
            var nonce = data.Slice(8, 12);
            var cipher = data.Slice(20, data.Length - 36);
            var tag = data[^16..];
            
            // 初始化缓冲区
            buffer = new byte[cipher.Length];
            
            // 根据可选熵生成 Key
            Span<byte> key = stackalloc byte[32];
            SHA256.HashData(optionalEntropy, key);
            
            using var aes = new AesGcm(key);
            
            aes.Decrypt(nonce, cipher, tag, buffer, data[..8]);
            
            // 清空 Key 内存
            CryptographicOperations.ZeroMemory(key);
        }
        else
        {
            // 处理无可选熵的情况
            buffer = new byte[data.Length - 8];
            data[8..].CopyTo(buffer);
        }
        
        return buffer;
    }
}
