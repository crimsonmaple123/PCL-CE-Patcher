using System;
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        Console.Title = "PCL CE Patcher v1.0";
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔══════════════════════════════════════╗");
        Console.WriteLine("║        PCL CE Patcher v1.0          ║");
        Console.WriteLine("║    PCL CE Offline Mode Patcher      ║");
        Console.WriteLine("╚══════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        // 获取 PCL CE 路径
        string exePath;
        if (args.Length > 0 && File.Exists(args[0]))
        {
            exePath = args[0];
        }
        else
        {
            Console.Write("请输入 PCL CE 可执行文件的完整路径: ");
            Console.ForegroundColor = ConsoleColor.White;
            exePath = Console.ReadLine()?.Trim('"', ' ') ?? "";
            Console.ResetColor();
        }

        if (!File.Exists(exePath))
        {
            Error("文件不存在: " + exePath);
            Pause();
            return;
        }

        if (!exePath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
        {
            Error("请选择 .exe 文件");
            Pause();
            return;
        }

        Console.WriteLine();
        Info($"目标文件: {exePath}");
        Console.WriteLine();

        try
        {
            string exeDir = Path.GetDirectoryName(exePath)!;
            string patchDir = Path.Combine(exeDir, "PCL_CE_Patch");
            string hookDir = Path.Combine(patchDir, "hook");
            string hookSrcDir = Path.Combine(hookDir, "src");
            string hookBinDir = Path.Combine(hookDir, "bin");

            Directory.CreateDirectory(patchDir);
            Directory.CreateDirectory(hookSrcDir);

            // ========== 步骤1: 创建 Hook DLL ==========
            Step("步骤1: 创建 Startup Hook...");

            string hookCs = @"
using System;
using System.Globalization;
using System.Threading;

internal class StartupHook
{
    public static void Initialize()
    {
        try
        {
            var zhCN = new CultureInfo(""zh-CN"");
            CultureInfo.DefaultThreadCurrentCulture = zhCN;
            CultureInfo.DefaultThreadCurrentUICulture = zhCN;
            Thread.CurrentThread.CurrentCulture = zhCN;
            Thread.CurrentThread.CurrentUICulture = zhCN;
        }
        catch { }
    }
}
";
            File.WriteAllText(Path.Combine(hookSrcDir, "StartupHook.cs"), hookCs);
            File.WriteAllText(Path.Combine(hookSrcDir, "hook.csproj"),
@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <OutputType>Library</OutputType>
  </PropertyGroup>
</Project>");

            Info("  编译中...");
            var proc = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"build -c Release -o \"{hookBinDir}\"",
                WorkingDirectory = hookSrcDir,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            });
            proc!.WaitForExit();

            string hookDll = Path.Combine(hookBinDir, "hook.dll");
            if (!File.Exists(hookDll))
            {
                Error("  Hook 编译失败: " + proc.StandardError.ReadToEnd());
                Pause();
                return;
            }
            Success($"  Hook DLL: {hookDll}");

            // ========== 步骤2: 创建离线档案 ==========
            Step("步骤2: 检查离线档案...");

            string profileDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                ".pclce");
            string profilePath = Path.Combine(profileDir, "profiles.json");
            Directory.CreateDirectory(profileDir);

            bool needProfile = true;
            if (File.Exists(profilePath))
            {
                string content = File.ReadAllText(profilePath);
                if (!content.Contains("\"profiles\":[]") && !content.Contains("\"profiles\": []"))
                {
                    Success("  已有档案，无需创建");
                    needProfile = false;
                }
            }

            if (needProfile)
            {
                if (File.Exists(profilePath))
                    File.Copy(profilePath, profilePath + ".backup", true);

                string uuid = Guid.NewGuid().ToString();
                string json = "{\"lastUsed\":0,\"profiles\":[{\"type\":\"Legacy\",\"username\":\"Player\",\"uuid\":\"" + uuid + "\"}]}";
                File.WriteAllText(profilePath, json);
                Success("  已创建离线档案 (Player)");
            }

            // ========== 步骤3: 创建启动脚本 ==========
            Step("步骤3: 创建启动脚本...");

            // BAT
            string batPath = Path.Combine(patchDir, "Launch_PCL_CE.bat");
            File.WriteAllText(batPath,
$"@echo off\r\nset DOTNET_STARTUP_HOOKS={hookDll}\r\nstart \"\" \"{exePath}\"\r\n");
            Success($"  BAT: {batPath}");

            // VBS (无黑窗)
            string vbsPath = Path.Combine(patchDir, "Launch_PCL_CE.vbs");
            File.WriteAllText(vbsPath,
$"Set WshShell = CreateObject(\"WScript.Shell\")\r\nSet WshEnv = WshShell.Environment(\"Process\")\r\nWshEnv(\"DOTNET_STARTUP_HOOKS\") = \"{hookDll}\"\r\nWshShell.Run \"\"\"{exePath}\"\"\", 1, False\r\n");
            Success($"  VBS: {vbsPath}");

            // ========== 步骤4: 创建桌面快捷方式 ==========
            Step("步骤4: 创建桌面快捷方式...");
            try
            {
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string lnkPath = Path.Combine(desktop, "PCL CE (Patched).lnk");
                string tmpVbs = Path.Combine(Path.GetTempPath(), "mk_lnk.vbs");
                File.WriteAllText(tmpVbs,
$"Set s = CreateObject(\"WScript.Shell\").CreateShortcut(\"{lnkPath}\")\r\ns.TargetPath = \"wscript.exe\"\r\ns.Arguments = \"\"\"{vbsPath}\"\"\"\r\ns.WorkingDirectory = \"{patchDir}\"\r\ns.IconLocation = \"{exePath},0\"\r\ns.Save\r\n");
                Process.Start(new ProcessStartInfo
                {
                    FileName = "wscript.exe",
                    Arguments = $"\"{tmpVbs}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                })!.WaitForExit();
                File.Delete(tmpVbs);
                Success($"  桌面快捷方式: {lnkPath}");
            }
            catch (Exception ex)
            {
                Warn($"  快捷方式创建失败: {ex.Message}");
            }

            // ========== 完成 ==========
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║          修补完成！                  ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("使用方法:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("  1. 双击桌面上的 \"PCL CE (Patched)\" 快捷方式");
            Console.WriteLine($"  2. 或运行: {batPath}");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("注意: 请始终通过快捷方式/脚本启动 PCL CE");
            Console.WriteLine("      不修改任何 PCL CE 文件，完全安全");
        }
        catch (Exception ex)
        {
            Error($"修补失败: {ex.Message}");
        }

        Pause();
    }

    static void Step(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    static void Info(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    static void Success(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    static void Warn(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    static void Error(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    static void Pause()
    {
        Console.WriteLine();
        Console.Write("按任意键退出...");
        Console.ReadKey();
    }
}
