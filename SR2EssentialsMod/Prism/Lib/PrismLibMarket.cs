using Il2CppMonomiPark.SlimeRancher.Economy;
using Il2CppMonomiPark.SlimeRancher.UI;
using SR2E.Cotton;
using SR2E.Prism.Enums;

namespace SR2E.Prism.Lib;

public static class PrismLibMarket
{
    public static void MakeSellable(IdentifiableType ident, PrismMarketData prismMarketData)
    {
        if (ident == null) return;
        if (ident.IsPlayer) return;
        if (ident.isGadget()) return;
        if (CottonLibrary.marketData.ContainsKey(ident)) CottonLibrary.marketData.Remove(ident);

        if (CottonLibrary.removeMarketPlortEntries.Contains(ident))
            CottonLibrary.removeMarketPlortEntries.Remove(ident);
        CottonLibrary.marketPlortEntries.Add(new PlortEntry
            {
                IdentType = ident
            },
            prismMarketData.hideInMarketUI);
        CottonLibrary.marketData.Add(ident, prismMarketData);
        TryRefreshMarketData();
    }

    internal static void TryRefreshMarketData(PlortEconomySettings settings = null)
    {
        try
        {
            if (settings == null) settings = Get<PlortEconomySettings>("PlortEconomy");
            if (settings == null) return;
            List<PlortValueConfiguration> entries = new List<PlortValueConfiguration>();
            entries.AddRange(settings.PlortsTable.Plorts);
            foreach (var entry in CottonLibrary.marketData)
            {
                foreach (var existingEntry in settings.PlortsTable.Plorts)
                {
                    if (existingEntry.Type.ReferenceId == entry.Key.ReferenceId)
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
        catch
        {
        }
    }

    public static bool IsSellable(IdentifiableType ident)
    {
        if (ident == null) return false;
        if (ident.IsPlayer) return false;
        if (ident.isGadget()) return false;
        try
        {
            var settings = Get<PlortEconomySettings>("PlortEconomy");
            foreach (var entry in settings.PlortsTable.Plorts)
                if (entry.Type == ident)
                    return true;
        }catch { }



        if (CottonLibrary.marketData.ContainsKey(ident)) return true;
        foreach (var keyPair in CottonLibrary.marketPlortEntries)
        {
            PlortEntry entry = keyPair.Key;
            if (entry.IdentType == ident)
                return true;
        }

        if (CottonLibrary.removeMarketPlortEntries.Count != 0)
            foreach (var pair in PrismLibLookup.refIDTranslationPrismNativeBaseSlime)
                if (pair.Value == ident.ReferenceId)
                {
                    bool returnBool = true;
                    foreach (IdentifiableType removed in CottonLibrary.removeMarketPlortEntries)
                        if (ident == removed)
                        {
                            returnBool = false;
                            break;
                        }

                    return returnBool;
                }

        return false;
    }

    public static void MakeNOTSellable(IdentifiableType ident)
    {
        if (ident == null) return;
        if (ident.IsPlayer) return;
        if (ident.isGadget()) return;
        CottonLibrary.removeMarketPlortEntries.Add(ident);
        foreach (var keyPair in CottonLibrary.marketPlortEntries)
        {
            PlortEntry entry = keyPair.Key;
            if (entry.IdentType == ident)
            {
                CottonLibrary.marketPlortEntries.Remove(entry);
                break;
            }
        }

        if (CottonLibrary.marketData.ContainsKey(ident))
            CottonLibrary.marketData.Remove(ident);
    }
}