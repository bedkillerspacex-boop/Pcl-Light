# PCL Light

PCL Light 是基于 PCL 的轻量化 Minecraft 启动器版本，目标是更快启动、更低常驻占用和更简洁的界面。

## PCL Light 的优势

PCL Light 面向只需要启动器核心能力的用户，保留 Minecraft 启动、下载、实例管理、设置和日志，同时去掉不参与核心启动流程的功能：主页内容系统、工具页、网络授权、Lunar、Bedrock、MCP、Discord RPC、高级材质、字体切换和背景音乐。启动阶段也不再创建音乐目录、查询硬件 WMI 或分配逐实例图片缓存。

### 体积和内存对比

文件大小为 x64 单文件实测值。内存数据来自同一台 Windows 设备：每个版本启动三次，每次等待 20 秒后连续采样五次（间隔 5 秒），先取每次运行的中位数，再取三次运行的中位数。Private Bytes 反映进程提交内存；Working Set 会受到系统文件缓存等因素影响。

PCL Light 基于 .NET 6，支持 Windows 7 SP1 及以上系统。

| 版本 | x64 单文件 | 空闲 Private Bytes（中位数；三次范围） | 空闲 Working Set（中位数；三次范围） |
| --- | ---: | ---: | ---: |
| PCL CE 2.15.1 | 22.40 MiB | 167.0 MiB（156.5–175.8） | 208.1 MiB（204.3–217.9） |
| PCL Light 1.0.0-hotfix-4 | 16.83 MiB | 本轮采样不稳定 | 本轮采样不稳定 |

本轮空闲内存采样受启动后窗口状态与页面初始化影响，尚未确认可重复的常驻内存下降，因此不把这些数值作为内存改进结论。

PCL Light 的定位是保留核心启动能力并减少附加功能；若需要更多功能和更低常驻 Private Bytes 占用，可访问独立的 [PCL Lite 仓库](https://github.com/askbbtsk/pcl-lite)。

## 当前版本特性

- 目标框架：`.NET 6.0`（`net6.0-windows`）
- 固定使用简体中文，已移除语言切换入口
- 保留启动、下载、实例管理、设置和日志等核心功能
- 已移除主页内容系统、工具页和网络授权弹窗
- 启动页使用本地轻量日志面板，不创建主页下载器、文件监视器或动态 XAML
- 启动阶段延后硬件 WMI 查询，只在导出崩溃报告时读取
- 清理无效的逐实例图片缓存，减少图片控件分配和常驻引用
- 单文件发布支持 `win-x64`

## 构建

构建需要 [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) 和 Windows Desktop targeting pack。运行单文件程序需要安装 [.NET 6 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/6.0)。

```powershell
dotnet restore "Plain Craft Launcher 2/Plain Craft Launcher 2.csproj" -p:Platform=x64
dotnet build "Plain Craft Launcher 2/Plain Craft Launcher 2.csproj" -c Release -p:Platform=x64 --no-restore
```

发布单文件：

```powershell
dotnet publish "Plain Craft Launcher 2/Plain Craft Launcher 2.csproj" -c Release -p:Platform=x64 -r win-x64 --self-contained false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

产物位于 `Plain Craft Launcher 2/bin/Release/net6.0-windows/win-x64/publish/`。

## 目录

- `Plain Craft Launcher 2/`：WPF 启动器界面和业务模块
- `PCL.Core/`：核心服务、配置、网络和 Minecraft 数据模型
- `docs/`：项目文档

## 许可证

项目代码遵循仓库中的 [LICENSE](LICENSE)。
