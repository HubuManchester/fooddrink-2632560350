# FlavorVault Windows 端适配指南

本文档记录了将一个原本只能运行在 Android 的 .NET MAUI 项目适配到 Windows 桌面端的完整步骤。

---

## 问题背景

原始项目仅面向 Android，`TargetFrameworks` 只有 `net9.0-android`，在 Windows 上无法编译和运行。需要做以下修改才能在 Windows 桌面端调试运行。

---

## 修改 1：`.csproj` — 添加 Windows 目标框架

**文件**: `FlavorVault/FlavorVault.csproj`

### 修改前

```xml
<PropertyGroup>
    <TargetFrameworks>net9.0-android</TargetFrameworks>
    <SupportedOSPlatformVersion>21</SupportedOSPlatformVersion>
</PropertyGroup>
```

### 修改后

```xml
<PropertyGroup>
    <TargetFrameworks>net9.0-android;net9.0-windows10.0.19041.0</TargetFrameworks>

    <!-- 按平台条件拆分 SupportedOSPlatformVersion -->
    <!-- Android API 21 (Android 5.0) -->
    <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'">21</SupportedOSPlatformVersion>
    <!-- Windows 10.0.19041.0 (Windows 10 2004) -->
    <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'windows'">10.0.19041.0</SupportedOSPlatformVersion>

    <!-- 允许不解包直接调试运行（否则会报 MSIX 打包错误） -->
    <WindowsPackageType>None</WindowsPackageType>
</PropertyGroup>
```

### 说明

| 修改点 | 原因 |
|--------|------|
| `TargetFrameworks` 添加 `net9.0-windows10.0.19041.0` | 让编译器知道需要生成 Windows 桌面目标 |
| `SupportedOSPlatformVersion` 按条件拆分 | Android 的 `21` 是 API level，Windows 需要的是 `10.0.19041.0`，两者不能共用同一个值，否则报 NETSDK1135 |
| `WindowsPackageType=None` | 不使用 MSIX 打包，直接以 exe 方式调试运行。否则会报"需要先部署"的错误 |

---

## 修改 2：`Platforms/Windows/App.xaml` — 修复命名空间冲突

**文件**: `FlavorVault/Platforms/Windows/App.xaml`

### 修改前

```xml
<maui:MauiWinUIApplication
    x:Class="FlavorVault.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:maui="using:Microsoft.Maui"
    xmlns:local="using:FlavorVault">
</maui:MauiWinUIApplication>
```

### 修改后

```xml
<maui:MauiWinUIApplication
    x:Class="FlavorVault.WinUI.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:maui="using:Microsoft.Maui"
    xmlns:local="using:FlavorVault">
</maui:MauiWinUIApplication>
```

### 说明

跨平台项目已有一个 `FlavorVault.App`（继承 `Application`），Windows 平台的 `App` 继承 `MauiWinUIApplication`，两者同名不同基类会产生冲突。将 Windows 端改为 `FlavorVault.WinUI.App` 即可解决。

---

## 修改 3：`Platforms/Windows/App.xaml.cs` — 修复类定义

**文件**: `FlavorVault/Platforms/Windows/App.xaml.cs`

### 修改前

```csharp
namespace FlavorVault;

public class App : MauiWinUIApplication
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
```

### 修改后

```csharp
using Microsoft.UI.Xaml;

namespace FlavorVault.WinUI;

public partial class App : MauiWinUIApplication
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);
    }
}
```

### 说明

| 修改点 | 原因 |
|--------|------|
| 命名空间改为 `FlavorVault.WinUI` | 与 `x:Class="FlavorVault.WinUI.App"` 匹配 |
| 添加 `partial` 修饰符 | XAML 代码生成器需要 partial 类来生成 `.g.cs` 文件 |
| `OnLaunched` 重写 | 确保 Windows 启动生命周期正确初始化 |

---

## 修改 4：`.sln` — 添加平台配置

**文件**: `FlavorVault.sln`

### 说明

原始 `.sln` 只有 `Any CPU` 平台配置，Windows 桌面需要具体的 CPU 架构（`x64`/`x86`/`ARM64`）。需要确保 `GlobalSection(ProjectConfigurationPlatforms)` 中包含以下配置：

```
Debug|Any CPU = Debug|Any CPU
Debug|ARM64 = Debug|ARM64
Debug|x64 = Debug|x64
Debug|x86 = Debug|x86
Release|Any CPU = Release|Any CPU
Release|ARM64 = Release|ARM64
Release|x64 = Release|x64
Release|x86 = Release|x86
```

每个配置都要有对应的 `.ActiveCfg`、`.Build.0`、`.Deploy.0` 映射。

---

## 修改 5：`Properties/launchSettings.json` — 新建调试配置

**文件**: `FlavorVault/Properties/launchSettings.json`（新建）

```json
{
  "profiles": {
    "Windows Machine": {
      "commandName": "Project",
      "nativeDebugging": false
    }
  }
}
```

### 说明

| 字段 | 说明 |
|------|------|
| `commandName: "Project"` | 直接运行项目 exe，不使用 MSIX 打包部署。匹配 `WindowsPackageType=None` |
| `nativeDebugging: false` | 不启用原生代码调试 |

**注意**: 如果设置 `commandName: "MsixPackage"`，会报"需要先部署"错误，因为 `WindowsPackageType=None` 不生成 MSIX 包。

---

## 修改 6：清理缓存

修改完成后，建议清理构建缓存让 Visual Studio 重新识别：

```powershell
# 删除 obj 目录
Remove-Item -Recurse -Force FlavorVault\obj
# 删除 .vs 隐藏目录（如果存在）
Remove-Item -Recurse -Force .vs
```

然后在 Visual Studio 中重新加载解决方案。

---

## 命令行验证构建

```powershell
dotnet build -f net9.0-windows10.0.19041.0
```

构建成功（0 error）即表示 Windows 端适配完成。308 个 XAML binding 警告为 MAUI 编译优化提示，不影响运行。

---

## 代码中已有的平台兼容处理

项目代码中已有以下平台兼容写法，Windows 端不需要额外修改：

| 服务 | 处理方式 |
|------|----------|
| `HapticService` | `#if ANDROID` 包裹 Android 原生振动代码，其他平台用 MAUI `Vibration.Default` |
| `CameraService` | `#if ANDROID` 包裹闪光灯代码，Windows 端跳过相机权限检查 |
| `SensorService` | 通过 `IsSupported` 检查和 `FeatureNotSupportedException` 处理无传感器情况 |
| `GeolocationService` | GPS 不可用时降级到默认北京坐标 |
| `SpeechToTextService` | `#if WINDOWS` 使用 WinRT API，`#if ANDROID` 使用 Android Intent |

---

## 文件修改汇总

| 文件 | 操作 | 说明 |
|------|------|------|
| `FlavorVault.csproj` | 修改 | 添加 Windows TFM、条件 OS 版本、WindowsPackageType |
| `Platforms/Windows/App.xaml` | 修改 | x:Class 命名空间改为 FlavorVault.WinUI.App |
| `Platforms/Windows/App.xaml.cs` | 修改 | 命名空间改为 FlavorVault.WinUI，添加 partial |
| `FlavorVault.sln` | 修改 | 补全 x64/x86/ARM64 平台配置 |
| `Properties/launchSettings.json` | 新建 | Windows 调试配置 |
