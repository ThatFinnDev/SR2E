using HarmonyLib;
using Il2Cpp;
using SR2E.Storage;

namespace CottonLibrary.Patches;

[LibraryPatch()]
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