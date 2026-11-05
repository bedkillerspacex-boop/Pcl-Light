using System;
using PCL.Core.Utils.Exts;
using PCL.Core.Utils.OS;

namespace PCL.Core.App;

// ReSharper disable InconsistentNaming
public static class Secrets
{
    /// <summary>
    /// 微软 OAuth 的 Client ID
    /// <para>
    /// 优先取编译期注入的密钥（构建时设 <c>PCL_WRITE_SECRET=1</c> 与 <c>PCL_MS_CLIENT_ID</c>），
    /// 其次读取运行时环境变量 <c>PCL_MS_CLIENT_ID</c>。
    /// 这里刻意允许 Release 读取运行时环境变量，这样用户不必重新编译就能接上自己的 Azure 应用。
    /// </para>
    /// <para><b>这是 PCL Lite 自己的身份</b>，主页面登录、普通账号刷新都用它。</para>
    /// </summary>
    public static string MSOAuthClientId { get; } = EnvironmentInterop.GetSecret("MS_CLIENT_ID").ReplaceNullOrEmpty();


    /// <summary>
    ///     In-Game Account Switcher（IAS）的 Azure 应用 client id。
    ///     <para>
    ///         同样是公共客户端、没有 secret，写在它自己的开源仓库里。
    ///         用途只有一个：导入别人从 IAS 里拿到的 Refresh Token —— 令牌绑死签发它的应用，
    ///         拿 PCL 的身份去换只会得到 <c>invalid_grant</c>。
    ///     </para>
    /// </summary>
    public const string IasOAuthClientId = "54fd49e4-2103-4044-9603-2b028c814ec3";

    /// <summary>
    /// CurseForge API 的 Client ID
    /// </summary>
    public static string CurseForgeAPIKey { get; } = EnvironmentInterop.GetSecret("CURSEFORGE_API_KEY", readEnvDebugOnly: true).ReplaceNullOrEmpty();

    /// <summary>
    /// 当前版本的 Git 提交 SHA
    /// </summary>
    public static string CommitHash { get; } = EnvironmentInterop.GetSecret("GITHUB_SHA", readEnvDebugOnly: true).ReplaceNullOrEmpty();



}
