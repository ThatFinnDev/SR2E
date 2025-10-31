using Il2CppMonomiPark.SlimeRancher.Economy;
using Il2CppMonomiPark.SlimeRancher.UI;
using SR2E.Cotton.Enums;

namespace SR2E.Cotton;

public static partial class CottonLibrary
{
    public static class Market
    {
        public static void MakeSellable(IdentifiableType ident, float marketValue, float marketSaturation, bool hideInMarket = false)
        {
            if (marketData.ContainsKey(ident))
            {
                MelonLogger.Error("Failed to make Object sellable: The Object is already sellable");
                return;
            }

            if (removeMarketPlortEntries.Contains(ident))
                removeMarketPlortEntries.Remove(ident);
            marketPlortEntries.Add(new PlortEntry
                {
                    IdentType = ident
                },
                hideInMarket);
            marketData.Add(ident, new ModdedMarketData(marketSaturation, marketValue));
            TryRefreshMarketData();
        }

        internal static void TryRefreshMarketData(PlortEconomySettings settings = null)
        {
            try
            {
                if(settings==null) settings = Get<PlortEconomySettings>("PlortEconomy");
                if (settings == null) return;
                List<PlortValueConfiguration> entries = new List<PlortValueConfiguration>();
                entries.AddRange(settings.PlortsTable.Plorts);
                foreach (var entry in CottonLibrary.marketData)
                {
                    foreach (var existingEntry in settings.PlortsTable.Plorts)
                    {
                        if(existingEntry.Type.ReferenceId==entry.Key.ReferenceId)
                        {
                            entries.Remove(existingEntry);
                            break;
                        }
                    }
                    var defualtValues = new PlortValueConfiguration()
                    {
                        FullSaturation = entry.Value.saturation,
                        Type = entry.Key,
                        InitialValue = entry.Value.value
                    };
            
                    entries.Add(defualtValues);
                }
                PlortValueConfigurationTable newTable = new PlortValueConfigurationTable();
                newTable.Plorts = entries.ToArray();
                settings.PlortsTable = newTable;
            }
            catch {}
        }
        public static bool IsSellable(IdentifiableType ident)
        {
            bool returnBool = false;
            List<string> sellableByDefault = new List<string>
            {
                "PinkPlort",
                "CottonPlort",
                "PhosphorPlort",
                "TabbyPlort",
                "AnglerPlort",
                "RockPlort",
                "HoneyPlort",
                "BoomPlort",
                "PuddlePlort",
                "FirePlort",
                "BattyPlort",
                "CrystalPlort",
                "HunterPlort",
                "FlutterPlort",
                "RingtailPlort",
                "SaberPlort",
                "YolkyPlort",
                "TanglePlort",
                "DervishPlort",
                "TwinPlort",
                "SloomberPlort",
                "ShadowPlort",
                "PrismaPlort",
                "HyperPlort",
                "GoldPlort"
            };
            if (removeMarketPlortEntries.Count != 0)
                foreach (string sellable in sellableByDefault)
                    if (sellable == ident.name)
                    {
                        returnBool = true;
                        foreach (IdentifiableType removed in removeMarketPlortEntries)
                            if (ident == removed)
                                returnBool = false;
                    }

            if (marketData.ContainsKey(ident))
                return true;
            foreach (var keyPair in marketPlortEntries)
            {
                PlortEntry entry = keyPair.Key;
                if (entry.IdentType == ident)
                    return true;
            }

            return returnBool;
        }

        public static void MakeNOTSellable(IdentifiableType ident)
        {
            removeMarketPlortEntries.Add(ident);
            foreach (var keyPair in marketPlortEntries)
            {
                PlortEntry entry = keyPair.Key;
                if (entry.IdentType == ident)
                {
                    marketPlortEntries.Remove(entry);
                    break;
                }
            }

            if (marketData.ContainsKey(ident))
                marketData.Remove(ident);
        }
    }
}