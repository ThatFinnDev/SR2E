using SR2E.Prism.Data;
using SR2E.Prism.Lib;
using UnityEngine.Localization;

namespace SR2E.Prism.Creators;

public class PrismPlortCreatorV01
{
    private PrismPlort _createdPlort;
    
    public string name;
    public Sprite icon;
    public LocalizedString localized;
    public string referenceID => "IdentifiableType.Modded" + name + "Plort";

    public Color32 vacColor = new Color32(0,0,0,255);
    public GameObject customBasePrefab = null;
    
    
    public PrismMarketData? moddedMarketData = null;
    
    
    public PrismPlortCreatorV01(string name, Sprite icon, LocalizedString localized)
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
            if (!customBasePrefab.HasComponent<IdentifiableActor>()) return false;
        }
        return true;
    }

    public PrismPlort CreatePlort()
    {
        if (!IsValid()) return null;
        if (_createdPlort != null) return _createdPlort;
        var plort = ScriptableObject.CreateInstance<IdentifiableType>();
        plort.hideFlags = HideFlags.DontUnloadUnusedAsset;
        plort.name = name + "Plort";
        plort.color = vacColor;
        plort.icon = icon;
        plort.IsPlort = true;
        
        
        plort.localizedName = localized;
        plort._pediaPersistenceSuffix = "modded"+name.ToLower()+"_plort";
        
        if(moddedMarketData.HasValue)
            PrismLibMarket.MakeSellable(plort, moddedMarketData.Value);
        plort.Prism_AddToGroup("PlortGroup");
        plort.Prism_AddToGroup("EdiblePlortFoodGroup");
        plort.Prism_AddToGroup("PlortGroupDroneExplorer");
        plort.Prism_AddToGroup("IdentifiableTypesGroup");
        
        var basePrefab = customBasePrefab;
        if (basePrefab == null) basePrefab = PrismNativePlort.Pink.GetPrismPlort().GetPrefab();
        plort.prefab = CreatePrefab("plort"+name, basePrefab);
        plort.prefab.GetComponent<IdentifiableActor>().identType = plort;
        
        PrismLibSaving.SetupForSaving(plort,referenceID);
        
        var prismPlort = new PrismPlort(plort, false);
        
        
        _createdPlort = prismPlort;
        PrismShortcuts._prismPlorts.Add(plort.ReferenceId,prismPlort);
        return _createdPlort;
    }
}


