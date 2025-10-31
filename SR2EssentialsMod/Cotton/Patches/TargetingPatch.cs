using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.Localization;
using SR2E.Cotton;
using SR2E.Storage;

namespace Cotton.Patches;

[LibraryPatch()]
[HarmonyPatch(typeof(TargetingUI), nameof(TargetingUI.Start))]
public class TargetingPatch
{

    [HarmonyPatch(typeof(TargetingUI), nameof(TargetingUI.ClearAllDisplayInfo))]
    static void ClearAllDisplayInfo(TargetingUI __instance) => Start(__instance);
    [HarmonyPatch(typeof(TargetingUI), nameof(TargetingUI.Start))]
    static void Start(TargetingUI __instance)
    {
        var eatStrings = __instance.SlimeEatStrings;

        if (eatStrings == null) return;
        if (eatStrings._foodGroupStringMap == null) return;
            
        foreach (var group in gameContext.LookupDirector._allIdentifiableTypeGroups.items)
            if (group._localizedName != null && group._isFood)
                eatStrings._foodGroupStringMap.TryAdd(group, group._localizedName);
    }
}