using Assets.Scripts;
using BepInEx.Logging;
using StationeersMods.Interface;
using UnityEngine;

namespace StationeersLuaAddonTemplate;

/// <summary>
/// LaunchPad entrypoint: registers Lua bindings with StationeersLua's <see cref="LuaLibraryRegistry"/>.
/// </summary>
[StationeersMod(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION_CONST)]
public sealed class StationeersLuaAddonTemplatePlugin : ModBehaviour
{
    private static bool _initialized;

    internal static ManualLogSource? Log { get; private set; }

    /// <inheritdoc />
    public override void OnLoaded(ContentHandler contentHandler)
    {
        base.OnLoaded(contentHandler);

        if (_initialized)
            return;

        _initialized = true;

        if (contentHandler == null)
            throw new System.ArgumentNullException(nameof(contentHandler));

        try
        {
            Log = new ManualLogSource(PluginInfo.PLUGIN_NAME);
            try
            {
                BepInEx.Logging.Logger.Sources.Add(Log);
            }
            catch
            {
            }

            ExampleLuaLibraryRegistration.TryRegister();
            Log.LogInfo("StationeersLua addon template: registered example Lua library.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
    }
}
