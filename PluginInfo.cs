namespace StationeersLuaAddonTemplate;

/// <summary>
/// Mod identity for LaunchPad. Replace GUID and name when you fork this template.
/// </summary>
/// <remarks>
/// <see cref="PLUGIN_VERSION_CONST"/> is generated at compile time from the csproj
/// <c>Version</c> property (see <c>GenerateVersionConst</c> target in the csproj).
/// </remarks>
internal static partial class PluginInfo
{
    /// <summary>Stable mod identity (Harmony ID, Workshop metadata, etc.).</summary>
    internal const string PLUGIN_GUID = "zedle.stationeers.lua.addontemplate";

    /// <summary>Human-readable mod name for logs and the mod list.</summary>
    internal const string PLUGIN_NAME = "StationeersLua Addon Template";

    internal static readonly string PLUGIN_VERSION =
        typeof(PluginInfo).Assembly.GetName().Version?.ToString() ?? PLUGIN_VERSION_CONST;
}
