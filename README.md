# FlavorVault Windows Adaptation Guide

This document records the complete steps to adapt a .NET MAUI project originally targeting only Android to also run on Windows desktop.

---

## Background

The original project only targeted Android, with `TargetFrameworks` set to `net9.0-android`, making it impossible to compile and run on Windows. The following modifications are required for Windows desktop debugging.

---

## Change 1: `.csproj` — Add Windows Target Framework

**File**: `FlavorVault/FlavorVault.csproj`

### Before

```xml
<PropertyGroup>
    <TargetFrameworks>net9.0-android</TargetFrameworks>
    <SupportedOSPlatformVersion>21</SupportedOSPlatformVersion>
</PropertyGroup>
```

### After

```xml
<PropertyGroup>
    <TargetFrameworks>net9.0-android;net9.0-windows10.0.19041.0</TargetFrameworks>

    <!-- Conditionally split SupportedOSPlatformVersion by platform -->
    <!-- Android API 21 (Android 5.0) -->
    <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'">21</SupportedOSPlatformVersion>
    <!-- Windows 10.0.19041.0 (Windows 10 2004) -->
    <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'windows'">10.0.19041.0</SupportedOSPlatformVersion>

    <!-- Allow unpackaged debugging (otherwise MSIX packaging errors occur) -->
    <WindowsPackageType>None</WindowsPackageType>
</PropertyGroup>
```

### Explanation

| Change | Reason |
|--------|--------|
| `TargetFrameworks` add `net9.0-windows10.0.19041.0` | Tell the compiler to generate a Windows desktop target |
| `SupportedOSPlatformVersion` split by condition | Android's `21` is an API level, Windows needs `10.0.19041.0` — they can't share the same value, otherwise NETSDK1135 error |
| `WindowsPackageType=None` | Don't use MSIX packaging, run directly as exe for debugging. Otherwise "deployment required" error occurs |

---

## Change 2: `Platforms/Windows/App.xaml` — Fix Namespace Conflict

**File**: `FlavorVault/Platforms/Windows/App.xaml`

### Before

```xml
<maui:MauiWinUIApplication
    x:Class="FlavorVault.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:maui="using:Microsoft.Maui"
    xmlns:local="using:FlavorVault">
</maui:MauiWinUIApplication>
```

### After

```xml
<maui:MauiWinUIApplication
    x:Class="FlavorVault.WinUI.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:maui="using:Microsoft.Maui"
    xmlns:local="using:FlavorVault">
</maui:MauiWinUIApplication>
```

### Explanation

The cross-platform project already has a `FlavorVault.App` (inheriting `Application`), and the Windows platform `App` inherits `MauiWinUIApplication`. Having the same name with different base classes causes a conflict. Changing the Windows side to `FlavorVault.WinUI.App` resolves this.

---

## Change 3: `Platforms/Windows/App.xaml.cs` — Fix Class Definition

**File**: `FlavorVault/Platforms/Windows/App.xaml.cs`

### Before

```csharp
namespace FlavorVault;

public class App : MauiWinUIApplication
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
```

### After

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

### Explanation

| Change | Reason |
|--------|--------|
| Namespace changed to `FlavorVault.WinUI` | Matches `x:Class="FlavorVault.WinUI.App"` |
| Added `partial` modifier | XAML code generator needs partial class to produce `.g.cs` files |
| `OnLaunched` override | Ensures correct Windows launch lifecycle initialization |

---

## Change 4: `.sln` — Add Platform Configurations

**File**: `FlavorVault.sln`

### Explanation

The original `.sln` only had `Any CPU` platform configuration. Windows desktop requires specific CPU architectures (`x64`/`x86`/`ARM64`). Ensure `GlobalSection(ProjectConfigurationPlatforms)` includes the following:

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

Each configuration must have corresponding `.ActiveCfg`, `.Build.0`, `.Deploy.0` mappings.

---

## Change 5: `Properties/launchSettings.json` — Create Debug Configuration

**File**: `FlavorVault/Properties/launchSettings.json` (new file)

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

### Explanation

| Field | Description |
|-------|-------------|
| `commandName: "Project"` | Run project exe directly without MSIX packaging. Matches `WindowsPackageType=None` |
| `nativeDebugging: false` | Don't enable native code debugging |

**Note**: Setting `commandName: "MsixPackage"` will cause a "deployment required" error because `WindowsPackageType=None` doesn't generate an MSIX package.

---

## Change 6: Clean Cache

After completing the changes, it's recommended to clean the build cache for Visual Studio to re-detect:

```powershell
# Delete obj directory
Remove-Item -Recurse -Force FlavorVault\obj
# Delete .vs hidden directory (if exists)
Remove-Item -Recurse -Force .vs
```

Then reload the solution in Visual Studio.

---

## Command Line Build Verification

```powershell
dotnet build -f net9.0-windows10.0.19041.0
```

A successful build (0 errors) means the Windows adaptation is complete. 308 XAML binding warnings are MAUI compile optimization hints and do not affect runtime.

---

## Existing Platform Compatibility in Code

The project already has platform-compatible code in place. No additional modifications needed for Windows:

| Service | Handling |
|---------|----------|
| `HapticService` | `#if ANDROID` wraps Android native vibration code, other platforms use MAUI `Vibration.Default` |
| `CameraService` | `#if ANDROID` wraps flash code, Windows skips camera permission check |
| `SensorService` | Uses `IsSupported` check and `FeatureNotSupportedException` for missing sensors |
| `GeolocationService` | Falls back to default Beijing coordinates when GPS unavailable |
| `SpeechToTextService` | `#if WINDOWS` uses WinRT API, `#if ANDROID` uses Android Intent |

---

## File Change Summary

| File | Action | Description |
|------|--------|-------------|
| `FlavorVault.csproj` | Modified | Add Windows TFM, conditional OS version, WindowsPackageType |
| `Platforms/Windows/App.xaml` | Modified | x:Class namespace changed to FlavorVault.WinUI.App |
| `Platforms/Windows/App.xaml.cs` | Modified | Namespace changed to FlavorVault.WinUI, added partial |
| `FlavorVault.sln` | Modified | Added x64/x86/ARM64 platform configurations |
| `Properties/launchSettings.json` | Created | Windows debug configuration |
