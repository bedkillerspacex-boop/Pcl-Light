using System;
using System.IO;
using PCL.Core.IO;

namespace PCL.Core.Minecraft;

public static class AssetIndexPath
{
    public static string ResolveLocalPath(string assetsRoot, string resourcesRoot, string objectName,
        string hash, bool mapToResources, bool isVirtual)
    {
        ArgumentNullException.ThrowIfNull(assetsRoot);
        ArgumentNullException.ThrowIfNull(resourcesRoot);
        ArgumentNullException.ThrowIfNull(objectName);
        ArgumentNullException.ThrowIfNull(hash);

        if (hash.Length != 40 || !IsHex(hash))
            throw new InvalidDataException("资源索引包含无效的 SHA-1 哈希。");

        var root = mapToResources
            ? resourcesRoot
            : isVirtual
                ? Path.Combine(assetsRoot, "virtual", "legacy")
                : Path.Combine(assetsRoot, "objects", hash[..2]);
        var relativePath = mapToResources || isVirtual ? objectName : hash;
        if (!ZipEntryPath.TryGetSafeDestinationPath(root, relativePath, out var path))
            throw new InvalidDataException("资源索引包含越界的逻辑路径。");

        return path!;
    }

    private static bool IsHex(string value)
    {
        foreach (var character in value)
        {
            if (!Uri.IsHexDigit(character))
                return false;
        }

        return true;
    }
}
