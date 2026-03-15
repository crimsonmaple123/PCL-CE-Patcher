\# PCL CE Patcher



!\[License](https://img.shields.io/badge/license-MIT-blue.svg)

!\[Platform](https://img.shields.io/badge/platform-Windows-lightgrey.svg)

!\[.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)



\*\*一个用于 PCL CE (社区版) 的离线模式修补工具。\*\*



\---



\## 项目介绍



PCL CE Patcher 是一个针对 \*\*PCL CE (Plain Craft Launcher 2 Community Edition)\*\* 的修补工具。



\### 工作原理



本工具 \*\*不修改任何 PCL CE 文件\*\*。它利用 .NET 官方提供的 \[Startup Hook](https://github.com/dotnet/runtime/blob/main/docs/design/features/host-startup-hook.md) 机制，在 PCL CE 启动时注入一个轻量级的 Hook DLL，将运行时的语言环境设置为 `zh-CN`，使 PCL CE 的地区检查通过，从而解除离线登录和第三方登录的限制。



```

工作流程：

1\. 启动脚本设置环境变量 DOTNET\_STARTUP\_HOOKS

2\. PCL CE 启动时 .NET 运行时自动加载 Hook DLL

3\. Hook 将 CultureInfo 设置为 zh-CN

4\. PCL CE 的地区检查通过，离线模式可用

```



\---



\## 功能特性



\- ✅ 恢复离线登录（Legacy 模式）

\- ✅ 恢复第三方登录（Authlib-Injector 等）

\- ✅ 不修改任何 PCL CE 原始文件

\- ✅ 一键修补，自动创建启动脚本和桌面快捷方式

\- ✅ 支持自定义选择 PCL CE 可执行文件路径

\- ✅ 支持拖拽文件到 Patcher 上直接修补



\---



\## 系统要求



\- Windows 10 / 11

\- \[.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) 或 \[.NET 8.0 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)

\- PCL CE (Plain Craft Launcher 2 Community Edition)



\---



\## 使用方法



\### 方式一：交互模式



1\. 从 \[Releases](../../releases) 下载最新版本的 `PCL\_CE\_Patcher.exe`

2\. 双击运行 `PCL\_CE\_Patcher.exe`

3\. 输入 PCL CE 可执行文件的完整路径（例如 `D:\\Games\\PCL2\_CE\_Release\_x64.exe`）

4\. 等待修补完成



\### 方式二：拖拽模式



1\. 将 PCL CE 的 exe 文件直接 \*\*拖拽\*\* 到 `PCL\_CE\_Patcher.exe` 上

2\. 自动完成修补



\### 方式三：命令行模式



```bash

PCL\_CE\_Patcher.exe "D:\\Games\\PCL2\_CE\_Release\_x64.exe"

```



\### 修补完成后



修补完成后，会在 PCL CE 所在目录下生成一个 `PCL\_CE\_Patch` 文件夹，包含以下文件：



```

PCL\_CE\_Patch/

├── hook/

│   ├── bin/

│   │   └── hook.dll          # Startup Hook DLL

│   └── src/

│       ├── StartupHook.cs    # Hook 源码

│       └── hook.csproj       # Hook 项目文件

├── Launch\_PCL\_CE.bat          # 启动脚本（命令行窗口）

└── Launch\_PCL\_CE.vbs          # 启动脚本（无命令行窗口）

```



\*\*请通过以下方式启动 PCL CE：\*\*



\- 双击 `Launch\_PCL\_CE.bat` 或 `Launch\_PCL\_CE.vbs`

\- 或使用修补时自动创建的桌面快捷方式 `PCL CE (Patched)`



> ⚠️ \*\*请勿直接双击原始的 PCL CE exe 文件\*\*，否则 Hook 不会生效，离线模式不可用。



\---



\## 常见问题



\### Q: 为什么必须通过脚本启动？



本工具通过 .NET 的 `DOTNET\_STARTUP\_HOOKS` 环境变量注入 Hook。只有通过启动脚本设置了该环境变量后启动的 PCL CE 才会加载 Hook。直接双击原始 exe 不会设置环境变量，因此 Hook 不会生效。



\### Q: 可以设置为系统环境变量吗？



可以。将 `DOTNET\_STARTUP\_HOOKS` 设置为用户环境变量，值为 `hook.dll` 的完整路径，这样直接双击原始 exe 也能生效。但请注意，这会影响所有 .NET 8 应用程序的启动。



\### Q: 修补后 PCL CE 更新了怎么办？



本工具不修改 PCL CE 文件，因此 PCL CE 更新后 Hook 仍然有效，无需重新修补。除非 PCL CE 更改了地区检查的逻辑。



\### Q: 修补后可以恢复原样吗？



可以。删除 `PCL\_CE\_Patch` 文件夹和桌面快捷方式即可完全恢复。如果修补时自动创建了离线档案，可以在 `%APPDATA%\\.pclce\\` 目录下找到 `profiles.json.backup` 进行恢复。



\### Q: 修补后出现报错怎么办？



请检查以下事项：

1\. 确认已安装 .NET 8.0 SDK 或 Runtime

2\. 确认通过脚本启动而非直接双击 exe

3\. 将问题截图和详细描述通过 \[Issues](../../issues) 提交



\---



\## 免责声明



1\. \*\*完全免费\*\*：本项目完全开源免费，作者未从中获取任何收益。如果您是付费获得此工具的，建议立即退款并举报商家。



2\. \*\*不修改原始文件\*\*：本工具不修改任何 PCL CE 的二进制文件、源代码或资源文件。仅通过 .NET 运行时的官方 Startup Hook 机制设置语言环境。



3\. \*\*非官方项目\*\*：本项目与 PCL2 原作者（龙腾猫跃）及 PCL CE 社区开发组 \*\*无任何关联\*\*。

&#x20;  - 使用本工具后遇到的任何问题，请 \*\*不要\*\* 向 PCL CE 或 PCL2 官方仓库反馈。

&#x20;  - 如果您需要官方支持，请使用原版 PCL CE。



4\. \*\*风险自担\*\*：本软件按"原样"提供，不提供任何担保。使用本工具产生的任何后果由用户自行承担。



\---



\## 构建



\### 环境要求



\- .NET 8.0 SDK



\### 编译



```bash

git clone https://github.com/crimsonmaple123/PCL-CE-Patcher.git

cd PCL-CE-Patcher/PCL\_CE\_Patcher

dotnet build -c Release

```



\### 发布为独立可执行文件



```bash

dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o publish

```



\---



\## 许可证



本项目源代码遵循 \[MIT License](LICENSE) 开源协议。



\---



\## 致谢



\- \[.NET Startup Hook](https://github.com/dotnet/runtime/blob/main/docs/design/features/host-startup-hook.md) - Microsoft 提供的运行时注入机制

```



\---



