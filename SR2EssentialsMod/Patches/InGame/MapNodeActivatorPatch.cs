/*using Il2CppMonomiPark.SlimeRancher.UI.Map;

namespace SR2E.Patches.InGame;

[HarmonyPatch(typeof(MapNodeActivator), nameof(MapNodeActivator.Start))]
public static class MapNodeActivatorPatch
{
    public static List<MapNodeActivator> mapNodeActivators = new List<MapNodeActivator>();
    public static void Postfix(MapNodeActivator __instance)
    {
        MelonLogger.Msg(__instance.name);
        mapNodeActivators.Add(__instance);
        mapNodeActivators.RemoveAll(item => item == null);
    }
}*/