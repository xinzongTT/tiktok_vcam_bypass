# TikTok 虚拟摄像头绕过工具

一键绕过 TikTok LIVE Studio 的虚拟摄像头检测，让 OBS Virtual Camera 被识别为真实摄像头。

## 原理

TikTok LIVE Studio 通过 JavaScript 函数 `isVirtualCamera(deviceId, deviceName)` 判断摄像头是否为虚拟设备。检测逻辑分三层：

1. **硬编码排除** — `deviceId` 以 `"ByteCast VirtualCamera"` 开头则不判定为虚拟
2. **服务端配置** — 远程下发物理/虚拟摄像头黑白名单
3. **本地启发式** — `deviceId === deviceName` 或设备 ID 不含硬件路径（USB/PCI）

本工具通过修改 TikTok 的内部 JS 文件，使 `isVirtualCamera` 始终返回 `false`。

## 使用方法

1. **下载** [Releases](https://github.com/xinzongTT/tiktok_vcam_bypass/releases) 中的 `tiktok_vcam_bypass.exe`
2. 双击运行（无需安装，自带 .NET 运行时）
3. 点击 **启用绕过** 按钮
4. 重启 TikTok LIVE Studio

点击 **恢复原始** 即可撤销修改。

## 自行编译

```bash
git clone https://github.com/xinzongTT/tiktok_vcam_bypass.git
cd tiktok_vcam_bypass/src
dotnet publish -c Release -o ../publish
```

要求: .NET 8.0 SDK

## 支持的版本

| TikTok LIVE Studio | 状态 |
|-------------------|------|
| v1.27.0           | ✓ 测试通过 |

其他版本可能因 JS 结构差异无法自动补丁。

## 免责声明

本工具仅供学习研究使用。修改软件行为可能违反 TikTok 服务条款，使用者自行承担风险。

## License

MIT
