using UnityEngine.Localization;

namespace SR2E.Prism.Data;

public class PrismGordo
{
    public static implicit operator IdentifiableType(PrismGordo prismGordo)
    {
        return prismGordo.GetIdentifiableType();
    }
    
    internal PrismGordo(IdentifiableType identifiableType, bool isNative)
    {
        this._identifiableType = identifiableType;
        this._isNative = isNative;
    }
    internal IdentifiableType _identifiableType;
    protected bool _isNative;
    
    public IdentifiableType GetIdentifiableType() => _identifiableType;
    public string GetReferenceID() => _identifiableType.ReferenceId;
    public string GetName() => _identifiableType.name;
    public Sprite GetIcon() => _identifiableType.icon;
    public LocalizedString GetLocalized() => _identifiableType.LocalizedName;
    public GameObject GetPrefab() => _identifiableType.prefab;
    public bool GetIsNative() => _isNative;
    
    public void SetIcon(Sprite newIcon)
    {
        _identifiableType.icon = newIcon;
    }
}