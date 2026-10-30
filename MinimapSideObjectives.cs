using HarmonyLib;

namespace MyMycoModRoot;

[HarmonyPatch]
internal static class MinimapSideObjectivesPatch
{
    [HarmonyPatch(
        typeof(Waypoint),
        nameof(Waypoint.EncounterWaypoint)
    )]
    [HarmonyPostfix]
    private static void Postfix(ref Waypoint __result)
    {
        if (!Configs.NorthUp.Value)
            return;
        
        if(__result.NearbyIconDistance > 0.0)
            __result.UseNearbyIcon = true;
    }
}