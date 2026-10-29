using BepInEx;
using HarmonyLib;
using UnityEngine;
using BepInEx.Logging;
using BepInEx.Configuration;
using Pigeon.Movement;
using System;

namespace MyMycoModRoot;

[MycoMod(null, ModFlags.IsClientSide)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private Harmony _harmony;
    internal new static ManualLogSource Logger;
    private ConfigEntry<bool> _northUp;
    internal static bool IsNorthUp { get; private set; }
    private ConfigEntry<short> _zoomAmount;
    internal static int GetZoomAmount { get; private set; }

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        _northUp = Config.Bind(
            "General",
            "North Up",
            true,
            "If set to true, the player's icon will rotate instead of the world around the player.");
        IsNorthUp = _northUp.Value;
        _northUp.SettingChanged += (_, _) => IsNorthUp = _northUp.Value;

        _zoomAmount = Config.Bind(
            "General",
            "Zoom Amount",
            (short)80,
            new ConfigDescription(
                "Changes how zoomed in the minimap is.",
                new AcceptableValueRange<short>(1, 512)
            )
        );
        GetZoomAmount = _zoomAmount.Value;
        _zoomAmount.SettingChanged += (_, _) => { GetZoomAmount = Mathf.Clamp(_zoomAmount.Value, 1, 512); };


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
        if (!Plugin.IsNorthUp)
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
    // [HarmonyPrefix]
    // private static void Prefix(Minimap __instance)
    // {
    //     if (!Plugin.IsNorthUp)
    //         return;
    //
    //     
    // }

    [HarmonyPostfix]
    private static void Postfix(Minimap __instance)
    {
        Plugin.Logger.LogInfo("__instance.transform.localScale bfr: " + __instance.transform.localScale);
        __instance.transform.localScale = new Vector3(2f, 2f, 2f);
        Plugin.Logger.LogInfo("__instance.transform.localScale aft: " + __instance.transform.localScale);
        
        // Changes map zoom amount
        __instance.minimapWorldRadius = Plugin.GetZoomAmount;
        
        if (!Plugin.IsNorthUp)
            return;

        // Stops the map image from rotating
        __instance.mapRenderer.transform.localRotation = Quaternion.identity;
        // Enables player icon to rotate
        __instance.playerPointer.rectTransform.localRotation =
            Quaternion.Euler(0.0f, 0.0f, -PlayerLook.Rotation.eulerAngles.y);
    }
}