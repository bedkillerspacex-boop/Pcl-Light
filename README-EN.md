[简体中文](README.md) | **English** | [繁體中文](README-ZH_TW.md)

# PCL Light

PCL Light is a lightweight Minecraft launcher focused on the core workflow: managing instances, downloading game files and loaders, launching Minecraft, and viewing logs.

## Highlights

- Built for `.NET 6.0` (`net6.0-windows`), with Windows 7 SP1 and later support.
- Single-file x64 publishing is supported.
- Simplified Chinese is built in; the language switcher has been removed.
- The homepage content system, tools page, network authorization, Lunar, Bedrock, MCP, Discord RPC, advanced materials, font switching, and music playback are not included.
- Startup keeps only the local lightweight launch panel and does not create homepage downloaders or file watchers.

PCL Light keeps the launcher core small. For a different balance with more features and lower idle memory use, see [PCL Lite](https://github.com/askbbtsk/pcl-lite).

## Size and memory

The following figures are x64 single-file measurements. Memory figures are measured on the same Windows device after startup and are included for comparison, not as a hardware guarantee.

| Version | Single-file size | Idle Private Bytes | Idle Working Set |
| --- | ---: | ---: | ---: |
| PCL CE 2.15.1 | 22.40 MiB | 167.0 MiB | 208.1 MiB |
| PCL Light 1.0-hotfix-2 | 17.19 MiB | pending a fresh same-machine sample | pending a fresh same-machine sample |

## Download

See the [PCL Light releases](https://github.com/bedkillerspacex-boop/Pcl-Light/releases).

The published x64 build is a single executable. A framework-dependent build requires the [.NET 6 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/6.0); self-contained publishing includes the runtime in the executable.

## Build

Install the [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) and the Windows Desktop targeting pack, then run:

```powershell
dotnet restore "Plain Craft Launcher 2/Plain Craft Launcher 2.csproj" -p:Platform=x64
dotnet build "Plain Craft Launcher 2/Plain Craft Launcher 2.csproj" -c Release -p:Platform=x64 --no-restore
```

To publish a single-file x64 build:

```powershell
dotnet publish "Plain Craft Launcher 2/Plain Craft Launcher 2.csproj" -c Release -p:Platform=x64 -r win-x64 --self-contained false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

## Documentation

- [Build and release](docs/BUILD.md)
- [Configuration](docs/CONFIG.md)
- [Themes](docs/THEME.md)
- [Open-source inventory](docs/CREDITS.md)

## License

See [LICENSE](LICENSE). The license file is authoritative.
