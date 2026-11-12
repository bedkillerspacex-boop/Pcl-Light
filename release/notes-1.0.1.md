PCL Light 1.0.1。

## 修复 Minecraft 启动兼容性

- 按所选 Java 的 32/64 位架构选择 Windows native classifier，并按版本 JSON 的 OS 架构规则筛选启动参数。
- 同时保留含 natives 声明的普通 artifact 与 native classifier，分别用于 classpath 和本地库提取。
- native 提取遵守 `extract.exclude`，拒绝 ZIP 条目路径穿越，并支持将 DLL 写入原生库子目录。
- 处理旧版 `virtual`、`map_to_resources` 和普通 SHA-1 对象资源索引，拒绝非法哈希和越界逻辑路径。
- 按 Windows 命令行转义规则解析与格式化启动参数，保留带空格、引号、空参数和末尾反斜杠的值。

新增回归覆盖 Java 架构 classifier、资源索引映射、ZIP 安全路径和 Windows 参数规则；全量 `.NET 6` 测试通过（447 项）。

发布 x64 与 ARM64 单文件，并附带 Authenticode 签名证书和 SHA-256 校验文件。构建为 framework-dependent，运行需要 .NET 6 Desktop Runtime。

签名文件 SHA-256：

- x64：`f28faa47a4689b637260ca2604c73b252522913888e2f80c609b40557fa3693b`
- ARM64：`7bee1f55f24639072fdc26078209e232a50109a2944f9429a6e018c3ba048458`
