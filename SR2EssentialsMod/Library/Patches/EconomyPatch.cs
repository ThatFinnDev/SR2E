using System.Linq;

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
            __instance.BaseValueMap = (from x in __instance.BaseValueMap 
                where !removeMarketPlortEntries.Exists((IdentifiableType y) => y.ValidatableName != x.Accept.identType.ValidatableName)
                select x).ToArray();
        } catch { }*/
        
    }
}