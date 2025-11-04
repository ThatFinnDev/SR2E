using Il2CppMonomiPark.SlimeRancher.Pedia;
using Il2CppSystem.Linq;
using SR2E.Cotton;
using SR2E.Prism.Creators;
using SR2E.Prism.Data;
using SR2E.Prism.Lib;
using SR2E.Storage;
using UnityEngine.Localization;

namespace SR2E.Prism;

public static class PrismShortcuts
{
    internal static Dictionary<SlimeDefinition, PrismBaseSlime> _prismBaseSlimes = new Dictionary<SlimeDefinition, PrismBaseSlime>();
    internal static Dictionary<SlimeDefinition, PrismLargo> _prismLargos = new Dictionary<SlimeDefinition, PrismLargo>();

    internal static TripleDictionary<PrismLargo, PrismBaseSlime, PrismBaseSlime> _prismLargoBases = new TripleDictionary<PrismLargo, PrismBaseSlime, PrismBaseSlime>();
    internal static Dictionary<IdentifiableType, PrismPlort> _prismPlorts = new Dictionary<IdentifiableType, PrismPlort>();
    internal static Dictionary<FixedPediaEntry, PrismFixedPediaEntry> _prismFixedPediaEntries = new Dictionary<FixedPediaEntry, PrismFixedPediaEntry>();
    internal static Dictionary<IdentifiablePediaEntry, PrismIdentifiablePediaEntry> _prismIdentifiablePediaEntries = new Dictionary<IdentifiablePediaEntry, PrismIdentifiablePediaEntry>();
    internal static Dictionary<PediaEntry, PrismPediaEntry> _prismUnknownPediaEntries = new Dictionary<PediaEntry, PrismPediaEntry>();
    internal static LocalizedString emptyTranslation;
    internal static Sprite unavailableIcon;
    public static PrismBaseSlime GetPrismBaseSlime(this PrismNativeBaseSlime nativeBaseSlime)
    {
        var refID = nativeBaseSlime.GetReferenceID();
        foreach (var slime in CottonLibrary.baseSlimes.GetAllMembersList())
            try {
                if (slime.ReferenceId == refID) return slime.TryCast<SlimeDefinition>().GetPrismBaseSlime();
            } catch { }
        return null;
    }
    public static PrismPlort GetPrismPlort(this PrismNativePlort nativePlort)
    {
        var refID = nativePlort.GetReferenceID();
        foreach (var plort in CottonLibrary.plorts.GetAllMembersList())
            try
            {
                if (plort.ReferenceId == refID) return plort.GetPrismPlort();
            } catch { }
        return null;
    }
    public static PrismBaseSlime GetPrismBaseSlime(this SlimeDefinition customOrNativeSlime)
    {
        if (customOrNativeSlime == null) return null;
        if (customOrNativeSlime.IsLargo) return null;
        if (_prismBaseSlimes.ContainsKey(customOrNativeSlime)) return _prismBaseSlimes[customOrNativeSlime];
        var newSlime = new PrismBaseSlime(customOrNativeSlime, true);
        _prismBaseSlimes.Add(customOrNativeSlime, newSlime);
        return newSlime;
    }
    
    public static PrismLargo GetPrismLargo(this SlimeDefinition customOrNativeSlime)
    {
        if (customOrNativeSlime == null) return null;
        if (!customOrNativeSlime.IsLargo) return null;
        if (_prismLargos.ContainsKey(customOrNativeSlime)) return _prismLargos[customOrNativeSlime];
        var newSlime = new PrismLargo(customOrNativeSlime, true);
        _prismLargos.Add(customOrNativeSlime, newSlime);
        return newSlime;
    }
    
    public static PrismSlime GetPrismSlime(this SlimeDefinition customOrNativeSlime)
    {
        if (customOrNativeSlime == null) return null;
        if (customOrNativeSlime.IsLargo)
        {
            if (_prismLargos.ContainsKey(customOrNativeSlime)) return _prismBaseSlimes[customOrNativeSlime];
            var newLargo = new PrismLargo(customOrNativeSlime, true);
            _prismLargos.Add(customOrNativeSlime, newLargo);
            return newLargo;
        }
        if (_prismBaseSlimes.ContainsKey(customOrNativeSlime)) return _prismBaseSlimes[customOrNativeSlime];
        var newBaseSlime = new PrismBaseSlime(customOrNativeSlime, true);
        _prismBaseSlimes.Add(customOrNativeSlime, newBaseSlime);
        return newBaseSlime;
        
    }

    public static PrismPlort GetPrismPlort(this IdentifiableType customOrNativePlort)
    {
        if (customOrNativePlort == null) return null;
        if (!customOrNativePlort.IsPlort) return null;
        if (_prismPlorts.ContainsKey(customOrNativePlort)) return _prismPlorts[customOrNativePlort];
        var newPlort = new PrismPlort(customOrNativePlort, true);
        _prismPlorts.Add(customOrNativePlort, newPlort);
        return newPlort;
    }

    
    
    public static PrismPediaEntry GetPrismPediaEntry(this PediaEntry customOrNativePedia)
    {
        if (customOrNativePedia == null) return null;
        if (customOrNativePedia.TryCast<FixedPediaEntry>()!=null)
        {
            var pedia = customOrNativePedia.TryCast<FixedPediaEntry>();
            if (_prismFixedPediaEntries.ContainsKey(pedia)) return _prismFixedPediaEntries[pedia];
            var newEntry = new PrismFixedPediaEntry(pedia, true);
            _prismFixedPediaEntries.Add(pedia, newEntry);
            return newEntry;
        }
        else if (customOrNativePedia.TryCast<IdentifiablePediaEntry>()!=null)
        {
            var pedia = customOrNativePedia.TryCast<IdentifiablePediaEntry>();
            if (_prismIdentifiablePediaEntries.ContainsKey(pedia)) return _prismIdentifiablePediaEntries[pedia];
            var newEntry = new PrismIdentifiablePediaEntry(pedia, true);
            _prismIdentifiablePediaEntries.Add(pedia, newEntry);
            return newEntry;
        }
        else
        {
            if (_prismUnknownPediaEntries.ContainsKey(customOrNativePedia)) return _prismUnknownPediaEntries[customOrNativePedia];
            var newEntry = new PrismPediaEntry(customOrNativePedia, true);
            _prismUnknownPediaEntries.Add(customOrNativePedia, newEntry);
            return newEntry;
        }
    }
    public static PrismFixedPediaEntry GetPrismFixedPediaEntry(this FixedPediaEntry customOrNativePedia)
    {
        if (customOrNativePedia == null) return null;
        if (_prismFixedPediaEntries.ContainsKey(customOrNativePedia)) return _prismFixedPediaEntries[customOrNativePedia];
        var newPedia = new PrismFixedPediaEntry(customOrNativePedia, true);
        _prismFixedPediaEntries.Add(customOrNativePedia, newPedia);
        return newPedia;
    }
    public static PrismIdentifiablePediaEntry GetPrismIdentifiablePediaEntry(this IdentifiablePediaEntry customOrNativePedia)
    {
        if (customOrNativePedia == null) return null;
        if (_prismIdentifiablePediaEntries.ContainsKey(customOrNativePedia)) return _prismIdentifiablePediaEntries[customOrNativePedia];
        var newPedia = new PrismIdentifiablePediaEntry(customOrNativePedia, true);
        _prismIdentifiablePediaEntries.Add(customOrNativePedia, newPedia);
        return newPedia;
    }
}