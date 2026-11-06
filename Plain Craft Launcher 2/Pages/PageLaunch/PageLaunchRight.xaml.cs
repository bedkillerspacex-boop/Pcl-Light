using System.IO;
using System.Windows;
using PCL.Core.App.Localization;

namespace PCL;

/// <summary>
/// 启动页右侧的轻量日志面板。
/// 主页网络加载、动态 XAML 和实时补丁已移除，避免启动时创建后台下载器、定时器和文件监视器。
/// </summary>
public partial class PageLaunchRight
{
    public PageLaunchRight()
    {
        InitializeComponent();
        PanBack.ScrollToHome();
        PanScroll = PanBack;
        PanLog.Visibility = ModBase.modeDebug ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>返回启动页提示文本，保留启动流程需要的本地提示能力。</summary>
    public static string GetRandomHint(bool enableLengthLimit = false, bool raw = false)
    {
        var externalPath = Path.Combine(ModBase.exePath, "PCL", "hints.txt");
        string[]? lines = null;
        if (File.Exists(externalPath))
        {
            try
            {
                lines = File.ReadAllLines(externalPath)
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(line => line.Trim())
                    .ToArray();
            }
            catch (Exception ex)
            {
                ModBase.Log(ex, "读取启动提示失败", ModBase.LogLevel.Developer);
            }
        }

        lines ??= LoadEmbeddedHints(LocalizationService.DefaultLanguageCode);
        if (lines.Length == 0) return string.Empty;
        if (enableLengthLimit)
        {
            var shortLines = lines.Where(line => line.Length < 50).ToArray();
            if (shortLines.Length > 0) lines = shortLines;
        }

        var hint = lines[Random.Shared.Next(lines.Length)];
        return raw
            ? hint
            : hint.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")
                .Replace("\"", "&quot;");
    }

    private static string[] LoadEmbeddedHints(string languageCode)
    {
        try
        {
            var uri = new Uri($"pack://application:,,,/Plain Craft Launcher 2;component/Resources/hints/{languageCode}.txt", UriKind.Absolute);
            using var stream = Application.GetResourceStream(uri)?.Stream;
            if (stream is null) return [];
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd()
                .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Trim())
                .ToArray();
        }
        catch
        {
            return [];
        }
    }
}
