using System.IO;
using System.IO.Compression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PCL.Core.IO;

namespace PCL.Core.Test.IO;

[TestClass]
public class ZipEncryptionDetectorTest
{
    [TestMethod]
    public void UnencryptedArchiveIsNotMarkedEncryptedAndStreamPositionIsRestored()
    {
        var bytes = CreateArchive();
        using var stream = new MemoryStream(bytes);
        stream.Position = 3;

        Assert.IsFalse(ZipEncryptionDetector.HasEncryptedEntries(stream));
        Assert.AreEqual(3L, stream.Position);
    }

    [TestMethod]
    public void EncryptedGeneralPurposeFlagIsDetected()
    {
        var bytes = CreateArchive();
        var centralHeader = FindCentralDirectoryHeader(bytes);
        bytes[centralHeader + 8] |= 1;

        using var stream = new MemoryStream(bytes);
        Assert.IsTrue(ZipEncryptionDetector.HasEncryptedEntries(stream));
    }

    [TestMethod]
    public void UninspectableStreamDoesNotRejectArchive()
    {
        using var stream = new MemoryStream(new byte[] { 0x50, 0x4b, 0x03, 0x04 });

        Assert.IsFalse(ZipEncryptionDetector.HasEncryptedEntries(stream));
    }

    private static byte[] CreateArchive()
    {
        using var stream = new MemoryStream();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        using (var writer = new StreamWriter(archive.CreateEntry("manifest.json").Open()))
            writer.Write("{}");
        return stream.ToArray();
    }

    private static int FindCentralDirectoryHeader(byte[] bytes)
    {
        for (var i = 0; i <= bytes.Length - 4; i++)
        {
            if (bytes[i] == 0x50 && bytes[i + 1] == 0x4b && bytes[i + 2] == 0x01 && bytes[i + 3] == 0x02)
                return i;
        }

        Assert.Fail("The test archive did not contain a central directory header.");
        return -1;
    }
}
