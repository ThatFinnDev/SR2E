using Il2CppMonomiPark.SlimeRancher.Pedia;
using SR2E.Prism.Data;
using SR2E.Prism.Lib;
using UnityEngine.Localization;

namespace SR2E.Prism.Creators;

public class PrismIdentifiablePediaEntryCreator
{
    PrismIdentifiablePediaEntry _createdPediaEntry;
    public IdentifiableType identifiableType;
    public PrismPediaCategoryType categoryType;
    public PrismPediaAdditionalFact[] additionalFacts;
    public PrismPediaFactSetType factSet;
    public LocalizedString descriptionLocalized;
    public PrismPediaDetail[] details;
    
    public PrismIdentifiablePediaEntryCreator(IdentifiableType identifiableType, PrismPediaCategoryType categoryType, LocalizedString descriptionLocalized)
    {
        this.identifiableType = identifiableType;
        this.categoryType = categoryType;
        this.descriptionLocalized = descriptionLocalized;
    }
    
    public bool IsValid()
    {
        if (identifiableType==null) return false;
        if (descriptionLocalized==null) return false;
        return true;
    }

    
    public PrismIdentifiablePediaEntry CreateIdentifiablePediaEntry()
    {
        if (!IsValid()) return null;
        if (_createdPediaEntry != null) return _createdPediaEntry;
        
        var entry = Object.Instantiate(PrismaLibPedia._identifiablePediaEntryPrefab);

        entry._title = identifiableType.localizedName;
        entry._identifiableType = identifiableType;
        entry._description = descriptionLocalized;
        entry.name = identifiableType.name;
        entry._highlightSet = factSet.GetPediaHighlightSet();
        var _details = new List<PediaEntryDetail>();
        if(details!=null)
            foreach (var detail in details)
                _details.Add(detail.ConvertToNativeType());
        entry._details = _details.ToArray();
        PrismaLibPedia.pediaEntryLookup[categoryType].Add(entry);
        
        var prismEntry = new PrismIdentifiablePediaEntry(entry, false);

        _createdPediaEntry = prismEntry;
        PrismShortcuts._prismIdentifiablePediaEntries.Add(entry,prismEntry);
        
        if(additionalFacts!=null)
            foreach (var additionalFact in additionalFacts)
                prismEntry.AddAdditionalFact(additionalFact);
        
        return prismEntry;
    }
}