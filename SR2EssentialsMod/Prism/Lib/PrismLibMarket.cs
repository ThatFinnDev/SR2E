using Il2CppMonomiPark.SlimeRancher.Economy;
using Il2CppMonomiPark.SlimeRancher.UI;
using SR2E.Prism.Data;

namespace SR2E.Prism.Lib;
/// <summary>
/// A library of helper functions for dealing with the market
/// </summary>
public static class PrismLibMarket
{
    /// <summary>
    /// Makes an identifiable type sellable in the plort market
    /// </summary>
    /// <param name="ident">The identifiable type to make sellable</param>
    /// <param name="prismMarketData">The market data for the identifiable type</param>
    public static void MakeSellable(IdentifiableType ident, PrismMarketData prismMarketData)
    {
        if (ident == null) return;
        if (ident.IsPlayer) return;
        if (ident.isGadget()) return;
        if (PrismShortcuts.marketData.ContainsKey(ident)) PrismShortcuts.marketData.Remove(ident);

        if (PrismShortcuts.removeMarketPlortEntries.Contains(ident))
            PrismShortcuts.removeMarketPlortEntries.Remove(ident);
        PrismShortcuts.marketPlortEntries.Add(new PlortEntry
            {
                IdentType = ident
            },
            prismMarketData.hideInMarketUI);
        PrismShortcuts.marketData.Add(ident, prismMarketData);
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
            foreach (var entry in PrismShortcuts.marketData)
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

    /// <summary>
    /// Checks if an identifiable type is sellable in the plort market
    /// </summary>
    /// <param name="ident">The identifiable type to check</param>
    /// <returns>Whether or not the identifiable type is sellable</returns>
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



        if (PrismShortcuts.marketData.ContainsKey(ident)) return true;
        foreach (var keyPair in PrismShortcuts.marketPlortEntries)
        {
            PlortEntry entry = keyPair.Key;
            if (entry.IdentType == ident)
                return true;
        }
        
        if(ident.ReferenceId!="IdentifiableType.UnstablePlort")
            foreach (var pair in PrismLibLookup.refIDTranslationPrismNativeBaseSlime)
                if (pair.Value == ident.ReferenceId)
                {
                    bool returnBool = true;
                    foreach (IdentifiableType removed in PrismShortcuts.removeMarketPlortEntries)
                        if (ident == removed)
                        {
                            returnBool = false; 
                            break;
                        }
                    return returnBool;
                }

        return false;
    }

    /// <summary>
    /// Makes an identifiable type not sellable in the plort market
    /// </summary>
    /// <param name="ident">The identifiable type to make not sellable</param>
    public static void MakeNotSellable(IdentifiableType ident)
    {
        if (ident == null) return;
        if (ident.IsPlayer) return;
        if (ident.isGadget()) return;
        if(!PrismShortcuts.removeMarketPlortEntries.Contains(ident))
         PrismShortcuts.removeMarketPlortEntries.Add(ident);
        foreach (var keyPair in PrismShortcuts.marketPlortEntries)
        {
            PlortEntry entry = keyPair.Key;
            if (entry.IdentType == ident)
            {
                PrismShortcuts.marketPlortEntries.Remove(entry);
                break;
            }
        }

        if (PrismShortcuts.marketData.ContainsKey(ident))
            PrismShortcuts.marketData.Remove(ident);
    }
}