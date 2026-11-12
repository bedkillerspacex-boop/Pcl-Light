using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PCL.Core.IO;

/// <summary>检查归档条目的逻辑路径，避免解压路径越出目标目录。</summary>
public static class ZipEntryPath
{
    public static bool IsExcluded(string entryName, IEnumerable<string>? exclusions)
    {
        if (string.IsNullOrEmpty(entryName) || exclusions is null)
            return false;

        var normalizedEntry = entryName.Replace('\\', '/');
        foreach (var exclusion in exclusions)
        {
            if (string.IsNullOrWhiteSpace(exclusion))
                continue;

            var prefix = exclusion.Replace('\\', '/').TrimEnd('/');
            if (prefix.Length == 0)
                continue;

            if (string.Equals(normalizedEntry, prefix, StringComparison.Ordinal) ||
                normalizedEntry.StartsWith(prefix + "/", StringComparison.Ordinal))
                return true;
        }

        return false;
    }

    public static bool TryGetSafeDestinationPath(string destinationRoot, string entryName, out string? fullPath)
    {
        fullPath = null;
        if (string.IsNullOrWhiteSpace(destinationRoot) || string.IsNullOrWhiteSpace(entryName))
            return false;

        var normalizedEntry = entryName.Replace('\\', '/');
        if (normalizedEntry.StartsWith("/", StringComparison.Ordinal) ||
            (normalizedEntry.Length >= 2 && char.IsLetter(normalizedEntry[0]) && normalizedEntry[1] == ':'))
            return false;

        var segments = normalizedEntry.Split('/');
        if (segments.Any(segment => segment == ".." || segment.Contains(':')))
            return false;

        try
        {
            var root = Path.GetFullPath(destinationRoot);
            var relativePath = string.Join(Path.DirectorySeparatorChar, segments.Where(segment => segment.Length > 0));
            if (relativePath.Length == 0)
                return false;

            var candidate = Path.GetFullPath(Path.Combine(root, relativePath));
            var relative = Path.GetRelativePath(root, candidate);
            if (relative == "." || relative == ".." ||
                relative.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal) ||
                Path.IsPathRooted(relative))
                return false;

            fullPath = candidate;
            return true;
        }
        catch (Exception ex) when (ex is ArgumentException or IOException or NotSupportedException)
        {
            return false;
        }
    }
}
