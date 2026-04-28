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
/// <see cref="LuaLibraryContext.Chip"/>, <see cref="LuaLibraryContext.State"/>, and
/// <see cref="LuaLibraryContext.Ic"/>.
///
/// This example exposes <c>ic.addon_template.hello()</c> returning a short string.
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
    }

    private static void RegisterExampleBindings(LuaLibraryContext context)
    {
        if (context.Ic == null)
            return;

        // Create a sub-table for your mod's API.
        var addonRoot = LuaBindingHelpers.GetOrCreateSubTable(context.Ic, "addon_template");

        // Create a function for your mod's API.
        addonRoot["hello"] = new LuaFunction("hello", (luaContext, ct) =>
        {
            luaContext.GetReturnBuffer(1)[0] = $"{PluginInfo.PLUGIN_NAME} ({PluginInfo.PLUGIN_VERSION})";
            return new ValueTask<int>(1);
        });

        // Example of a global table (like ScriptedScreens' <c>ss</c>) if you prefer top-level names:
        // var env = context.State.Environment;
        // if (!env["myglobal"].TryRead<LuaTable>(out var g) || g == null) { g = new LuaTable(); env["myglobal"] = g; }
    }
}
