# StationeersLua addon template

A minimal **[Stationeers](https://store.steampowered.com/app/544550/Stationeers/)** mod sample that extends **[StationeersLua](https://steamcommunity.com/sharedfiles/filedetails/?id=3659911735)** using the same public hooks as mods like **[ScriptedScreens](https://steamcommunity.com/sharedfiles/filedetails/?id=3666779631)**. It links against `StationeersLua.dll` on your PC.

## Community

[![Discord — Orbital Foundry Mod Team](https://img.shields.io/badge/Discord-Orbital%20Foundry-5865F2?logo=discord&logoColor=white)](https://discord.gg/HxvySSu5G3) [![Ko-fi — support](https://img.shields.io/badge/Support-Ko--fi-FF5E5B?logo=ko-fi&logoColor=white)](https://ko-fi.com/G2G61S5ZB5)

## Requirements

- **Stationeers** installed (used for managed assemblies and LaunchPad paths).
- **StationeersLua** subscribed or copied into your Stationeers mods folder (see resolution below).
- **Visual Studio** or **.NET SDK** to build `StationeersLuaAddonTemplate.sln`.

## First-time setup

1. Copy **`Stationeers.VS.User.props.example`** to **`Stationeers.VS.User.props`** (gitignored) in this folder.
2. Set **`SteamLibraryDirectory`** to the folder that **contains** the `Stationeers` game directory, usually the `steamapps\common` segment, for example:
   - `D:\Steam\steamapps\common`
   - `G:\SteamLibrary\steamapps\common`

## Where `StationeersLua.dll` is resolved from

If you set **`StationeersLuaAssemblyPath`** in **`Stationeers.VS.User.props`**, that path is always used.

Otherwise MSBuild picks the first path that exists:

1. **`Documents\My Games\Stationeers\mods\StationeersLua\StationeersLua.dll`**
2. **`{SteamLibraryDirectory}\..\workshop\content\544550\3659911735\StationeersLua.dll`** (Steam Workshop layout for app `544550` and StationeersLua item `3659911735`, same as **`About/About.xml`**)

You can override **`StationeersSteamAppId`** / **`StationeersLuaWorkshopId`** in User.props only if you need a non-default Workshop folder.

## Build and deploy

Open **`StationeersLuaAddonTemplate.sln`**, configuration **Debug** or **Release**, platform **x64**, then build.

The project copies the output DLL (and Debug PDB when applicable) plus **`About`**, **`GameData`**, and **`Assets`** into:

`Documents\My Games\Stationeers\mods\StationeersLuaAddonTemplate\`

Launch the game with **StationeersLua** enabled. **`About/About.xml`** uses LaunchPad **`OrderAfter`** so this mod sorts after StationeersLua when both are installed.

## What this sample adds

- **`ic.addon_template.hello()`** returning a short string from **`ExampleLuaLibraryRegistration.cs`**.
- Optional syntax tokens for the chip editor highlighter (same pattern as full mods).

Example on a chip:

```lua
local msg = ic.addon_template.hello()
```

## Forking / shipping your own mod

1. Rename the project folder, **`ModName`** / **`RootNamespace`** in the `.csproj`, **`PLUGIN_GUID`** and **`PLUGIN_NAME`** in **`PluginInfo.cs`**, and registration name strings in your Lua library code.
2. Edit **`About/About.xml`** (name, author, version, **`ModID`**) to match what you want to publish.
3. Replace **`ExampleLuaLibraryRegistration`** with your API.
