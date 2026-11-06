[简体中文](README.md) | [English](README-EN.md) | **繁體中文**

# PCL Light

PCL Light 是一個專注於核心流程的輕量 Minecraft 啟動器：管理遊戲實例、下載遊戲檔案與載入器、啟動 Minecraft，以及查看日誌。

## 主要特色

- 以 `.NET 6.0`（`net6.0-windows`）編譯，支援 Windows 7 SP1 及更新版本。
- 支援 x64 單檔發布。
- 內建簡體中文，已移除語言切換器。
- 不包含首頁內容系統、工具頁、網路授權、Lunar、Bedrock、MCP、Discord RPC、高級材質、字體切換與音樂播放。
- 啟動時只保留輕量本地啟動面板，不建立首頁下載器或檔案監視器。

PCL Light 保留啟動器核心能力並縮小體積。若需要更多功能與更低的閒置記憶體占用，請參考 [PCL Lite](https://github.com/askbbtsk/pcl-lite)。

## 體積與記憶體

下列為 x64 單檔測量值。記憶體數據在同一台 Windows 裝置上啟動後測量，僅供比較，不代表所有硬體環境。

| 版本 | 單檔大小 | 閒置 Private Bytes | 閒置 Working Set |
| --- | ---: | ---: | ---: |
| PCL CE 2.15.1 | 22.40 MiB | 167.0 MiB | 208.1 MiB |
| PCL Light 1.0-hotfix-2 | 17.19 MiB | 待同機重新測量 | 待同機重新測量 |

## 下載

前往 [PCL Light Releases](https://github.com/bedkillerspacex-boop/Pcl-Light/releases)。

發布的 x64 版本是單一執行檔。依賴框架的版本需要 [.NET 6 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/6.0)；自包含發布會把執行階段放入執行檔。

## 建置

安裝 [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) 與 Windows Desktop targeting pack，然後執行：

```powershell
dotnet restore "Plain Craft Launcher 2/Plain Craft Launcher 2.csproj" -p:Platform=x64
dotnet build "Plain Craft Launcher 2/Plain Craft Launcher 2.csproj" -c Release -p:Platform=x64 --no-restore
```

發布 x64 單檔版本：

```powershell
dotnet publish "Plain Craft Launcher 2/Plain Craft Launcher 2.csproj" -c Release -p:Platform=x64 -r win-x64 --self-contained false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

## 文件

- [建置與發布](docs/BUILD.md)
- [設定](docs/CONFIG.md)
- [主題](docs/THEME.md)
- [開源專案清單](docs/CREDITS.md)

## 授權條款

詳見 [LICENSE](LICENSE)，完整條款以該檔案為準。
