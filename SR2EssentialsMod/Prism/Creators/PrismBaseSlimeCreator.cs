using Cotton;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Linq;
using SR2E.Cotton;
using SR2E.Prism.Data;
using SR2E.Prism.Lib;
using UnityEngine.Localization;

namespace SR2E.Prism.Creators;

public class PrismBaseSlimeCreator
{
    private PrismBaseSlime _createdSlime;
    
    
    public string name;
    public Sprite icon;
    public LocalizedString localized;
    public string referenceID => "SlimeDefinition.Modded" + name;

    public PrismPlort plort = null;
    public GameObject customBasePrefab = null;
    public SlimeAppearance customBaseAppearance = null;
    public bool vaccable = true;
    public bool canLargofy = false;
    public bool createAllLargos = false;
    public Color32 vacColor = new Color32(0,0,0,255);

    
    
    public PrismBaseSlimeCreator(string name, Sprite icon, LocalizedString localized)
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

    
    
    
    
    
    
    
    public PrismBaseSlime CreateSlime()
    {
        if (!IsValid()) return null;
        if (_createdSlime != null) return _createdSlime;

        var slimeDef = Object.Instantiate(CottonSlimes.GetSlime("Pink"));
        Object.DontDestroyOnLoad(slimeDef);
        slimeDef.hideFlags = HideFlags.HideAndDontSave;
        slimeDef.Name = name;
        slimeDef.name = name;
        slimeDef.AppearancesDefault = new Il2CppReferenceArray<SlimeAppearance>(1);

        var baseAppearance = customBaseAppearance;
        if (baseAppearance == null) baseAppearance = Get<SlimeAppearance>("PinkDefault");
        SlimeAppearance appearance = Object.Instantiate(baseAppearance);
        Object.DontDestroyOnLoad(appearance);
        appearance.name = name+"Default";
        appearance._icon = icon;
        slimeDef.AppearancesDefault = slimeDef.AppearancesDefault.AddToNew(appearance);
        if (slimeDef.AppearancesDefault[0] == null)
        {
            slimeDef.AppearancesDefault[0] = appearance;
        }

        for (int i = 0; i < slimeDef.AppearancesDefault[0].Structures.Count - 1; i++)
        {
            SlimeAppearanceStructure a = slimeDef.AppearancesDefault[0].Structures[i];
            var a2 = new SlimeAppearanceStructure(a);
            slimeDef.AppearancesDefault[0].Structures[i] = a2;
            if (a.DefaultMaterials.Count != 0)
            {
                a2.DefaultMaterials[0] = Object.Instantiate(a.DefaultMaterials[0]);
            }
        }
        
        
        var basePrefab = customBasePrefab;
        if (basePrefab == null) basePrefab = CottonSlimes.GetSlime("Pink").prefab;
        slimeDef.prefab = CreatePrefab("slime"+name, basePrefab);
        if(slimeDef.prefab.HasComponent<SlimeEat>())
            slimeDef.prefab.GetComponent<SlimeEat>().SlimeDefinition = slimeDef; 
        slimeDef.prefab.GetComponent<SlimeAppearanceApplicator>().SlimeDefinition = slimeDef;
        slimeDef.prefab.GetComponent<IdentifiableActor>().identType = slimeDef;
        
        SlimeDiet slimeDiet = PrismLibDiet.CreateNewDiet();
        slimeDef.Diet = slimeDiet;
        vacColor.a = 255;
        slimeDef.color = vacColor;
        slimeDef.icon = icon;

        if(!gameContext.SlimeDefinitions.Slimes.Contains(slimeDef))
            gameContext.SlimeDefinitions.Slimes = gameContext.SlimeDefinitions.Slimes.AddToNew(slimeDef);
        if(!gameContext.SlimeDefinitions._slimeDefinitionsByIdentifiable.ContainsKey(slimeDef))
            gameContext.SlimeDefinitions._slimeDefinitionsByIdentifiable.Add(slimeDef, slimeDef);

        CottonLibrary.Saving.INTERNAL_SetupLoadForIdent(referenceID, slimeDef);

        if (canLargofy&&createAllLargos)
        {
            /*
            CottonLibrary.createLargoActions.Add(() =>
            {
                foreach (var slime in CottonLibrary.baseSlimes.GetAllMembers().ToList())
                    if (slime.TryCast<SlimeDefinition>().CanLargofy)
                        CottonSlimes.CreateCompleteLargo(slimeDef, slime.Cast<SlimeDefinition>(), largoSettings);
            });*/
        }

        if(vaccable) slimeDef.AddToGroup("VaccableBaseSlimeGroup");
        else slimeDef.AddToGroup("BaseSlimeGroup");
        
        
        slimeDef.AppearancesDefault[0]._colorPalette = new SlimeAppearance.Palette
            { Ammo = vacColor, Bottom = vacColor, Middle = vacColor, Top = vacColor };
        slimeDef.AppearancesDefault[0]._splatColor = vacColor;
        
        slimeDef.CanLargofy = canLargofy;
        
        slimeDef.localizedName = localized;
        slimeDef._pediaPersistenceSuffix = "modded"+name.ToLower()+"_slime";
        
        var prismSlime = new PrismBaseSlime(slimeDef, false);
        
        if(plort!=null)
            PrismLibDiet.AddEatProduction(prismSlime, plort);
        
        _createdSlime = prismSlime;
        PrismShortcuts._prismBaseSlimes.Add(slimeDef,_createdSlime);
        return _createdSlime;
    }

}