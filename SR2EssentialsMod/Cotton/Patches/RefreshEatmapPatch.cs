using Cotton;
using SR2E.Storage;

namespace SR2E.Cotton.Patches;

[LibraryPatch()]
public class RefreshEatmapPatch
{
    [HarmonyPatch(typeof(SlimeDiet), nameof(SlimeDiet.RefreshEatMap))]
    [HarmonyPostfix]
    public static void AddCustomEats(SlimeDiet __instance, SlimeDefinitions definitions, SlimeDefinition definition)
    {
        if (CottonSlimes.customEatmaps.TryGetValue(definition, out var eatMap))
        {
            foreach (var eat in eatMap)
            {
                __instance.EatMap.Add(eat);
            }
        }
    }
}