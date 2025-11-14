using SR2E.Prism.Lib;
using SR2E.Storage;

namespace SR2E.Prism.Patches;

[PrismPatch()]
[HarmonyPatch(typeof(SlimeDiet), nameof(SlimeDiet.RefreshEatMap))]
internal class RefreshEatmapPatch
{
    public static void Postfix(SlimeDiet __instance, SlimeDefinitions definitions, SlimeDefinition definition)
    {
        if (PrismLibDiet.customEatmaps.TryGetValue(definition, out var eatMap))
        {
            foreach (var eat in eatMap)
            {
                if(eat.Value)
                    if(!PrismLibDiet._CarefulCheck(__instance.EatMap,eat.Key)) continue;
                __instance.EatMap.Add(eat.Key);
            }
        }
    }
}