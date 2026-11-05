using System;
using System.Buffers.Binary;
using System.IO;

namespace PCL.Core.IO;

/// <summary>
/// Reads the encryption flag from a ZIP central directory without decoding entry names or
/// applying a second ZIP implementation's format restrictions.
/// </summary>
public static class ZipEncryptionDetector
{
    private const uint EndOfCentralDirectorySignature = 0x06054b50;
    private const uint Zip64EndOfCentralDirectorySignature = 0x06064b50;
    private const uint Zip64LocatorSignature = 0x07064b50;
    private const uint CentralDirectoryHeaderSignature = 0x02014b50;
    private const int EndOfCentralDirectoryLength = 22;
    private const int CentralDirectoryHeaderLength = 46;
    private const int MaxZip64RecordSearchLength = 1024 * 1024;

    /// <summary>
    /// Returns true if a central-directory entry has the ZIP encryption flag set.
    /// If the directory cannot be inspected, returns false so the caller's ZIP reader
    /// remains responsible for deciding whether the archive is valid.
    /// The input stream position is restored before returning.
    /// </summary>
    public static bool HasEncryptedEntries(Stream stream)
    {
        if (stream is null || !stream.CanRead || !stream.CanSeek) return false;

        long originalPosition;
        try
        {
            originalPosition = stream.Position;
        }
        catch (Exception ex) when (ex is IOException or NotSupportedException)
        {
            return false;
        }

        try
        {
            if (stream.Length < EndOfCentralDirectoryLength) return false;

            var tailLength = (int)Math.Min(stream.Length, EndOfCentralDirectoryLength + ushort.MaxValue);
            var tail = new byte[tailLength];
            stream.Position = stream.Length - tailLength;
            if (!_ReadExactly(stream, tail)) return false;

            var endRecordIndex = _FindEndOfCentralDirectory(tail);
            if (endRecordIndex < 0) return false;

            var endRecordOffset = stream.Length - tailLength + endRecordIndex;
            var endRecord = tail.AsSpan(endRecordIndex);
            var diskNumber = BinaryPrimitives.ReadUInt16LittleEndian(endRecord.Slice(4, 2));
            var directoryDisk = BinaryPrimitives.ReadUInt16LittleEndian(endRecord.Slice(6, 2));
            var entriesOnDisk = BinaryPrimitives.ReadUInt16LittleEndian(endRecord.Slice(8, 2));
            var totalEntries = BinaryPrimitives.ReadUInt16LittleEndian(endRecord.Slice(10, 2));
            ulong directorySize = BinaryPrimitives.ReadUInt32LittleEndian(endRecord.Slice(12, 4));
            var directoryOffset = BinaryPrimitives.ReadUInt32LittleEndian(endRecord.Slice(16, 4));

            long directoryEndOffset;
            ulong entryCount;
            if (diskNumber == ushort.MaxValue || directoryDisk == ushort.MaxValue ||
                entriesOnDisk == ushort.MaxValue || totalEntries == ushort.MaxValue ||
                directorySize == uint.MaxValue || directoryOffset == uint.MaxValue)
            {
                if (!_TryReadZip64DirectoryInfo(stream, endRecordOffset,
                        out directoryEndOffset, out directorySize, out entryCount))
                    return false;
            }
            else
            {
                if (diskNumber != 0 || directoryDisk != 0 || entriesOnDisk != totalEntries)
                    return false;
                directoryEndOffset = endRecordOffset;
                entryCount = totalEntries;
            }

            if (directorySize > long.MaxValue) return false;
            var directoryStartOffset = directoryEndOffset - (long)directorySize;
            if (directoryStartOffset < 0 || directoryStartOffset > stream.Length) return false;

            return _ScanCentralDirectory(stream, directoryStartOffset, directoryEndOffset, entryCount);
        }
        catch (Exception ex) when (ex is IOException or NotSupportedException or ArgumentException or OverflowException)
        {
            return false;
        }
        finally
        {
            try { stream.Position = originalPosition; } catch (Exception ex) when (ex is IOException or NotSupportedException) { }
        }
    }

    private static int _FindEndOfCentralDirectory(byte[] tail)
    {
        for (var index = tail.Length - EndOfCentralDirectoryLength; index >= 0; index--)
        {
            if (BinaryPrimitives.ReadUInt32LittleEndian(tail.AsSpan(index, 4)) != EndOfCentralDirectorySignature)
                continue;
            var commentLength = BinaryPrimitives.ReadUInt16LittleEndian(tail.AsSpan(index + 20, 2));
            if (index + EndOfCentralDirectoryLength + commentLength == tail.Length)
                return index;
        }

        return -1;
    }

    private static bool _TryReadZip64DirectoryInfo(Stream stream, long endRecordOffset,
        out long directoryEndOffset, out ulong directorySize, out ulong entryCount)
    {
        directoryEndOffset = 0;
        directorySize = 0;
        entryCount = 0;

        var locatorOffset = endRecordOffset - 20;
        if (locatorOffset < 0) return false;
        stream.Position = locatorOffset;
        Span<byte> locator = stackalloc byte[20];
        if (!_ReadExactly(stream, locator) ||
            BinaryPrimitives.ReadUInt32LittleEndian(locator.Slice(0, 4)) != Zip64LocatorSignature ||
            BinaryPrimitives.ReadUInt32LittleEndian(locator.Slice(4, 4)) != 0 ||
            BinaryPrimitives.ReadUInt32LittleEndian(locator.Slice(16, 4)) != 1)
            return false;

        var searchLength = (int)Math.Min(locatorOffset, MaxZip64RecordSearchLength);
        var recordTail = new byte[searchLength];
        stream.Position = locatorOffset - searchLength;
        if (!_ReadExactly(stream, recordTail)) return false;

        for (var index = recordTail.Length - 56; index >= 0; index--)
        {
            var header = recordTail.AsSpan(index);
            if (BinaryPrimitives.ReadUInt32LittleEndian(header.Slice(0, 4)) != Zip64EndOfCentralDirectorySignature)
                continue;
            var recordPayloadLength = BinaryPrimitives.ReadUInt64LittleEndian(header.Slice(4, 8));
            if (recordPayloadLength < 44 || (ulong)index + 12 + recordPayloadLength != (ulong)recordTail.Length)
                continue;

            if (BinaryPrimitives.ReadUInt32LittleEndian(header.Slice(16, 4)) != 0 ||
                BinaryPrimitives.ReadUInt32LittleEndian(header.Slice(20, 4)) != 0 ||
                BinaryPrimitives.ReadUInt64LittleEndian(header.Slice(24, 8)) !=
                BinaryPrimitives.ReadUInt64LittleEndian(header.Slice(32, 8)))
                return false;

            directorySize = BinaryPrimitives.ReadUInt64LittleEndian(header.Slice(40, 8));
            entryCount = BinaryPrimitives.ReadUInt64LittleEndian(header.Slice(32, 8));
            directoryEndOffset = locatorOffset - searchLength + index;
            return true;
        }

        return false;
    }

    private static bool _ScanCentralDirectory(Stream stream, long directoryStartOffset,
        long directoryEndOffset, ulong entryCount)
    {
        if (directoryEndOffset < directoryStartOffset ||
            entryCount > (ulong)((directoryEndOffset - directoryStartOffset) / CentralDirectoryHeaderLength))
            return false;

        var position = directoryStartOffset;
        Span<byte> header = stackalloc byte[CentralDirectoryHeaderLength];
        for (ulong index = 0; index < entryCount; index++)
        {
            if (position > directoryEndOffset - CentralDirectoryHeaderLength) return false;
            stream.Position = position;
            if (!_ReadExactly(stream, header) ||
                BinaryPrimitives.ReadUInt32LittleEndian(header.Slice(0, 4)) != CentralDirectoryHeaderSignature)
                return false;

            var flags = BinaryPrimitives.ReadUInt16LittleEndian(header.Slice(8, 2));
            if ((flags & 1) != 0) return true;

            var nameLength = BinaryPrimitives.ReadUInt16LittleEndian(header.Slice(28, 2));
            var extraLength = BinaryPrimitives.ReadUInt16LittleEndian(header.Slice(30, 2));
            var commentLength = BinaryPrimitives.ReadUInt16LittleEndian(header.Slice(32, 2));
            position += CentralDirectoryHeaderLength + nameLength + extraLength + commentLength;
            if (position > directoryEndOffset) return false;
        }

        return false;
    }

    private static bool _ReadExactly(Stream stream, Span<byte> buffer)
    {
        var read = 0;
        while (read < buffer.Length)
        {
            var count = stream.Read(buffer[read..]);
            if (count == 0) return false;
            read += count;
        }

        return true;
    }
}
