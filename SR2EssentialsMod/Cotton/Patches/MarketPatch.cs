using Il2CppMonomiPark.SlimeRancher.UI;
using System.Linq;
using SR2E.Storage;

namespace SR2E.Cotton.Patches;

[LibraryPatch()]
[HarmonyPatch(typeof(MarketUI))]
public static class MarketPatch
{
    [HarmonyPatch(nameof(MarketUI.Start))]
    [HarmonyPrefix]
    public static void Prefix(MarketUI __instance)
    {
        List<PlortEntry> plortEntries = new List<PlortEntry>(__instance._config._plorts);
        foreach (var entry in __instance._config._plorts)
            foreach (var type in CottonLibrary.removeMarketPlortEntries)
            {
                if (entry.IdentType.ReferenceId == type.ReferenceId)
                {
                    plortEntries.Remove(entry);
                    break;
                }
            }
        foreach (var pair in CottonLibrary.marketPlortEntries)
            if (!pair.Value)
                plortEntries.Add(pair.Key);
        
        __instance._config._plorts = plortEntries.Take(34).ToArray();
        
        CottonLibrary.Market.TryRefreshMarketData();
    }   
    
    [HarmonyPriority(HarmonyLib.Priority.Last)]
    [HarmonyPatch(nameof(MarketUI.Start))]
    [HarmonyPostfix]
    public static void Postfix(MarketUI __instance)
    {
        __instance._config._plorts = __instance._config._plorts.Take(34).ToArray();
    }
}