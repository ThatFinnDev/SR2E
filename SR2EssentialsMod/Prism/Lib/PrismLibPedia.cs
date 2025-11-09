using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.Pedia;
using SR2E.Prism.Data;
using UnityEngine.Localization;

namespace SR2E.Prism.Lib;

/// <summary>
/// A library of helper functions for dealing with the pedia
/// </summary>
public static class PrismLibPedia
{
    internal static Dictionary<PrismPediaCategoryType, PediaCategory> pediaCategories = new();
    static Dictionary<PrismPediaDetailType, PediaDetailSection> pediaDetailSectionLookup = new Dictionary<PrismPediaDetailType, PediaDetailSection>();
    static Dictionary<PrismPediaFactSetType, PediaHighlightSet> pediaPrismFactSetLookup = new Dictionary<PrismPediaFactSetType, PediaHighlightSet>() { };
    internal static Dictionary<PediaEntry, List<PrismPediaAdditionalFact>> _additionalFactsMap = new Dictionary<PediaEntry, List<PrismPediaAdditionalFact>>();
    internal static IdentifiablePediaEntry _identifiablePediaEntryPrefab = null;
    internal static FixedPediaEntry _fixedPediaEntryPrefab = null;
    internal static Dictionary<PrismPediaCategoryType, List<PediaEntry>> pediaEntryLookup = new Dictionary<PrismPediaCategoryType, List<PediaEntry>>()
    {
        {PrismPediaCategoryType.Slimes, new List<PediaEntry>()}, {PrismPediaCategoryType.Resources, new List<PediaEntry>()}, {PrismPediaCategoryType.Blueprints, new List<PediaEntry>()},
        {PrismPediaCategoryType.World, new List<PediaEntry>()}, {PrismPediaCategoryType.Weather, new List<PediaEntry>()}, {PrismPediaCategoryType.Toys, new List<PediaEntry>()},
        {PrismPediaCategoryType.Ranch, new List<PediaEntry>()}, {PrismPediaCategoryType.Science, new List<PediaEntry>()}, {PrismPediaCategoryType.Tutorial, new List<PediaEntry>()},
    };



    /// <summary>
    /// Gets the pedia entries for a given category
    /// </summary>
    /// <param name="type">The category to get the entries for</param>
    /// <returns>The pedia entries for the given category</returns>
    public static List<PediaEntry> GetPediaEntries(this PrismPediaCategoryType type)
    {
        if (pediaEntryLookup.ContainsKey(type)) return pediaEntryLookup[type];
        return null;
    }
    /// <summary>
    /// Gets the pedia highlight set for a given fact set type
    /// </summary>
    /// <param name="type">The fact set type to get the highlight set for</param>
    /// <returns>The pedia highlight set for the given fact set type</returns>
    public static PediaHighlightSet GetPediaHighlightSet(this PrismPediaFactSetType type)
    {
        if (pediaPrismFactSetLookup.ContainsKey(type)) return pediaPrismFactSetLookup[type];
        return null;
    }
    /// <summary>
    /// Gets the pedia detail section for a given detail type
    /// </summary>
    /// <param name="type">The detail type to get the detail section for</param>
    /// <returns>The pedia detail section for the given detail type</returns>
    public static PediaDetailSection GetPediaDetailSection(this PrismPediaDetailType type)
    {
        if (pediaDetailSectionLookup.ContainsKey(type)) return pediaDetailSectionLookup[type];
        return null;
    }
    
    internal static PediaEntryHighlight ConvertToNativeType(this PrismPediaAdditionalFact prismPediaAdditionalFact) => new PediaEntryHighlight()
    {
        Icon = prismPediaAdditionalFact.icon,
        Description = prismPediaAdditionalFact.description,
        Label = prismPediaAdditionalFact.title,
    };

    internal static PediaEntryHighlight ConvertToNativeType(this PrismPediaAdditionalFact? prismPediaAdditionalFact)
    {
        if (prismPediaAdditionalFact == null) return null;
        return ConvertToNativeType(prismPediaAdditionalFact.Value);
    }
    internal static PediaEntryDetail ConvertToNativeType(this PrismPediaDetail prismPediaDetail) => new PediaEntryDetail()
    {
        Contents = new Il2CppReferenceArray<PediaEntryDetailSubContent>(0),
        Section = pediaDetailSectionLookup[prismPediaDetail.type],
        Text = prismPediaDetail.text,
        TextGamepad = prismPediaDetail.text,
        TextPS4 = prismPediaDetail.text,
    };

    internal static PediaEntryDetail ConvertToNativeType(this PrismPediaDetail? prismPediaDetail)
    {
        if (prismPediaDetail == null) return null;
        return ConvertToNativeType(prismPediaDetail.Value);
    }
    
    internal static void PediaDetailTypesInitialize()
    {
        pediaDetailSectionLookup.Add(PrismPediaDetailType.Slimeology, Get<PediaDetailSection>("Slimeology"));
        pediaDetailSectionLookup.Add(PrismPediaDetailType.RancherRisks, Get<PediaDetailSection>("Rancher Risks"));
        pediaDetailSectionLookup.Add(PrismPediaDetailType.Plortonomics, Get<PediaDetailSection>("Plortonomics"));
        pediaDetailSectionLookup.Add(PrismPediaDetailType.About, Get<PediaDetailSection>("About"));
        pediaDetailSectionLookup.Add(PrismPediaDetailType.OnTheRanch, Get<PediaDetailSection>("How To Use"));
        pediaDetailSectionLookup.Add(PrismPediaDetailType.Instructions, Get<PediaDetailSection>("Instructions"));

        pediaPrismFactSetLookup.Add(PrismPediaFactSetType.None, Get<PediaHighlightSet>("TutorialPediaTemplate"));
        pediaPrismFactSetLookup.Add(PrismPediaFactSetType.Resource, Get<PediaHighlightSet>("ResourceHighlights"));
        pediaPrismFactSetLookup.Add(PrismPediaFactSetType.Slime, Get<PediaHighlightSet>("SlimeHighlights"));
        pediaPrismFactSetLookup.Add(PrismPediaFactSetType.Food, Get<PediaHighlightSet>("FoodHightlights")); //yes, there is a typo in there...

        _identifiablePediaEntryPrefab = Get<IdentifiablePediaEntry>("Pink");
        if (_identifiablePediaEntryPrefab == null) _identifiablePediaEntryPrefab = GetAny<IdentifiablePediaEntry>();
        if (_identifiablePediaEntryPrefab != null) _identifiablePediaEntryPrefab.hideFlags = HideFlags.DontUnloadUnusedAsset; 
        
        _fixedPediaEntryPrefab = Get<FixedPediaEntry>("PrismaPlorts");
        if (_fixedPediaEntryPrefab == null) _fixedPediaEntryPrefab = GetAny<FixedPediaEntry>();
        if (_fixedPediaEntryPrefab != null) _fixedPediaEntryPrefab.hideFlags = HideFlags.DontUnloadUnusedAsset;
    }
}