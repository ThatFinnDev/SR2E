using HarmonyLib;
using Il2Cpp;

namespace CottonLibrary.Patches;

public class RefreshEatmapPatch
{
    [HarmonyPatch(typeof(SlimeDiet), nameof(SlimeDiet.RefreshEatMap))]
    [HarmonyPostfix]
    public static void AddCustomEats(SlimeDiet __instance, SlimeDefinitions definitions, SlimeDefinition definition)
    {
        if (customEatmaps.TryGetValue(definition, out var eatMap))
        {
            foreach (var eat in eatMap)
            {
                __instance.EatMap.Add(eat);
            }
        }
    }
}