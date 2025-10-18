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
        List<PlortEntry> marketPlortEntriesList = new List<PlortEntry>();
        foreach (var pair in CottonLibrary.marketPlortEntries)
            if (!pair.Value)
                marketPlortEntriesList.Add(pair.Key);

        __instance._config._plorts = (from x in __instance._config._plorts
                             where !marketPlortEntriesList.Exists
                                 ((y) => y == x)
                             select x).ToArray();


        try
        {
            __instance._config._plorts = (from x in __instance._config._plorts
                                 where !CottonLibrary.removeMarketPlortEntries.Exists((IdentifiableType y) => y.name != x.IdentType.name)
                                 select x).ToArray();
        }
        catch { }



        __instance._config._plorts = __instance._config._plorts.ToArray().AddRangeToArray(marketPlortEntriesList.ToArray());
        __instance._config._plorts = __instance._config._plorts.Take(34).ToArray();

    }   
    
    [HarmonyPriority(HarmonyLib.Priority.Last)]
    [HarmonyPatch(nameof(MarketUI.Start))]
    [HarmonyPostfix]
    public static void Postfix(MarketUI __instance)
    {
        __instance._config._plorts = __instance._config._plorts.Take(34).ToArray();
    }
}