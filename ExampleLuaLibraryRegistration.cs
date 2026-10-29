using System;
using System.Threading.Tasks;
using Lua;
using StationeersLua;
using UnityEngine;

namespace StationeersLuaAddonTemplate;

/// <summary>
/// Registers extra Lua API surface through <see cref="LuaLibraryRegistry"/>,
/// the same extension point ScriptedScreens uses for <c>ss</c> / UI bindings.
/// </summary>
/// <remarks>
/// Registration runs once at mod load. StationeersLua applies all registered callbacks
/// whenever a chip VM is prepared, passing <see cref="LuaLibraryContext"/> with
/// <see cref="LuaLibraryContext.Chip"/> and <see cref="LuaLibraryContext.State"/>.
///
/// The API lives in a global table of the mod's own, like ScriptedScreens' <c>ss</c>: the <c>ic</c> table belongs to
/// StationeersLua (a library that adds to it is named in a warning in the log). This example exposes
/// <c>addon_template.hello()</c> returning a short string, and <c>addon_template.device_name(id)</c>, which shows the three addon helpers (StationeersLua 1.2):
/// <see cref="LuaChipDevices"/> (reach only what the chip can), <see cref="LuaInstructionBudget"/> (charge a call's
/// cost) and <see cref="LuaReference"/> (completions, hover and a reference-panel tab).
/// Replace the table name, methods, and registry name with your mod's API.
/// </remarks>
internal static class ExampleLuaLibraryRegistration
{
    /// <summary>
    /// Call from your plugin <c>OnLoaded</c> (or another early entrypoint) so bindings exist before chips run.
    /// </summary>
    internal static void TryRegister()
    {
        try
        {
            LuaLibraryRegistry.RegisterLibrary("StationeersLuaAddonTemplate.Example", RegisterExampleBindings);
            LuaLibraryRegistry.RegisterSyntax("StationeersLuaAddonTemplate.Example", RegisterExampleSyntax);
            RegisterExampleReference();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[{PluginInfo.PLUGIN_NAME}] Lua library registration failed: {ex.Message}");
        }
    }

    private static void RegisterExampleSyntax(LuaSyntaxTokenContext syntax)
    {
        // Token names for StationeersLua's syntax highlighter / editor integration.
        // Color index 0 = default namespace style (ic, mem, etc.), 12 = function-like (matches common builtins like print, require, etc.), 15 = language keywords (if, then, else, etc.).
        // StationeersLua v0.9 will add a new enum for these values for simplicity.

        // Add tokens for your mod's API.
        syntax.Add("addon_template", 0U); // StationeersLua v0.9 will use LuaSyntaxColorIndex.Default
        syntax.Add("hello", 12U); // StationeersLua v0.9 will use LuaSyntaxColorIndex.Function
        syntax.Add("device_name", LuaSyntaxColorIndex.Function);
    }

    /// <summary>
    /// The reference: a tab in the in-game reference panel, plus the editor's completions and hover and the LSP.
    /// A larger API ships per-language XML instead: <c>LuaReference.RegisterLocalizedXml(lang => ...)</c>.
    /// </summary>
    private static void RegisterExampleReference()
    {
        LuaReference.RegisterSection("Addon Template", new[]
        {
            new LuaReferenceCategory("Examples", new[]
            {
                new LuaReferenceEntry("addon_template.hello", "addon_template.hello()",
                    "Returns the addon's name and version.", "print(addon_template.hello())"),
                new LuaReferenceEntry("addon_template.device_name", "addon_template.device_name(id)",
                    "The name of a device this chip can reach, or nil and a reason.",
                    "local name, why = addon_template.device_name(device_id(0))"),
            }),
        });
    }

    private static void RegisterExampleBindings(LuaLibraryContext context)
    {
        // Your mod's own global table (created on first use). Name it after your mod; never add to ic.
        var addonRoot = context.GetOrCreateGlobalTable("addon_template");

        // Create a function for your mod's API.
        addonRoot["hello"] = new LuaFunction("hello", (luaContext, ct) =>
        {
            luaContext.GetReturnBuffer(1)[0] = $"{PluginInfo.PLUGIN_NAME} ({PluginInfo.PLUGIN_VERSION})";
            return new ValueTask<int>(1);
        });

        // A device by ReferenceId, reached exactly as read_id would, with the call's cost charged to the script.
        var chip = context.Chip;
        addonRoot["device_name"] = new LuaFunction("device_name", (luaContext, ct) =>
        {
            var result = luaContext.GetReturnBuffer(2);
            if (!LuaInstructionBudget.TryCharge(luaContext.State, 50))
            {
                result[0] = LuaValue.Nil;
                result[1] = "call limit"; // refused before the budget ran out: nothing was charged
                return new ValueTask<int>(2);
            }

            var device = LuaChipDevices.ResolveDeviceById(chip, (long)luaContext.GetArgument<double>(0));
            if (device == null)
            {
                result[0] = LuaValue.Nil;
                result[1] = "not reachable from this chip";
                return new ValueTask<int>(2);
            }

            result[0] = device.DisplayName;
            return new ValueTask<int>(1);
        });
    }
}
