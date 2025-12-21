using System;
using System.Linq;
using Il2CppMonomiPark.SlimeRancher.Pedia;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppSystem.Linq;
using SR2E.Prism.Creators;
using SR2E.Prism.Data;
using SR2E.Prism.Lib;
using SR2E.Storage;
using UnityEngine.Localization;

namespace SR2E.Prism;

public static class PrismShortcuts
{
    internal static Dictionary<IdentifiableType, PrismMarketData> marketData = new (0);
    internal static Dictionary<PlortEntry, bool> marketPlortEntries = new ();
    internal static List<IdentifiableType> removeMarketPlortEntries = new ();
    internal static List<System.Action> createLargoActions = new ();
    
    
    internal static Dictionary<IdentifiableTypeGroup, PrismIdentifiableTypeGroup> _prismIdentifiableTypeGroups = new Dictionary<IdentifiableTypeGroup, PrismIdentifiableTypeGroup>();
    internal static Dictionary<string, PrismBaseSlime> _prismBaseSlimes = new Dictionary<string, PrismBaseSlime>();
    internal static Dictionary<string, PrismGordo> _prismGordos = new Dictionary<string, PrismGordo>();
    internal static Dictionary<string, PrismLargo> _prismLargos = new Dictionary<string, PrismLargo>();

    internal static TripleDictionary<PrismLargo, PrismBaseSlime, PrismBaseSlime> _prismLargoBases = new TripleDictionary<PrismLargo, PrismBaseSlime, PrismBaseSlime>();
    internal static Dictionary<string, PrismPlort> _prismPlorts = new Dictionary<string, PrismPlort>();
    internal static Dictionary<FixedPediaEntry, PrismFixedPediaEntry> _prismFixedPediaEntries = new Dictionary<FixedPediaEntry, PrismFixedPediaEntry>();
    internal static Dictionary<IdentifiablePediaEntry, PrismIdentifiablePediaEntry> _prismIdentifiablePediaEntries = new Dictionary<IdentifiablePediaEntry, PrismIdentifiablePediaEntry>();
    internal static Dictionary<PediaEntry, PrismPediaEntry> _prismUnknownPediaEntries = new Dictionary<PediaEntry, PrismPediaEntry>();
    internal static LocalizedString emptyTranslation;
    internal static Sprite unavailableIcon;
    
    
    private static SlimeAppearanceDirector _mainAppearanceDirector;

    public static SlimeAppearanceDirector mainAppearanceDirector
    {
        get
        {
            if (_mainAppearanceDirector == null)
                
                _mainAppearanceDirector = Get<SlimeAppearanceDirector>("MainSlimeAppearanceDirector");
            return _mainAppearanceDirector;
        }
        set { _mainAppearanceDirector = value; }
    }
    internal static Dictionary<string, List<Action>> onSceneLoaded = new Dictionary<string, List<Action>>();
    internal static void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        var pair = onSceneLoaded.FirstOrDefault(x => sceneName.Contains(x.Key));

        if (pair.Value != null)
            foreach (var action in pair.Value)
                action();
    }
    internal static void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
    }

    
    
    public static PrismBaseSlime GetPrismBaseSlime(this PrismNativeBaseSlime nativeBaseSlime)
    {
        try
        {
            return LookupEUtil.baseSlimeTypes.GetEntryByRefID(nativeBaseSlime.GetReferenceID()).GetPrismBaseSlime();
        } catch { }
        return null;
    }
    public static PrismPlort GetPrismPlort(this PrismNativePlort nativePlort)
    {
        try
        {
            return LookupEUtil.plortTypes.GetEntryByRefID(nativePlort.GetReferenceID()).GetPrismPlort();
        } catch { }
        return null;
    }
    public static PrismBaseSlime GetPrismBaseSlime(this SlimeDefinition customOrNativeSlime)
    {
        if (customOrNativeSlime == null) return null;
        if (customOrNativeSlime.IsLargo) return null;
        if (_prismBaseSlimes.ContainsKey(customOrNativeSlime.ReferenceId)) return _prismBaseSlimes[customOrNativeSlime.ReferenceId];
        var newSlime = new PrismBaseSlime(customOrNativeSlime, true);
        _prismBaseSlimes.Add(customOrNativeSlime.ReferenceId, newSlime);
        return newSlime;
    }
    public static PrismGordo GetPrismGordo(this IdentifiableType customOrNativeGordo)
    {
        if (customOrNativeGordo == null) return null;
        if (customOrNativeGordo.prefab==null) return null;
        if (!customOrNativeGordo.prefab.HasComponent<GordoIdentifiable>()) return null;
        if (_prismGordos.ContainsKey(customOrNativeGordo.ReferenceId)) return _prismGordos[customOrNativeGordo.ReferenceId];
        var newGordo = new PrismGordo(customOrNativeGordo, true);
        _prismGordos.Add(customOrNativeGordo.ReferenceId, newGordo);
        return newGordo;
    }
    public static PrismLargo GetPrismLargo(this SlimeDefinition customOrNativeSlime)
    {
        if (customOrNativeSlime == null) return null;
        if (!customOrNativeSlime.IsLargo) return null;
        if (_prismLargos.ContainsKey(customOrNativeSlime.ReferenceId)) return _prismLargos[customOrNativeSlime.ReferenceId];
        var newSlime = new PrismLargo(customOrNativeSlime, true);
        _prismLargos.Add(customOrNativeSlime.ReferenceId, newSlime);
        return newSlime;
    }
    
    public static PrismSlime GetPrismSlime(this SlimeDefinition customOrNativeSlime)
    {
        if (customOrNativeSlime == null) return null;
        if (customOrNativeSlime.IsLargo)
        {
            if (_prismLargos.ContainsKey(customOrNativeSlime.ReferenceId)) return _prismLargos[customOrNativeSlime.ReferenceId];
            var newLargo = new PrismLargo(customOrNativeSlime, true);
            _prismLargos.Add(customOrNativeSlime.ReferenceId, newLargo);
            return newLargo;
        }
        if (_prismBaseSlimes.ContainsKey(customOrNativeSlime.ReferenceId)) return _prismBaseSlimes[customOrNativeSlime.ReferenceId];
        var newBaseSlime = new PrismBaseSlime(customOrNativeSlime, true);
        _prismBaseSlimes.Add(customOrNativeSlime.ReferenceId, newBaseSlime);
        return newBaseSlime;
        
    }

    public static PrismPlort GetPrismPlort(this IdentifiableType customOrNativePlort)
    {
        if (customOrNativePlort == null) return null;
        if (!customOrNativePlort.IsPlort) return null;
        if (_prismPlorts.ContainsKey(customOrNativePlort.ReferenceId)) return _prismPlorts[customOrNativePlort.ReferenceId];
        var newPlort = new PrismPlort(customOrNativePlort, true);
        _prismPlorts.Add(customOrNativePlort.ReferenceId, newPlort);
        return newPlort;
    }

    public static PrismIdentifiableTypeGroup GetPrismIdentifiableGroup(this IdentifiableTypeGroup group)
    {
        if (group == null) return null;
        if (_prismIdentifiableTypeGroups.ContainsKey(group)) return _prismIdentifiableTypeGroups[group];
        var newPlort = new PrismIdentifiableTypeGroup(group, true);
        _prismIdentifiableTypeGroups.Add(group, newPlort);
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