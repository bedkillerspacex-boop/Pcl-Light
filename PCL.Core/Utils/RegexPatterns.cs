using System.Text.RegularExpressions;

namespace PCL.Core.Utils;

/// <summary>
/// 基于代码生成优化的正则表达式实例。
/// </summary>
public static partial class RegexPatterns
{
    /// <summary>
    /// 陶瓦联机 ID。
    /// </summary>
    public static readonly Regex TerracottaId = _TerracottaId();

    private static Regex _TerracottaId() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get("([0-9A-Z]{5}-){4}[0-9A-Z]{5}", RegexOptions.IgnoreCase);

    /// <summary>
    /// 换行符，包括 <c>\r\n</c> <c>\n</c> <c>\r</c> 三种。
    /// </summary>
    public static readonly Regex NewLine = _NewLine();

    private static Regex _NewLine() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"\r\n|\n|\r");

    /// <summary>
    /// ISO 8601 当日终点时间写法 24:00(:00)(.0…)，语义等于次日零点。
    /// </summary>
    public static readonly Regex Iso8601EndOfDay = _Iso8601EndOfDay();

    private static Regex _Iso8601EndOfDay() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"[Tt]24:00(?::00(?:\.0+)?)?(?=[Zz+\-]|$)");

    /// <summary>
    /// Semantic Versioning (SemVer) 规范的版本号，包含可选的 v 前缀。
    /// </summary>
    public static readonly Regex SemVer = _SemVer();
    private const string PatternSemVer =
        @"^v?(?<major>0|[1-9]\d*)\.(?<minor>0|[1-9]\d*)\.(?<patch>0|[1-9]\d*)" +
        @"(?:-(?<prerelease>(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?" +
        @"(?:\+(?<build>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$";

    private static Regex _SemVer() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(PatternSemVer, RegexOptions.ExplicitCapture);

    /// <summary>
    /// 简单匹配 HTTP(S) URI，若需严格检查请使用 <see cref="FullHttpUri"/>。
    /// </summary>
    public static readonly Regex HttpUri = _HttpUri();
    private const string PatternHttpUri = @"^https?://(?:\[[^\]\s]+\]|[^/\s?#:]+)(?::\d{1,5})?(?:/[^\s?#]*)?(?:\?[^\s#]*)?(?:#\S*)?$";

    private static Regex _HttpUri() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(PatternHttpUri, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    /// <summary>
    /// 包含完整规则的 HTTP(S) URI，含有 <c>scheme</c> <c>host</c> <c>ipv6</c>
    /// <c>port</c> <c>path</c> <c>query</c> <c>fragment</c> 分组。
    /// </summary>
    public static readonly Regex FullHttpUri = _FullHttpUri();
    private const string PatternFullHttpUri =
        @"^(?<scheme>https?)://(?<host>localhost|(?:(?:25[0-5]|2[0-4]\d|1?\d?\d)(?:\.(?:25[0-5]|2[0-4]\d|1?\d?\d)){3})" +
        @"|\[(?<ipv6>[0-9A-Fa-f:.]+)\]|(?:(?:[A-Za-z0-9](?:[A-Za-z0-9\-]{0,61}[A-Za-z0-9])?\.)+(?:[A-Za-z]{2,63}|xn--[" +
        @"A-Za-z0-9\-]{2,59})))(?::(?<port>6553[0-5]|655[0-2]\d|65[0-4]\d{2}|6[0-4]\d{3}|[1-5]\d{4}|[1-9]\d{0,3}))?" +
        @"(?<path>/[^\s?#]*)?(?:\?(?<query>[^\s#]*))?(?:#(?<fragment>[^\s]*))?$";

    private static Regex _FullHttpUri() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(PatternFullHttpUri, RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    /// <summary>
    /// LastPending(_Xxx).log 路径。
    /// </summary>
    public static readonly Regex LastPendingLogPath = _LastPendingLogPath();

    private static Regex _LastPendingLogPath() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"\\LastPending[_]?[^\\]*\.log$", RegexOptions.IgnoreCase);

    /// <summary>
    /// Mod Loader 不兼容的错误提示。
    /// </summary>
    public static readonly Regex IncompatibleModLoaderErrorHint = _IncompatibleModLoaderErrorHint();

    private static Regex _IncompatibleModLoaderErrorHint() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"(incompatible[\s\S]+'Fabric Loader' \(fabricloader\)|Mod ID: '(?:neo)?forge', Requested by '([^']+)')");

    /// <summary>
    /// Minecraft 颜色代码，为 Hex 颜色代码，格式为 <c>#RRGGBB</c>。
    /// </summary>
    public static readonly Regex HexColor = _HexColor();

    private static Regex _HexColor() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get("^#[0-9A-Fa-f]{6}$");

    /// <summary>
    /// A compiled regular expression for matching Minecraft MOTD formatting codes.
    /// Matches legacy color/format codes (e.g., §a, §b, §k) and hexadecimal color codes (e.g., #FF0000).
    /// </summary>
    public static readonly Regex MotdCode = _MotdCode();

    private static Regex _MotdCode() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get("(§[0-9a-fk-oAr]|#[0-9A-Fa-f]{6})");

    public static readonly Regex BroadcastMotd = _BroadcastMotd();

    private static Regex _BroadcastMotd() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"\[MOTD\](.*?)\[/MOTD\]", RegexOptions.Compiled);

    public static readonly Regex BroadcastAd = _BroadcastAd();

    private static Regex _BroadcastAd() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"\[AD\](.*?)\[/AD\]", RegexOptions.Compiled);

    /// <summary>
    /// 匹配 Minecraft 正常版本号，如 1.20.4、1.19.3 等。
    /// </summary>
    public static readonly Regex McNormalVersion = _McNormalVersion();

    private static Regex _McNormalVersion() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"^\d+\.\d+\.\d+$|^\d+\.\d+$");

    /// <summary>
    /// 匹配 Minecraft 快照版本号，如 24w14a 等。
    /// </summary>
    public static readonly Regex McSnapshotVersion = _McSnapshotVersion();

    private static Regex _McSnapshotVersion() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"(\d+)w(\d+)([a-z]?)");

    /// <summary>
    /// 匹配 Minecraft Indev 版本号，如 in-20091231-2、in-20100130 等。
    /// </summary>
    public static readonly Regex McIndevVersion = _McIndevVersion();

    private static Regex _McIndevVersion() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"^in-(\d{8})(-(\d+))?$");

    /// <summary>
    /// 匹配 Minecraft Infdev 版本号，如 inf-20100611 等。
    /// </summary>
    public static readonly Regex McInfdevVersion = _McInfdevVersion();

    private static Regex _McInfdevVersion() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"^inf-(\d{8})(-(\d+))?$");

    /// <summary>
    /// 匹配 accessToken 内容。
    /// </summary>
    public static readonly Regex AccessToken = _AccessToken();

    private static Regex _AccessToken() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get("(?<=accessToken ([^ ]{5}))[^ ]+(?=[^ ]{5})");

    /// <summary>
    /// 使用 IsMatch 检查是否存在中文字符
    /// </summary>
    public static readonly Regex HasChineseChar = _HasChineseChar();

    private static Regex _HasChineseChar() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get("[\u4e00-\u9fbb]");

    /// <summary>
    /// 用 Replace 替换英文中的分隔特征
    /// </summary>
    public static readonly Regex EnglishSpacedKeywords = _EnglishSpacedKeywords();

    private static Regex _EnglishSpacedKeywords() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get("([A-Z]+|[a-z]+?)(?=[A-Z]+[a-z]+[a-z ]*)");

    /// <summary>
    /// NTFS 8.3 文件名格式
    /// </summary>
    public static readonly Regex Ntfs83FileName = _Ntfs83FileName();

    private static Regex _Ntfs83FileName() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@".{2,}~\d");

    public static readonly Regex UncPath = _UncPath();

    private static Regex _UncPath() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get("""^\\\\[^\\/:*?"<>|]+\\[^\\/:*?"<>|]+(\\[^\\/:*?"<>|]+)*\\?$""");

    #region Minecraft 实例解析

    public static readonly Regex OptiFineVersion = _OptiFineVersion();

    private static Regex _OptiFineVersion() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"(?<=HD_U_)[^"":/]+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static readonly Regex OptiFineLibVersion = _OptiFineLibVersion();

    private static Regex _OptiFineLibVersion() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"(?<=HD_U_)[^"":/]+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static readonly Regex LegacyFabricVersion = _LegacyFabricVersion();

    private static Regex _LegacyFabricVersion() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"(?<=(net.fabricmc:fabric-loader:))[0-9\.]+(\+build.[0-9]+)?", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static readonly Regex FabricVersion = _FabricVersion();

    private static Regex _FabricVersion() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"(?<=(net.fabricmc:fabric-loader:))[0-9\.]+(\+build.[0-9]+)?", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static readonly Regex QuiltVersion = _QuiltVersion();

    private static Regex _QuiltVersion() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"(?<=(org.quiltmc:quilt-loader:))[0-9\.]+(\+build.[0-9]+)?((-beta.)[0-9]([0-9]?))", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static readonly Regex CleanroomVersion = _CleanroomVersion();

    private static Regex _CleanroomVersion() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"(?<=(com.cleanroommc:cleanroom:))[0-9\.]+(\+build.[0-9]+)?(-alpha)?", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    /// <summary>
    /// 匹配 Forge 主版本号（位于 "forge:X.Y.Z-" 之后）。
    /// </summary>
    public static readonly Regex ForgeMainVersion = _ForgeMainVersion();

    private static Regex _ForgeMainVersion() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"(?<=forge:[0-9\.]+(_pre[0-9]*)?\-)[0-9\.]+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    /// <summary>
    /// 匹配 Forge Maven 坐标中的版本号（net.minecraftforge:minecraftforge:X.Y.Z）。
    /// </summary>
    public static readonly Regex ForgeLibVersion = _ForgeLibVersion();

    private static Regex _ForgeLibVersion() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"(?<=net\.minecraftforge:(?:forge|fmlloader):[0-9.]+-)[0-9a-zA-Z._+-]+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    /// <summary>
    /// 匹配 NeoForge 版本号（从 JSON 参数中提取，如 "--fml.neoForgeVersion", "20.6.119-beta"）。
    /// </summary>
    public static readonly Regex NeoForgeVersion = _NeoForgeVersion();

    private static Regex _NeoForgeVersion() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"(?<=orgeVersion"",[^""]*?"")[^""]+(?="",)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static readonly Regex FabricLikeLibVersion = _FabricLikeLibVersion();

    private static Regex _FabricLikeLibVersion() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"(?<=((fabricmc)|(quiltmc)|(legacyfabric)):intermediary:)[^""]*", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static readonly Regex LabyModVersion = _LabyModVersion();

    private static Regex _LabyModVersion() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"(?<=-Dnet.labymod.running-version=)1.[0-9+.]+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static readonly Regex MinecraftJsonVersion = _MinecraftJsonVersion();

    private static Regex _MinecraftJsonVersion() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"(([1-9][0-9]w[0-9]{2}[a-g])|((1|[2-9][0-9])\.[0-9]+(\.[0-9]+)?(-(pre|rc|snapshot-?)[1-9]*| Pre-Release( [1-9])?)?))(_unobfuscated)?", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static readonly Regex MinecraftDownloadUrlVersion = _MinecraftDownloadUrlVersion();

    private static Regex _MinecraftDownloadUrlVersion() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"(?<=launcher.mojang.com/mc/game/)[^/]*", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static readonly Regex CatchLwjglInLib = _CatchLwjglInLib();

    private static Regex _CatchLwjglInLib() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"(?<=org.lwjgl:)lwjgl(-[a-z._.\-.0-9]*)(?=(:[0-9].[0-9].[0-9](-[a-z.0-9._.\-]*)?:([a-z._.\-.0-9]*)?))", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    #endregion

    #region Minecraft 下载信息

    /// <summary>
    /// 匹配 NeoForge 版本列表 JSON 中的版本号
    /// </summary>
    public static readonly Regex DlNeoForgeVersion = _DlNeoForgeVersion();

    private static Regex _DlNeoForgeVersion() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"(?<="")(1\.20\.1-)?\d+\.[^\.]+\.\d+(\.\d+)?(-(beta|alpha)(\.\d+)?)?(\+snapshot-\d+)?(\+pre-\d+)?(?="")", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    #endregion

    #region 外部组件

    public static readonly Regex ModIdMatch = _ModIdMatch();

    private static Regex _ModIdMatch() => PCL.Core.Utils.Compatibility.RegexCompatibility.Get(@"[0-9a-zA-Z_-]+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    #endregion
}
