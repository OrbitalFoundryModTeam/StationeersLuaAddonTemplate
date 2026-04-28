using Lua;

namespace StationeersLuaAddonTemplate;

/// <summary>
/// Small helpers for attaching nested tables to the Lua <c>ic</c> namespace or globals,
/// matching the pattern used inside StationeersLua's runtime builtins.
/// </summary>
internal static class LuaBindingHelpers
{
    /// <summary>
    /// Gets an existing child table or creates an empty <see cref="LuaTable"/> at <paramref name="key"/>.
    /// </summary>
    internal static LuaTable GetOrCreateSubTable(LuaTable parent, string key)
    {
        if (parent[key].TryRead<LuaTable>(out var existing) && existing != null)
            return existing;

        var created = new LuaTable();
        parent[key] = created;
        return created;
    }
}
