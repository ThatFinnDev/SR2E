using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.UI;
using System.Linq;

namespace SR2E.Library
{
    // Token: 0x02000004 RID: 4
    public static class LibraryPatches
    {
        // Token: 0x04000003 RID: 3
        internal static List<SR2EMod> mods = new List<SR2EMod>();

        // Token: 0x02000006 RID: 6
        [HarmonyPatch(typeof(MarketUI ), "Start")]
        public static class MarketPatch
        {
            // Token: 0x0600003B RID: 59 RVA: 0x00002F94 File Offset: 0x00001194
            public static void Prefix(MarketUI __instance)
            {
                __instance.plorts = (from x in __instance.plorts
                                     where !SR2EMod.marketPlortEntries.Exists((MarketUI.PlortEntry y) => y == x)
                                     select x).ToArray<MarketUI.PlortEntry>();
                __instance.plorts.Take(22).ToArray<MarketUI.PlortEntry>();
            }
        }

        // Token: 0x02000007 RID: 7
        [HarmonyPatch(typeof(EconomyDirector), "InitModel")]
        public static class EconomyPatch
        {
            // Token: 0x0600003C RID: 60 RVA: 0x00002FF0 File Offset: 0x000011F0
            public static void Prefix(EconomyDirector __instance)
            {
                List<EconomyDirector.ValueMap> valueMaps = new List<EconomyDirector.ValueMap>();
                foreach (KeyValuePair<IdentifiableType, CottonMarketData> marketData in SR2EMod.marketData)
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

        // Token: 0x02000008 RID: 8
        [HarmonyPatch(typeof(AutoSaveDirector), "Awake")]
        public static class SaveDirectorPatch
        {
            // Token: 0x0600003D RID: 61 RVA: 0x000030B4 File Offset: 0x000012B4
            public static void Postfix()
            {
                SR2EMod.slimeDefinitions = SR2EMod.Get<SlimeDefinitions>("MainSlimeDefinitions");
                foreach (SR2EMod lib in mods)
                {
                    lib.SaveDirectorLoaded();
                }
            }
        }

        // Token: 0x02000009 RID: 9
        [HarmonyPatch(typeof(SavedGame), "Load")]
        public static class SaveLoadedPatch
        {
            // Token: 0x0600003E RID: 62 RVA: 0x00003128 File Offset: 0x00001328
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
