# PCL CE Patcher

![GitHub release (latest by date)](https://img.shields.io/github/v/release/crimsonmaple123/PCL-CE-Patcher) ![Platform](https://img.shields.io/badge/platform-Windows-lightgrey.svg) ![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg) [![Bilibili](https://img.shields.io/badge/Bilibili-Space-00a1d6?logo=bilibili&logoColor)](https://space.bilibili.com/579852212)

**一个用于 PCL CE (社区版) 的离线模式修补工具。**

## 项目介绍

PCL CE Patcher 是一个针对 **PCL CE (Plain Craft Launcher 2 Community Edition)** 的修补工具。<br>
**本项目使用Claude建造，可能出现未知bug。**

### 工作原理

本工具 **不修改任何 PCL CE 文件**。它利用 .NET 官方提供的 [Startup Hook](https://github.com/dotnet/runtime/blob/main/docs/design/features/host-startup-hook.md) 机制，在 PCL CE 启动时注入一个轻量级的 Hook DLL，将运行时的语言环境设置为 zh-CN，使 PCL CE 的地区检查通过，从而解除离线登录和第三方登录的限制。

## 功能特性

- ✅ 恢复离线登录（Legacy 模式）
- ✅ 恢复第三方登录（Authlib-Injector 等）
- ✅ 不修改任何 PCL CE 原始文件
- ✅ 一键修补，自动创建启动脚本和桌面快捷方式
- ✅ 支持自定义选择 PCL CE 可执行文件路径
- ✅ 支持拖拽文件到 Patcher 上直接修补

## 系统要求

- Windows 10 / 11
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) 或更高版本（8.0 / 9.0 / 10.0 均可）
- PCL CE ([Plain Craft Launcher 2 Community Edition](https://github.com/PCL-Community/PCL-CE))

## 使用方法

### 方式一：交互模式

1. 从 [Releases](../../releases) 下载最新版本的 `PCL_CE_Patcher.exe`
2. 双击运行 `PCL_CE_Patcher.exe`
3. 输入 PCL CE 可执行文件的完整路径
4. 等待修补完成

### 方式二：拖拽模式

将 PCL CE 的 exe 文件直接 **拖拽** 到 `PCL_CE_Patcher.exe` 上，自动完成修补。

### 修补完成后

修补完成后，会在 PCL CE 所在目录下生成一个 `PCL_CE_Patch` 文件夹：
PCL_CE_Patch/
├── hook/bin/hook.dll # Startup Hook DLL
├── Launch_PCL_CE.bat # 启动脚本（命令行窗口）
└── Launch_PCL_CE.vbs # 启动脚本（无命令行窗口）

**请通过 `Launch_PCL_CE.bat` 或 `Launch_PCL_CE.vbs` 启动 PCL CE。**

> ⚠️ 请勿直接双击原始的 PCL CE exe 文件，否则 Hook 不会生效。

## 常见问题

### 为什么必须通过脚本启动？

本工具通过 .NET 的 DOTNET_STARTUP_HOOKS 环境变量注入 Hook。只有通过启动脚本设置了该环境变量后启动的 PCL CE 才会加载 Hook。

### 修补后 PCL CE 更新了怎么办？

本工具不修改 PCL CE 文件，因此 PCL CE 更新后 Hook 仍然有效，无需重新修补。

### 修补后可以恢复原样吗？

可以。删除 `PCL_CE_Patch` 文件夹和桌面快捷方式即可完全恢复。

### 可以不用脚本启动吗？

可以。在系统环境变量中添加 `DOTNET_STARTUP_HOOKS`，值设为 `hook.dll` 的完整路径，这样直接双击原始 exe 也能生效。

## 免责声明

1. **完全免费**：本项目完全开源免费，作者未从中获取任何收益。如果您是付费获得此工具的，建议立即退款并举报商家。
2. **不修改原始文件**：本工具不修改任何 PCL CE 的二进制文件。仅通过 .NET 运行时的官方 Startup Hook 机制设置语言环境。
3. **非官方项目**：本项目与 PCL2 原作者（龙腾猫跃）及 PCL CE 社区开发组无任何关联。使用本工具后遇到的问题，请不要向官方仓库反馈。
4. **风险自担**：本软件按"原样"提供，不提供任何担保。





