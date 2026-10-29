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
