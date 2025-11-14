using Il2CppMonomiPark.SlimeRancher.Pedia;
using SR2E.Prism.Data;
using SR2E.Prism.Lib;
using UnityEngine.Localization;

namespace SR2E.Prism.Creators;

public class PrismFixedPediaEntryCreatorV01
{
    PrismFixedPediaEntry _createdPediaEntry;
    public string name;
    public Sprite icon;
    public PrismPediaCategoryType categoryType;
    public PrismPediaFactSetType factSet;
    public PrismPediaAdditionalFact[] additionalFacts;
    public LocalizedString descriptionLocalized;
    public LocalizedString titleLocalized;
    public PrismPediaDetail[] details;

    public string customPersistenceSuffix = null;
    public PrismFixedPediaEntryCreatorV01(string name, PrismPediaCategoryType categoryType, LocalizedString titleLocalized, LocalizedString descriptionLocalized)
    {
        this.name = name;
        this.categoryType = categoryType;
        this.descriptionLocalized = descriptionLocalized;
        this.titleLocalized = titleLocalized;
    }
    
    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        for (int i = 0; i < name.Length; i++)
            if (!((name[i] >= 'A' && name[i] <= 'Z') || (name[i] >= 'a' && name[i] <= 'z')))
                return false;
        if (descriptionLocalized==null) return false;
        if (titleLocalized==null) return false;
        if (customPersistenceSuffix!=null)
            for (int i = 0; i < customPersistenceSuffix.Length; i++)
                if (!((customPersistenceSuffix[i] >= 'A' && customPersistenceSuffix[i] <= 'Z') || (customPersistenceSuffix[i] >= 'a' && customPersistenceSuffix[i] <= 'z')))
                    return false;
        return true;
    }

    
    public PrismFixedPediaEntry CreateFixedPediaEntry()
    {
        if (!IsValid()) return null;
        if (_createdPediaEntry != null) return _createdPediaEntry;
        
        var entry = Object.Instantiate(PrismLibPedia._fixedPediaEntryPrefab);
        entry.hideFlags = HideFlags.DontUnloadUnusedAsset;
        entry._title = titleLocalized;
        entry._icon = icon!=null?icon:PrismShortcuts.unavailableIcon;
        entry._description = descriptionLocalized;
        entry.name = name;
        entry._highlightSet = factSet.GetPediaHighlightSet();
        if (customPersistenceSuffix == null)
            entry._persistenceSuffix = entry.name.ToLower();
        else entry._persistenceSuffix = customPersistenceSuffix;
        var _details = new List<PediaEntryDetail>();
        if(details!=null)
            foreach (var detail in details)
                _details.Add(detail.ConvertToNativeType());
        entry._details = _details.ToArray();
        PrismLibPedia.pediaEntryLookup[categoryType].Add(entry);
        
        var prismEntry = new PrismFixedPediaEntry(entry, false);

        _createdPediaEntry = prismEntry;
        PrismShortcuts._prismFixedPediaEntries.Add(entry,prismEntry);
        
        if(additionalFacts!=null)
            foreach (var additionalFact in additionalFacts)
                prismEntry.AddAdditionalFact(additionalFact);
        
        if (PrismLibPedia.pediaCategories.ContainsKey(categoryType))
            PrismLibPedia.pediaCategories[categoryType].GetRuntimeCategory();
        return prismEntry;
    }
}