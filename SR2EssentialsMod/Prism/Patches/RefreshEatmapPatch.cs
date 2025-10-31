using Cotton;
using SR2E.Storage;

namespace SR2E.Prism.Patches;

[PrismPatch()]
[HarmonyPatch(typeof(SlimeDiet), nameof(SlimeDiet.RefreshEatMap))]
internal class RefreshEatmapPatch
{
    public static void Postfix(SlimeDiet __instance, SlimeDefinitions definitions, SlimeDefinition definition)
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