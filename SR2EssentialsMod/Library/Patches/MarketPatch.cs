using Il2CppMonomiPark.SlimeRancher.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Library.Patches;

[HarmonyPatch(typeof(MarketUI), nameof(MarketUI.Start))]
public static class MarketPatch
{
    public static void Prefix(MarketUI __instance)
    {
        List<MarketUI.PlortEntry> marketPlortEntriesList = new List<MarketUI.PlortEntry>();
        foreach (var pair in marketPlortEntries)
            if (!pair.Value)
                marketPlortEntriesList.Add(pair.Key);

        __instance.plorts = (from x in __instance.plorts
                             where !marketPlortEntriesList.Exists
                                 ((MarketUI.PlortEntry y) => y == x)
                             select x).ToArray();

        //To someone that finds that code, ik it looks like garbage, because it is,
        //but i've tried for too long to make it work. This works some I just gonna leave it
        /*

        List<MarketUI.PlortEntry> list = __instance.plorts.ToList();
        List<MarketUI.PlortEntry> listTwo = __instance.plorts.ToList();
        foreach (MarketUI.PlortEntry entry in list)
            foreach (IdentifiableType toRemove in removeMarketPlortEntries)
                if (entry.identType.ValidatableName == toRemove.ValidatableName)
                    listTwo.Remove(entry);
        __instance.plorts = new Il2CppReferenceArray<MarketUI.PlortEntry>(0);
        foreach (MarketUI.PlortEntry entry in listTwo)
            if (entry != null)
                __instance.plorts.AddItem(entry);

        */

        __instance.plorts = __instance.plorts.ToArray<MarketUI.PlortEntry>().AddRangeToArray(marketPlortEntriesList.ToArray());
        __instance.plorts = __instance.plorts.Take(33).ToArray();

    }
}