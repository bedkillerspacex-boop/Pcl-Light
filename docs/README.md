# PCL Light 文档

这里是 PCL Light 的开发与发布文档。

## 目录

| 文档 | 内容 |
| --- | --- |
| [CONTRIBUTE.md](CONTRIBUTE.md) | 贡献、构建与提交约定 |
| [THEME.md](THEME.md) | 主题与配色系统 |
| [CONFIG.md](CONFIG.md) | 配置文件与数据目录 |
| [BUILD.md](BUILD.md) | 编译、密钥注入、签名与发布 |
| [AZURE-LOGIN.md](AZURE-LOGIN.md) | Azure 应用与正版登录配置 |
| [UPDATE.md](UPDATE.md) | 更新清单与更新源 |
| [MIRROR.md](MIRROR.md) | 镜像仓库与 Release 迁移 |
| [CREDITS.md](CREDITS.md) | 开源项目与第三方库清单 |

## 项目结构

```
PCL-Light/
├── PCL.Core/                     核心服务、配置、网络和 Minecraft 数据模型
├── Plain Craft Launcher 2/       WPF 界面、页面、控件与中文资源
├── PCL.Core.SourceGenerators/    编译期密钥注入源生成器
└── docs/                         项目文档
```

技术栈是 `.NET 6.0` + WPF，目标框架为 `net6.0-windows`，支持 Windows 7 SP1 及以上系统。

## 版本号

版本唯一来源是 `Plain Craft Launcher 2/metadata.json` 的 `version` 对象：

1. `base` 使用三段式版本号，例如 `1.0.0`。
2. `suffix` 用于预发布或热修复标记，例如 `hotfix-2`。
3. `code` 是内部递增版本代码。

项目文件会从 metadata.json 读取版本并生成程序集版本，修改版本时只需更新 metadata.json。

## 密钥

OAuth client ID 等密钥由 `PCL.Core.SourceGenerators` 在编译期从环境变量注入，不写入源码。构建与注入方式见 [BUILD.md](BUILD.md)。
