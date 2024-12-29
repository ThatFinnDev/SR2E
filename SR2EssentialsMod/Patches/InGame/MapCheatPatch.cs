using Il2CppMonomiPark.SlimeRancher.UI.Map;

namespace SR2E.Patches.InGame;

[HarmonyPatch(typeof(MapUI), nameof(MapUI.Start))]
internal static class MapCheatPatch
{
    internal static void Postfix(MapUI __instance)
    {
        if (SR2ECheatMenu.removeFog)
        {
            __instance.gameObject.getObjRec<GameObject>("fog_static").SetActive(false);
            __instance.gameObject.getObjRec<GameObject>("zone_fog_areas").SetActive(false);
        }
    }
}