using System.Linq;
using Cotton;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Linq;
using SR2E.Cotton;
using SR2E.Prism.Data;
using SR2E.Prism.Enums;
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
    public bool disableSinkInShallowWater = true;
    public bool disableEdibleByTarrs = true;
    public bool disableVaccable = true;
    
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

        var slimeDef = Object.Instantiate( PrismNativeBaseSlime.Pink.GetPrismBaseSlime().GetSlimeDefinition());
        Object.DontDestroyOnLoad(slimeDef);
        slimeDef.hideFlags = HideFlags.HideAndDontSave;
        slimeDef.Name = name;
        slimeDef.name = name;
        slimeDef.AppearancesDefault = new Il2CppReferenceArray<SlimeAppearance>(0);

        var baseAppearance = customBaseAppearance;
        if (baseAppearance == null) baseAppearance = PrismNativeBaseSlime.Pink.GetPrismBaseSlime().GetSlimeAppearance();
        SlimeAppearance appearance = Object.Instantiate(baseAppearance);
        Object.DontDestroyOnLoad(appearance);
        appearance.name = name+"Default";
        appearance._icon = icon;
        slimeDef.AppearancesDefault = slimeDef.AppearancesDefault.AddToNew(appearance);
        if (slimeDef.AppearancesDefault[0] == null)
        {
            slimeDef.AppearancesDefault[0] = appearance;
        }

        var newSlimeAppearanceStructure = new List<SlimeAppearanceStructure>();
        foreach (var slimeAppearanceStructure in baseAppearance.Structures)
        {
            var newStructure = new SlimeAppearanceStructure(slimeAppearanceStructure);
            
            var newDefaultMaterials = new List<Material>();
            foreach (var mat in slimeAppearanceStructure.DefaultMaterials)
                newDefaultMaterials.Add(Object.Instantiate(mat));
            newStructure.DefaultMaterials = newDefaultMaterials.ToArray();

            if(slimeAppearanceStructure.Element!=null)
            {
                newStructure.Element = Object.Instantiate(slimeAppearanceStructure.Element);
                var newElementPrefabs = new List<SlimeAppearanceObject>();
                foreach (var prefab in newStructure.Element.Prefabs)
                {
                    var prefabCopy = CreatePrefab("copy_" + prefab.name, prefab.gameObject);
                    newElementPrefabs.Add(prefabCopy.GetComponent<SlimeAppearanceObject>());
                }

                newStructure.Element.Prefabs = newElementPrefabs.ToArray();
            }

            if (slimeAppearanceStructure.ElementMaterials != null)
            {
                var newElementMaterials = new List<SlimeAppearanceMaterials>();
                foreach (var elementMaterial in slimeAppearanceStructure.ElementMaterials)
                {
                    var newElementMaterial = new SlimeAppearanceMaterials();
                    newElementMaterial.OverrideDefaults = elementMaterial.OverrideDefaults;
                
                    var newElementMaterialsMaterials = new List<Material>();
                    foreach (var mat in elementMaterial.Materials)
                        newElementMaterialsMaterials.Add(Object.Instantiate(mat));
                    newElementMaterial.Materials = newElementMaterialsMaterials.ToArray();
                    newElementMaterials.Add(newElementMaterial);
                }
                newStructure.ElementMaterials = newElementMaterials.ToArray();
            }
            newSlimeAppearanceStructure.Add(newStructure);
        }
        appearance.Structures = newSlimeAppearanceStructure.ToArray();

        if (baseAppearance._face != null) appearance._face = Object.Instantiate(baseAppearance._face);
        if(baseAppearance._idleAnimationOverride!=null) appearance._idleAnimationOverride = Object.Instantiate(baseAppearance._idleAnimationOverride);
        if (baseAppearance._crystalAppearance != null) appearance._crystalAppearance = Object.Instantiate(baseAppearance._crystalAppearance);
        if (baseAppearance._deathAppearance != null) appearance._deathAppearance = Object.Instantiate(baseAppearance._deathAppearance);
        if (baseAppearance._explosionAppearance != null) appearance._explosionAppearance = Object.Instantiate(baseAppearance._explosionAppearance);
        if (baseAppearance._glintAppearance != null) appearance._glintAppearance = Object.Instantiate(baseAppearance._glintAppearance);
        if (baseAppearance._qubitAppearance != null) appearance._qubitAppearance = Object.Instantiate(baseAppearance._qubitAppearance);
        if (baseAppearance._roarAppearance != null) appearance._roarAppearance = Object.Instantiate(baseAppearance._roarAppearance);
        if (baseAppearance._shockedAppearance != null) appearance._shockedAppearance = Object.Instantiate(baseAppearance._shockedAppearance);
        if (baseAppearance._stoneFormAppearance != null) appearance._stoneFormAppearance = Object.Instantiate(baseAppearance._stoneFormAppearance);
        if (baseAppearance._tornadoAppearance != null) appearance._tornadoAppearance = Object.Instantiate(baseAppearance._tornadoAppearance);
        if (baseAppearance._vineAppearance != null) appearance._vineAppearance = Object.Instantiate(baseAppearance._vineAppearance);
        if (baseAppearance._wingFlapAnimationOverride != null) appearance._wingFlapAnimationOverride = Object.Instantiate(baseAppearance._wingFlapAnimationOverride);
        if (baseAppearance._biteAnimationOverride != null) appearance._biteAnimationOverride = Object.Instantiate(baseAppearance._biteAnimationOverride);
            
        
        
        /*for (int i = 0; i < slimeDef.AppearancesDefault[0].Structures.Count - 1; i++)
        {
            SlimeAppearanceStructure a = slimeDef.AppearancesDefault[0].Structures[i];
            var a2 = new SlimeAppearanceStructure(a);
            slimeDef.AppearancesDefault[0].Structures[i] = a2;
            for (int j = 0; j < a2.DefaultMaterials.Count; j++)
            {
                a2.DefaultMaterials[j] = Object.Instantiate(a.DefaultMaterials[j]);
            }
        }*/
        
        
        var basePrefab = customBasePrefab;
        if (basePrefab == null) basePrefab = PrismNativeBaseSlime.Pink.GetPrismBaseSlime().GetPrefab();
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

        if(!disableVaccable) slimeDef.AddToGroup("VaccableBaseSlimeGroup");
        slimeDef.AddToGroup("SmallSlimeGroup");
        if(!disableEdibleByTarrs)
            slimeDef.AddToGroup("EdibleSlimeGroup");
        slimeDef.AddToGroup("IdentifiableTypesGroup");
        if(!disableSinkInShallowWater) slimeDef.AddToGroup("SlimesSinkInShallowWaterGroup");
        
        
        slimeDef.AppearancesDefault[0]._colorPalette = new SlimeAppearance.Palette
            { Ammo = vacColor, Bottom = vacColor, Middle = vacColor, Top = vacColor };
        slimeDef.AppearancesDefault[0]._splatColor = vacColor;
        
        slimeDef.CanLargofy = canLargofy;
        
        slimeDef.localizedName = localized;
        slimeDef._pediaPersistenceSuffix = "modded"+name.ToLower()+"_slime";

        if (!disableEdibleByTarrs)
            PrismNativeBaseSlime.Tarr.GetPrismBaseSlime().RefreshEatMap();
        var prismSlime = new PrismBaseSlime(slimeDef, false);
        
        if (canLargofy&&createAllLargos)
        {
            CottonLibrary.createLargoActions.Add(() =>
            {
                foreach (var slime in CottonLibrary.baseSlimes.GetAllMembers().ToList())
                {
                    try
                    {
                        var otherSlimeDef = slime.TryCast<SlimeDefinition>();
                        if(otherSlimeDef==slimeDef) continue;
                        if (otherSlimeDef.CanLargofy)
                        {
                            var largoCreator = new PrismLargoCreator(prismSlime, otherSlimeDef.GetPrismBaseSlime()).CreateLargo();
                        }
                    }
                    catch (Exception e)
                    {
                        MelonLogger.Error(e);
                        MelonLogger.Error("Error creating largo with "+slime.ReferenceId);
                    }
                }
            });
        }

        
        
        if(plort!=null)
            PrismLibDiet.AddEatProduction(prismSlime, plort);
        
        _createdSlime = prismSlime;
        PrismShortcuts._prismBaseSlimes.Add(slimeDef,_createdSlime);
        return _createdSlime;
    }

}