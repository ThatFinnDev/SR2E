using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.UI;
using System.Linq;

namespace SR2E.Library
{
    
    public static class LibraryPatches
    {
        internal static List<SR2EMod> mods = new List<SR2EMod>();

        [HarmonyPatch(typeof(MarketUI), "Start")]
        public static class MarketPatch
        {
            public static void Prefix(MarketUI __instance)
            {
                __instance.plorts = (from x in __instance.plorts
                                     where !marketPlortEntries.Exists((MarketUI.PlortEntry y) => y == x)
                                     select x).ToArray();
                __instance.plorts.Take(22).ToArray();
            }
        }

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
            }
        }
        [HarmonyPatch(typeof(AutoSaveDirector), "Awake")]
        public static class SaveDirectorPatch
        {
            public static void Postfix()
            {
                slimeDefinitions = Get<SlimeDefinitions>("MainSlimeDefinitions");
                foreach (SR2EMod lib in mods)
                {
                    lib.SaveDirectorLoaded();
                }
            }
        }

        [HarmonyPatch(typeof(SavedGame), "Load")]
        public static class SaveLoadedPatch
        {
            public static void Postfix(SavedGame __instance)
            {
                SR2EMod.Save = __instance.GameState;
                foreach (SR2EMod lib in mods)
                {
                    lib.SavedGameLoad();
                }
            }
        }
    }
}
