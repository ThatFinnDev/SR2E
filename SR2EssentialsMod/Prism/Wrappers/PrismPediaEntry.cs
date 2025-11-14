using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.Pedia;
using SR2E.Prism.Lib;
using UnityEngine.Localization;

namespace SR2E.Prism.Data;

public class PrismPediaEntry
{
    public static implicit operator PediaEntry(PrismPediaEntry prismPediaEntry)
    {
        return prismPediaEntry.GetPediaEntry();
    }
    
    internal PrismPediaEntry(PediaEntry pediaEntry, bool isNative)
    {
        this._pediaEntry = pediaEntry;
        this._isNative = isNative;
    }
    internal PediaEntry _pediaEntry;
    protected bool _isNative;
    
    public PediaEntry GetPediaEntry() => _pediaEntry;
    public string GetPersistenceID() => _pediaEntry.PersistenceId;
    public bool GetIsUnlockedInitially() => _pediaEntry._isUnlockedInitially;
    public string GetName() => _pediaEntry.name;
    public Sprite GetIcon() => _pediaEntry.Icon;
    public LocalizedString GetTitle() => _pediaEntry.Title;
    public LocalizedString GetDescription() => _pediaEntry.Description;
    public int GetDetailCount() => _pediaEntry._details.Count;
    public bool GetIsNative() => _isNative;

    public void AddDetail(PrismPediaDetail? detail)
    {
        if(detail!=null)
            _pediaEntry._details = _pediaEntry._details.AddToNew(detail.ConvertToNativeType());
    }
    public void RemoveAtDetail(int index)
    {
        if (index < 0) return;
        if (index >= _pediaEntry._details.Count) return;
        _pediaEntry._details = _pediaEntry._details.RemoveAtToNew(index);
    }    
    public void ReplaceDetail(int index,PrismPediaDetail? detail)
    {
        if (index < 0) return;
        if (index >= _pediaEntry._details.Count) return;
        if (detail == null) return;
        _pediaEntry._details = _pediaEntry._details.ReplaceToNew(detail.ConvertToNativeType(),index);
    }    
    public void InsertDetail(int index,PrismPediaDetail? detail)
    {
        if (index < 0) return;
        if (index >= _pediaEntry._details.Count) return;
        if (detail == null) return;
        _pediaEntry._details = _pediaEntry._details.InsertToNew(detail.ConvertToNativeType(),index);
    }

    public void ClearDetails()
    {
        _pediaEntry._details = new Il2CppReferenceArray<PediaEntryDetail>(0);
    }
    
    public void AddAdditionalFact(PrismPediaAdditionalFact? fact)
    {
        if (fact == null) return;
        if (!PrismLibPedia._additionalFactsMap.ContainsKey(_pediaEntry))
            PrismLibPedia._additionalFactsMap[_pediaEntry] = new List<PrismPediaAdditionalFact>();
        PrismLibPedia._additionalFactsMap[_pediaEntry].Add(fact.Value);
    }
    public void RemoveAdditionalFact(PrismPediaAdditionalFact? fact)
    {
        if (fact == null) return;
        if (!PrismLibPedia._additionalFactsMap.ContainsKey(_pediaEntry)) return;
        if (PrismLibPedia._additionalFactsMap[_pediaEntry].Contains(fact.Value))
            PrismLibPedia._additionalFactsMap[_pediaEntry].Remove(fact.Value);
    }
    public void RemoveAtAdditionalFact(int index)
    {
        if (index < 0) return;
        if (!PrismLibPedia._additionalFactsMap.ContainsKey(_pediaEntry)) return;
        if (PrismLibPedia._additionalFactsMap[_pediaEntry].Count>index)
            PrismLibPedia._additionalFactsMap[_pediaEntry].RemoveAt(index);
    }

    public void ClearAdditionalFacts()
    {
        if (PrismLibPedia._additionalFactsMap.ContainsKey(_pediaEntry)) return;
            PrismLibPedia._additionalFactsMap.Remove(_pediaEntry);
    }
}