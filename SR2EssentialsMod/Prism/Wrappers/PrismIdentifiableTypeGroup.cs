using UnityEngine.Localization;

namespace SR2E.Prism.Data;

public class PrismIdentifiableTypeGroup
{
    public static implicit operator IdentifiableTypeGroup(PrismIdentifiableTypeGroup group)
    {
        return group.GetIdentifiableTypeGroup();
    }
    
    public static implicit operator PrismIdentifiableTypeGroup(IdentifiableTypeGroup group)
    {
        return group.GetPrismIdentifiableGroup();
    }

    internal PrismIdentifiableTypeGroup(IdentifiableTypeGroup group, bool isNative)
    {
        this._group = group;
        this._isNative = isNative;
    }
    internal IdentifiableTypeGroup _group;
    protected bool _isNative;
    
    public IdentifiableTypeGroup GetIdentifiableTypeGroup() => _group;
    public string GetReferenceID() => _group.ReferenceId;
    public string GetName() => _group.name;
    public bool GetIsFood() => _group._isFood;
    public LocalizedString GetLocalized() => _group.LocalizedName;
    public bool GetIsNative() => _isNative;
    
    public void SetIsFood(bool isFood)
    {
        _group._isFood=isFood;
    }

}