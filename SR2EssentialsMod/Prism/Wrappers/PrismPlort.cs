using UnityEngine.Localization;

namespace SR2E.Prism.Data;

public class PrismPlort
{
    public static implicit operator IdentifiableType(PrismPlort prismPlort)
    {
        return prismPlort.GetIdentifiableType();
    }
    
    public static implicit operator PrismPlort(PrismNativePlort nativePlort)
    {
        return nativePlort.GetPrismPlort();
    }
    internal IdentifiableType _identifiableType;
    private bool _isNative;
    public IdentifiableType GetIdentifiableType() => _identifiableType;
    public string GetReferenceID() => _identifiableType.ReferenceId;
    public string GetName() => _identifiableType.name;
    public Sprite GetIcon() => _identifiableType.icon;
    public LocalizedString GetLocalized() => _identifiableType.LocalizedName;
    public Color32 GetVacColor() => _identifiableType.color;
    public GameObject GetPrefab() => _identifiableType.prefab;
    public bool GetIsNative() => _isNative;
    
    
    public void SetIcon(Sprite newIcon)
    {
        _identifiableType.icon = newIcon;
    }
    public void SetVacColor(Color32 newColor)
    {
        _identifiableType.color = newColor;
    }
    
    internal PrismPlort(IdentifiableType identifiableType, bool isNative)
    {
        this._identifiableType = identifiableType;
        this._isNative = isNative;
    }
}