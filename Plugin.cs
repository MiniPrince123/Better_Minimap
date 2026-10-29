using BepInEx;
using HarmonyLib;
using UnityEngine;
using BepInEx.Logging;
using Pigeon.Movement;

namespace MyMycoModRoot;

[MycoMod(null, ModFlags.IsClientSide)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    // public const string PluginGUID = MyPluginInfo.PLUGIN_GUID;
    // public const string PluginName = MyPluginInfo.PLUGIN_NAME;
    // public const string PluginVersion = MyPluginInfo.PLUGIN_VERSION;

    private Harmony _harmony;
    internal new static ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} ({MyPluginInfo.PLUGIN_VERSION}) is loaded!");

        Configs.Initialize(Config);
        Logger.LogInfo($"Config initialized: NorthUp={Configs.NorthUp.Value}, ZoomAmount={Configs.ZoomAmount.Value}");

        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        _harmony.PatchAll();
        Logger.LogInfo("Harmony patches applied");
    }

    private void OnDestroy()
    {
        _harmony?.UnpatchSelf();
    }
}

[HarmonyPatch(
    typeof(Minimap),
    nameof(Minimap.UpdatePing)
)]
internal static class MinimapPingsPatch
{
    [HarmonyPrefix]
    private static void Prefix(ref Quaternion rotation)
    {
        if (!Configs.NorthUp.Value)
            return;

        // Stops waypoints/pings from rotating
        rotation = Quaternion.identity;
    }
}

[HarmonyPatch(
    typeof(Minimap),
    nameof(Minimap.UpdateMinimap)
)]
internal static class MinimapRendererPatch
{
    [HarmonyPostfix]
    private static void Postfix(Minimap __instance)
    {
        // Changes map scale
        // __instance.transform.localScale = new Vector3(Configs.MapScale.Value, Configs.MapScale.Value, Configs.MapScale.Value);
        // __instance.transform.localPosition = new Vector3(Configs.MapScale.Value, Configs.MapScale.Value, Configs.MapScale.Value);
        // Plugin.Logger.LogInfo("GetTopLeftPos : " + __instance.GetTopLeftPos());
        // Plugin.Logger.LogInfo("transform.position : " + __instance.transform.position);
        // Plugin.Logger.LogInfo("transform.localPosition : " + __instance.transform.localPosition);
        
        // Changes map zoom amount
        __instance.minimapWorldRadius = Configs.ZoomAmount.Value;

        if (!Configs.NorthUp.Value)
            return;

        // Stops the map image from rotating
        __instance.mapRenderer.transform.localRotation = Quaternion.identity;
        // Enables player icon to rotate
        __instance.playerPointer.rectTransform.localRotation =
            Quaternion.Euler(0.0f, 0.0f, -PlayerLook.Rotation.eulerAngles.y);
    }
}