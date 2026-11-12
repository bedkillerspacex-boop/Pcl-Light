using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PCL.Core.Minecraft;

namespace PCL.Core.Test.Minecraft;

[TestClass]
public sealed class AssetIndexPathTest
{
    [TestMethod]
    public void ResolvesObjectVirtualAndLegacyResourceLayouts()
    {
        var assets = Path.Combine(Path.GetTempPath(), "pcl-light-assets");
        var resources = Path.Combine(Path.GetTempPath(), "pcl-light-instance", "resources");
        const string hash = "0123456789abcdef0123456789abcdef01234567";

        Assert.AreEqual(Path.Combine(assets, "objects", "01", hash),
            AssetIndexPath.ResolveLocalPath(assets, resources, "sound/test.ogg", hash, false, false));
        Assert.AreEqual(Path.Combine(assets, "virtual", "legacy", "sound", "test.ogg"),
            AssetIndexPath.ResolveLocalPath(assets, resources, "sound/test.ogg", hash, false, true));
        Assert.AreEqual(Path.Combine(resources, "sound", "test.ogg"),
            AssetIndexPath.ResolveLocalPath(assets, resources, "sound/test.ogg", hash, true, false));
    }

    [TestMethod]
    public void RejectsTraversalAndMalformedHashes()
    {
        const string hash = "0123456789abcdef0123456789abcdef01234567";

        Assert.ThrowsException<InvalidDataException>(() =>
            AssetIndexPath.ResolveLocalPath("assets", "resources", "../outside.dll", hash, true, false));
        Assert.ThrowsException<InvalidDataException>(() =>
            AssetIndexPath.ResolveLocalPath("assets", "resources", "sound.ogg", "../hash", false, false));
    }
}
