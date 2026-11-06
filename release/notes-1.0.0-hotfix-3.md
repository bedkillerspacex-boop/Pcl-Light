PCL Light 1.0.0 热修复 3。

## 同步 PCL Lite 的修复

- 修复图像加载器错误的空参数检查。
- 修复实例错误状态下描述被无效赋值的问题。
- 修复等值对象缺少哈希实现导致集合查找不一致的问题。
- 修复离线 UUID 转换和若干 Release 构建告警。

## 收紧发布与提权边界

- 编译期只允许注入微软 OAuth Client ID 与 Git 提交 SHA，API key 和服务器密钥不会写入客户端。
- Release 不再读取运行时 `PCL_MS_CLIENT_ID` 环境变量，Debug 构建仍支持本地回退。
- 删除提权服务中的通用进程启动、结束和 JSON 启动操作。
- 发布工作流固定使用 .NET 6，并校验版本、配置、架构和发布身份。

## 构建

- 目标框架：`.NET 6.0`（`net6.0-windows`）
- 支持：Windows 7 SP1 及以上
- 发布：x64 单文件
- 本地 Release 单文件实测：17.05 MiB（GitHub Actions 发布包会随注入的 Client ID 略有差异）
- GitHub Release 附件带 Sigstore 签名文件
