using System;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PCL.Core.Test.Minecraft;

[TestClass]
public sealed class LibraryMetadataTest
{
    [TestMethod]
    public void NativeRulesUseSelectedJavaArchitecture()
    {
        var x64Rule = JsonNode.Parse("""
            [{"action":"allow","os":{"name":"windows","arch":"x86_64|amd64"}}]
            """);
        var x86Rule = JsonNode.Parse("""
            [{"action":"allow","os":{"name":"windows","arch":"x86"}}]
            """);

        Assert.IsTrue(PCL.ModLibrary.McJsonRuleCheck(x64Rule, is64BitJava: true));
        Assert.IsFalse(PCL.ModLibrary.McJsonRuleCheck(x64Rule, is64BitJava: false));
        Assert.IsTrue(PCL.ModLibrary.McJsonRuleCheck(x86Rule, is64BitJava: false));
        Assert.IsFalse(PCL.ModLibrary.McJsonRuleCheck(x86Rule, is64BitJava: true));
    }

    [TestMethod]
    public void NativeLibraryKeepsArtifactAndSelectsJavaArchitectureClassifier()
    {
        const string metadata = """
            {
              "libraries": [{
                "name": "fixture:main:1.0",
                "natives": { "windows": "natives-windows-${arch}" },
                "downloads": {
                  "artifact": {
                    "path": "fixture/main/1.0/main.jar",
                    "url": "https://example.test/main.jar",
                    "size": 10,
                    "sha1": "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
                  },
                  "classifiers": {
                    "natives-windows-32": {
                      "path": "fixture/main/1.0/main-natives-windows-32.jar",
                      "url": "https://example.test/native-32.jar",
                      "size": 20,
                      "sha1": "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"
                    },
                    "natives-windows-64": {
                      "path": "fixture/main/1.0/main-natives-windows-64.jar",
                      "url": "https://example.test/native-64.jar",
                      "size": 30,
                      "sha1": "cccccccccccccccccccccccccccccccccccccccc"
                    }
                  }
                },
                "extract": { "exclude": ["META-INF/"] }
              }]
            }
            """;

        var version = JsonNode.Parse(metadata)!.AsObject();
        var libraryRoot = Path.Combine(Path.GetTempPath(), "pcl-light-library-metadata");

        var x64 = PCL.ModLibrary.McLibListGetWithJson(version, customMcFolder: libraryRoot,
            is64BitJava: true);
        Assert.AreEqual(2, x64.Count);
        var artifact = x64.Single(file => file.LocalPath.EndsWith(
            Path.Combine("fixture", "main", "1.0", "main.jar"), StringComparison.OrdinalIgnoreCase));
        var x64Native = x64.Single(file => file.IsNatives && !file.IsClasspath);
        Assert.IsFalse(artifact.IsNatives);
        Assert.IsTrue(artifact.IsClasspath);
        Assert.AreEqual("https://example.test/main.jar", artifact.Url);
        Assert.IsTrue(x64Native.LocalPath.EndsWith(
            Path.Combine("fixture", "main", "1.0", "main-natives-windows-64.jar"),
            StringComparison.OrdinalIgnoreCase));
        Assert.AreEqual("https://example.test/native-64.jar", x64Native.Url);
        Assert.IsFalse(x64Native.IsClasspath);
        CollectionAssert.AreEqual(new[] { "META-INF/" }, x64Native.ExtractExcludes.ToArray());

        var x86 = PCL.ModLibrary.McLibListGetWithJson(version, customMcFolder: libraryRoot,
            is64BitJava: false);
        Assert.AreEqual(2, x86.Count);
        var x86Native = x86.Single(file => file.IsNatives && !file.IsClasspath);
        Assert.IsTrue(x86Native.LocalPath.EndsWith(
            Path.Combine("fixture", "main", "1.0", "main-natives-windows-32.jar"),
            StringComparison.OrdinalIgnoreCase));
        Assert.AreEqual("https://example.test/native-32.jar", x86Native.Url);
    }

    [TestMethod]
    public void NativeArtifactWithoutClassifierIsExtractedAndExcludedFromClasspath()
    {
        const string metadata = """
            {
              "libraries": [{
                "name": "fixture:bundled:1.0",
                "natives": { "windows": "natives-windows-${arch}" },
                "downloads": {
                  "artifact": {
                    "path": "fixture/bundled/1.0/bundled-natives-windows.jar",
                    "url": "https://example.test/bundled-native.jar"
                  }
                },
                "extract": { "exclude": ["META-INF/"] }
              }]
            }
            """;

        var version = JsonNode.Parse(metadata)!.AsObject();
        var libraries = PCL.ModLibrary.McLibListGetWithJson(version,
            customMcFolder: Path.Combine(Path.GetTempPath(), "pcl-light-library-metadata"),
            is64BitJava: true);

        Assert.AreEqual(1, libraries.Count);
        Assert.IsTrue(libraries[0].IsNatives);
        Assert.IsFalse(libraries[0].IsClasspath);
        Assert.AreEqual("https://example.test/bundled-native.jar", libraries[0].Url);
        CollectionAssert.AreEqual(new[] { "META-INF/" }, libraries[0].ExtractExcludes.ToArray());
    }
}
