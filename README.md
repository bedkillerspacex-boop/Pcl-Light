# PCL Light

PCL Light 是基于 PCL 的轻量化 Minecraft 启动器版本，目标是更快启动、更低常驻占用和更简洁的界面。

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
