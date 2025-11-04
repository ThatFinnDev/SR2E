using Il2CppSystem.Linq;
using UnityEngine.Localization;

namespace SR2E.Prism.Data;

public class PrismSlime
{
    public static implicit operator SlimeDefinition(PrismSlime prismSlime)
    {
        return prismSlime.GetSlimeDefinition();
    }
    
    internal PrismSlime(SlimeDefinition slimeDefinition, bool isNative)
    {
        this._slimeDefinition = slimeDefinition;
        this._isNative = isNative;
    }
    internal SlimeDefinition _slimeDefinition;
    protected bool _isNative;
    
    public SlimeDefinition GetSlimeDefinition() => _slimeDefinition;
    public string GetReferenceID() => _slimeDefinition.ReferenceId;
    public string GetName() => _slimeDefinition.name;
    public Sprite GetIcon() => _slimeDefinition.icon;
    public LocalizedString GetLocalized() => _slimeDefinition.LocalizedName;
    public Color32 GetVacColor() => _slimeDefinition.color;
    public GameObject GetPrefab() => _slimeDefinition.prefab;
    public SlimeAppearance GetSlimeAppearance() => _slimeDefinition.AppearancesDefault[0];
    public SlimeDiet GetSlimeDiet() => _slimeDefinition.Diet;
    public bool GetIsNative() => _isNative;
    
    public void SetIcon(Sprite newIcon)
    {
        _slimeDefinition.icon = newIcon;
        foreach (var appearance in _slimeDefinition.Appearances.ToList())
            appearance._icon=newIcon;
    }
    public void SetVacColor(Color32 newColor)
    {
        _slimeDefinition.color = newColor;
        foreach (var appearance in _slimeDefinition.Appearances.ToList())
        {
            appearance._splatColor=newColor;
            appearance._colorPalette = new SlimeAppearance.Palette
            {
                Ammo = newColor, Bottom = appearance._colorPalette.Bottom, Middle = appearance._colorPalette.Middle,
                Top = appearance._colorPalette.Top
            };
            ;
        }
    }
    
    
    
    
}