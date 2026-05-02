using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StationeersLua;
using UnityEngine;

namespace StationeersLuaAddonTemplate;

/// <summary>
/// Registers optional MCP documentation and a sample tool on StationeersLua's HTTP/MCP server
/// via <see cref="LuaMcpRegistry"/> (same extension model as <see cref="LuaLibraryRegistry"/>).
/// </summary>
internal static class ExampleMcpRegistration
{
    internal static void TryRegister()
    {
        try
        {
            LuaMcpRegistry.RegisterDocumentationSearchScope("addon_template", "stationeers://addon_template/");

            LuaMcpRegistry.RegisterDocumentationResource(
                "stationeers://addon_template/mcp",
                "StationeersLua addon template (MCP)",
                "Short overview of this sample mod's MCP registration pattern for AI editors.",
                "text/markdown",
                BuildMcpReadmeMarkdown);

            LuaMcpRegistry.RegisterTool(
                "addon_template_echo",
                "Echoes a short message from the MCP client for sanity checks. Does not touch game state.",
                @"{""type"":""object"",""properties"":{""message"":{""type"":""string"",""description"":""Text to echo back""}},""required"":[""message""]}",
                HandleEcho);

            LuaMcpRegistry.RequestDocumentationSearchIndexRefresh();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[{PluginInfo.PLUGIN_NAME}] MCP registration failed: {ex.Message}");
        }
    }

    private static string BuildMcpReadmeMarkdown()
    {
        return $@"# StationeersLua addon template — MCP

This mod registers MCP resources and tools through **`LuaMcpRegistry`** in **`StationeersLua`** (released **`StationeersLua.dll`**).

- **Documentation:** `stationeers://addon_template/mcp` (this page), exposed to `resources/read` and `search_docs`.
- **Tools:** `addon_template_echo` — trivial JSON echo for wiring tests.

Ship real docs as markdown strings, lazy-loaded delegates, or files under your mod folder. Prefer URIs like `stationeers://your_mod_id/...` so names stay unique.
";
    }

    private static string HandleEcho(JObject? p)
    {
        var msg = (string?)p?["message"] ?? string.Empty;
        return new JObject
        {
            ["ok"] = true,
            ["echo"] = msg,
            ["from"] = PluginInfo.PLUGIN_NAME,
            ["version"] = PluginInfo.PLUGIN_VERSION
        }.ToString(Formatting.None);
    }
}
