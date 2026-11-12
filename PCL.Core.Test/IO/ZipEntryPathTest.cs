using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PCL.Core.IO;

namespace PCL.Core.Test.IO;

[TestClass]
public sealed class ZipEntryPathTest
{
    [TestMethod]
    public void AcceptsNormalNestedPathsAndRejectsTraversal()
    {
        var root = Path.Combine(Path.GetTempPath(), "pcl-light-zip-root");

        Assert.IsTrue(ZipEntryPath.TryGetSafeDestinationPath(root, "lwjgl/native.dll", out var nested));
        Assert.AreEqual(Path.Combine(root, "lwjgl", "native.dll"), nested);
        Assert.IsTrue(ZipEntryPath.TryGetSafeDestinationPath(root, "lwjgl\\native.dll", out _));

        Assert.IsFalse(ZipEntryPath.TryGetSafeDestinationPath(root, "../outside.dll", out _));
        Assert.IsFalse(ZipEntryPath.TryGetSafeDestinationPath(root, "lwjgl/../../outside.dll", out _));
        Assert.IsFalse(ZipEntryPath.TryGetSafeDestinationPath(root, @"..\..\outside.dll", out _));
        Assert.IsFalse(ZipEntryPath.TryGetSafeDestinationPath(root, "/outside.dll", out _));
        Assert.IsFalse(ZipEntryPath.TryGetSafeDestinationPath(root, @"C:\outside.dll", out _));
    }

    [TestMethod]
    public void ExclusionsMatchDirectoryPrefixWithoutMatchingSiblingNames()
    {
        var exclusions = new[] { "META-INF/", "org/example/private.dll" };

        Assert.IsTrue(ZipEntryPath.IsExcluded("META-INF/MANIFEST.MF", exclusions));
        Assert.IsTrue(ZipEntryPath.IsExcluded("META-INF", exclusions));
        Assert.IsTrue(ZipEntryPath.IsExcluded("org/example/private.dll", exclusions));
        Assert.IsFalse(ZipEntryPath.IsExcluded("META-INFX/native.dll", exclusions));
        Assert.IsFalse(ZipEntryPath.IsExcluded("org/example/private.dll.extra", exclusions));
        Assert.IsFalse(ZipEntryPath.IsExcluded("../META-INF/native.dll", exclusions));
    }
}
