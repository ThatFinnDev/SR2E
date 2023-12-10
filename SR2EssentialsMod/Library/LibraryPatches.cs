using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.UI;
using System.Linq;
using Il2CppAssets.Script.Util.Extensions;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.IO;

namespace SR2E.Library
{
    
    public static class LibraryPatches
    {
        internal static List<SR2EMod> mods = new List<SR2EMod>();
        
        [HarmonyPatch(typeof(MarketUI), nameof(MarketUI.Start))]
        public static class MarketPatch
        {
            public static void Prefix(MarketUI __instance)
            {
                List<MarketUI.PlortEntry> marketPlortEntriesList = new List<MarketUI.PlortEntry>();
                foreach (var pair in marketPlortEntries)
                    if(!pair.Value)
                        marketPlortEntriesList.Add(pair.Key);
                
                __instance.plorts = (from x in __instance.plorts
                                     where !marketPlortEntriesList.Exists
                                         ((MarketUI.PlortEntry y) => y == x)
                                     select x).ToArray();
                __instance.plorts = __instance.plorts.ToArray<MarketUI.PlortEntry>().AddRangeToArray(marketPlortEntriesList.ToArray());

                //To someone that finds that code, ik it looks like garbage, because it is,
                //but i've tried for too long to make it work. This works some I just gonna leave it
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
                
                __instance.plorts = __instance.plorts.Take(34).ToArray();
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
                
                //Same as on the top
                List<EconomyDirector.ValueMap> list = __instance.BaseValueMap.ToList();
                List<EconomyDirector.ValueMap> listTwo = __instance.BaseValueMap.ToList();
                foreach (EconomyDirector.ValueMap entry in list)
                foreach (IdentifiableType toRemove in removeMarketPlortEntries)
                    if (entry.Accept.identType.ValidatableName == toRemove.ValidatableName)
                        listTwo.Remove(entry);
                __instance.BaseValueMap = new Il2CppReferenceArray<EconomyDirector.ValueMap>(0);
                foreach (EconomyDirector.ValueMap entry in listTwo)
                    if (entry != null)
                        __instance.BaseValueMap.AddItem(entry);
            }
        }
        [HarmonyPatch(typeof(AutoSaveDirector), "Awake")]
        public static class SaveDirectorPatch
        {
            public static void Prefix(AutoSaveDirector __instance)
            {
                foreach (SR2EMod lib in mods)
                {
                    lib.SaveDirectorLoading(__instance);
                }
            }
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
                foreach (SR2EMod lib in mods)
                {
                    lib.OnSavedGameLoaded();
                }
            }
        }
    }
}
