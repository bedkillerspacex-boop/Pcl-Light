using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PCL.Core.Minecraft.Java.UserPreference;
using PCL.Core.Utils;

namespace PCL.Core.Test.Minecraft;

[TestClass]
public class JavaPreferenceJsonConverterTest
{
    [TestMethod]
    public void ReadsDiscriminatorAfterDerivedProperties()
    {
        const string json = "{\"RelativePath\":\"runtime/java.exe\",\"kind\":\"relative\"}";

        var preference = JsonSerializer.Deserialize<JavaPreference>(json, JsonCompat.SerializerOptions);

        Assert.AreEqual(typeof(UseRelativePath), preference?.GetType());
        Assert.AreEqual("runtime/java.exe", ((UseRelativePath)preference!).RelativePath);
    }

    [TestMethod]
    public void ReadsCaseInsensitiveDiscriminatorWithDefaultOptions()
    {
        const string json = "{\"RelativePath\":\"runtime/java.exe\",\"KIND\":\"relative\"}";

        var preference = JsonSerializer.Deserialize<JavaPreference>(json);

        Assert.AreEqual(typeof(UseRelativePath), preference?.GetType());
        Assert.AreEqual("runtime/java.exe", ((UseRelativePath)preference!).RelativePath);
    }

    [TestMethod]
    public void WritesExistingDiscriminatorShape()
    {
        var json = JsonSerializer.Serialize<JavaPreference>(
            new ExistingJava("C:\\Java\\bin\\javaw.exe"), JsonCompat.SerializerOptions);
        using var document = JsonDocument.Parse(json);
        var properties = document.RootElement.EnumerateObject();

        Assert.IsTrue(properties.MoveNext());
        Assert.AreEqual("kind", properties.Current.Name);
        Assert.AreEqual("exist", properties.Current.Value.GetString());
        Assert.AreEqual("C:\\Java\\bin\\javaw.exe",
            document.RootElement.GetProperty("JavaExePath").GetString());
    }
}
