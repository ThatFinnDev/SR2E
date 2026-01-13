using System.Linq;
using Il2CppMonomiPark.SlimeRancher.UI;
using SR2E.Prism.Lib;
using SR2E.Storage;

namespace SR2E.Prism.Patches;

[PrismPatch()]
[HarmonyPriority(-9999999)]
[HarmonyPatch(typeof(MarketUI))]
internal static class MarketUIPatch
{
    [HarmonyPatch(nameof(MarketUI.Start))]
    [HarmonyPriority(-9999999)]
    [HarmonyPrefix]
    public static void Prefix(MarketUI __instance)
    {
        List<PlortEntry> plortEntries = new List<PlortEntry>(__instance._config._plorts);
        foreach (var entry in __instance._config._plorts)
            foreach (var type in PrismShortcuts.removeMarketPlortEntries)
            {
                if (entry.IdentType.ReferenceId == type.ReferenceId)
                {
                    plortEntries.Remove(entry);
                    break;
                }
            }
        foreach (var pair in PrismShortcuts.marketPlortEntries)
            if (!pair.Value)
                plortEntries.Add(pair.Key);
        
        __instance._config._plorts = plortEntries.Take(34).ToArray();
        
        PrismLibMarket.TryRefreshMarketData();
    }   
    
    [HarmonyPriority(-9999999)]
    [HarmonyPatch(nameof(MarketUI.Start))]
    [HarmonyPostfix]
    public static void Postfix(MarketUI __instance)
    {
        __instance._config._plorts = __instance._config._plorts.Take(34).ToArray();
    }
}