using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.Localization;
using static CottonLibrary.Library;

namespace CottonLibrary.Patches;

[HarmonyPatch(typeof(TargetingUI), nameof(TargetingUI.Update))]
public class TargetingPatch
{
    static bool injectedStrings = false;
    static SlimeEatStrings eatStrings;
    
    static void Prefix(TargetingUI __instance)
    {
        eatStrings = __instance.SlimeEatStrings;

        if (eatStrings != null && !injectedStrings)
        {
            if (eatStrings._foodGroupStringMap == null)
                return;
            
            foreach (var group in customGroups)
                if (group._localizedName != null && group._isFood)
                    eatStrings._foodGroupStringMap.TryAdd(group, group._localizedName);
            injectedStrings = true;
        }
    }
}