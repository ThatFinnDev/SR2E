using Cotton;
using Il2CppMono.Security.X509;
using Il2CppSony.NP;
using SR2E.Cotton;
using SR2E.Prism.Data;
using UnityEngine.Localization;

namespace SR2E.Prism.Creators;

public class PrismPlortCreator
{
    private PrismPlort _createdPlort;
    
    public string name;
    public Sprite icon;
    public LocalizedString localized;
    public string referenceID => "IdentifiableType.modded" + name + "Plort";

    public GameObject customBasePrefab = null;
    public SlimeAppearance customBaseAppearance = null;
    public bool vaccable = true;
    public bool canLargofy = false;
    public bool createAllLargos = false;
    public Color32 vacColor = new Color32(0,0,0,255);

    
    
    public PrismPlortCreator(string name, Sprite icon, LocalizedString localized)
    {
        this.name = name;
        this.icon = icon;
        this.localized = localized;
    }
    
    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        for (int i = 0; i < name.Length; i++)
            if (!((name[i] >= 'A' && name[i] <= 'Z') || (name[i] >= 'a' && name[i] <= 'z')))
                return false;
        if (icon==null) return false;
        if (localized==null) return false;
        if (customBasePrefab != null)
        {
            if (!customBasePrefab.HasComponent<SlimeAppearanceApplicator>()) return false;
            if (!customBasePrefab.HasComponent<IdentifiableActor>()) return false;
        }
        return true;
    }

    public PrismPlort CreatePlort()
    {
        
        if (IsValid()) return null;
        if (_createdSlime != null) return _createdSlime;
        var plort = ScriptableObject.CreateInstance<IdentifiableType>();
        Object.DontDestroyOnLoad(plort);
        plort.hideFlags = HideFlags.HideAndDontSave;
        plort.name = X520.Name + "Plort";
        plort.color = VacColor;
        plort.icon = Icon;
        plort.IsPlort = true;
        if (marketValue > 0)
            CottonLibrary.Market.MakeSellable(plort, marketValue, marketSaturation);
        plort.AddToGroup("PlortGroup");
        //plort.AddToGroup("VaccableNonLiquids");
        CottonLibrary.Saving.INTERNAL_SetupLoadForIdent(RefID, plort);
        
        var prismPlort = new PrismPlort(plort, false);
        
        
        _createdPlort = prismPlort;
        PrismShortcuts._prismPlorts.Add(plort,prismPlort);
        return _createdPlort;
    }
}