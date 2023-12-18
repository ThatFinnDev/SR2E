using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace SR2E.Library.Patches;


[HarmonyPatch(typeof(EconomyDirector), "InitModel")]
public static class EconomyPatch
{
    public static void Prefix(EconomyDirector __instance)
    {
        List<EconomyDirector.ValueMap> valueMaps = new List<EconomyDirector.ValueMap>();
        foreach (KeyValuePair<IdentifiableType, ModdedMarketData> marketData in marketData)
        {
            EconomyDirector.ValueMap valueMap = new EconomyDirector.ValueMap
            {
                FullSaturation = marketData.Value.SAT,
                Value = marketData.Value.VAL,
                Accept = marketData.Key.prefab.GetComponent<Identifiable>()
            };
            valueMaps.Add(valueMap);
        }
        __instance.BaseValueMap = HarmonyLib.CollectionExtensions.AddRangeToArray<EconomyDirector.ValueMap>(__instance.BaseValueMap.ToArray(), valueMaps.ToArray());
        /*
        // Same as in MarketPatch
        */
        /*
        try
        {
            List<EconomyDirector.ValueMap> list = __instance.BaseValueMap.ToList();
            List<EconomyDirector.ValueMap> listTwo = __instance.BaseValueMap.ToList();
            foreach (EconomyDirector.ValueMap entry in list)
                if(entry!=null)
                    foreach (IdentifiableType toRemove in removeMarketPlortEntries)
                        if(toRemove!=null)
                            if (entry.Accept.identType.ValidatableName == toRemove.ValidatableName)
                                listTwo.Remove(entry);
            __instance.BaseValueMap = new Il2CppReferenceArray<EconomyDirector.ValueMap>(0);
            foreach (EconomyDirector.ValueMap entry in listTwo)
                if (entry != null)
                    __instance.BaseValueMap.AddItem(entry);
        }
        catch { }*/
    }
}