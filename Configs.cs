using UnityEngine;
using BepInEx.Configuration;

namespace MyMycoModRoot;

internal static class Configs
{
    public static ConfigEntry<bool> NorthUp { get; private set; }
    public static ConfigEntry<short> ZoomAmount { get; private set; }
    // public static ConfigEntry<float> MapScale { get; private set; }

    internal static void Initialize(ConfigFile configFile)
    {
        NorthUp = configFile.Bind(
            "General",
            "North Up",
            true,
            "If True, the player's icon will rotate instead of the world around the player.");
        
        ZoomAmount = configFile.Bind(
            "General",
            "Zoom Amount",
            (short)80,
            new ConfigDescription(
                "Changes how zoomed in the minimap is. (lower = more zoomed in)",
                new AcceptableValueRange<short>(1, 512)
            ));
        ZoomAmount.SettingChanged += (_, _) => { ZoomAmount.Value = (short)Mathf.Clamp(ZoomAmount.Value, 1, 512); };
        
        // MapScale = configFile.Bind(
        //     "General",
        //     "Map Scale",
        //     1.0f,
        //     new ConfigDescription(
        //         "Changes the minimap size.",
        //         new AcceptableValueRange<float>(0.0f, 5.0f)
        //     ));
        // MapScale.SettingChanged += (_, _) => { MapScale.Value = Mathf.Clamp(MapScale.Value, 0.0f, 5.0f); };
    }
}