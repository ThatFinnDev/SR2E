using Cotton;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.Damage;
using Il2CppSystem.Linq;
using SR2E.Cotton;
using SR2E.Prism.Data;
using SR2E.Prism.Enums;
using SR2E.Prism.Lib;
using UnityEngine.Localization;


namespace SR2E.Prism.Creators;

public class PrismLargoCreator
{
    private PrismLargo _createdLargo;
    
    public PrismBaseSlime firstSlime;
    public PrismBaseSlime secondSlime;
    public PrismLargoMergeSettings largoMergeSettings = new PrismLargoMergeSettings();
    
    public string name => firstSlime._slimeDefinition.Name + secondSlime._slimeDefinition.Name;
    public string referenceID => "SlimeDefinition.Modded" + name;

    
    public LocalizedString customLocalized;
    
    
    
    public GameObject customBasePrefab = null;

    
    
    public PrismLargoCreator(PrismBaseSlime firstSlime, PrismBaseSlime secondSlime)
    {
        this.firstSlime = firstSlime;
        this.secondSlime = secondSlime;
    }
    
    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        for (int i = 0; i < name.Length; i++)
            if (!((name[i] >= 'A' && name[i] <= 'Z') || (name[i] >= 'a' && name[i] <= 'z')))
                return false;
        if (firstSlime == null | secondSlime == null) return false;
        if (customBasePrefab != null)
        {
            if (!customBasePrefab.HasComponent<SlimeAppearanceApplicator>()) return false;
            if (!customBasePrefab.HasComponent<IdentifiableActor>()) return false;
        }
        return true;
    }

    
    
    
    
    
    
    
    public PrismLargo CreateLargo()
    {
        if (!IsValid()) return null;
        if (_createdLargo != null) return _createdLargo;

        if (CottonSlimes.DoesLargoComboExist(firstSlime, secondSlime)) return null;
        

        var firstSlimeDef = firstSlime.GetSlimeDefinition();
        var secondSlimeDef = secondSlime.GetSlimeDefinition();
        if (firstSlimeDef.IsLargo || secondSlimeDef.IsLargo)
            return null;

        SlimeDefinition baseLargo = CottonSlimes.GetLargo("PinkRock");
        
        SlimeDefinition largoDef = Object.Instantiate(baseLargo);
        largoDef.BaseSlimes = new[]
        {
            firstSlimeDef, secondSlimeDef
        };
        largoDef.SlimeModules = new[]
        {
            Get<GameObject>("moduleSlime" + firstSlimeDef.name), Get<GameObject>("moduleSlime" + secondSlimeDef.name)
        };

        largoDef._pediaPersistenceSuffix = "modded"+firstSlimeDef.name.ToLower() + "_" + secondSlimeDef.name.ToLower() + "_largo";

        largoDef.referenceId = referenceID;

        MelonLogger.Error(largoDef.referenceId);

        if (customLocalized != null)
            largoDef.localizedName = customLocalized;
        else
            largoDef.localizedName = AddTranslation(firstSlimeDef.name + " " + secondSlimeDef.name + " Largo",
                "l." + largoDef._pediaPersistenceSuffix);


        largoDef.FavoriteToyIdents = new Il2CppReferenceArray<ToyDefinition>(PrismLibMerging.MergeFavoriteToys(firstSlime, secondSlime));

        Object.DontDestroyOnLoad(largoDef);
        largoDef.hideFlags = HideFlags.HideAndDontSave;
        largoDef.name = name;

        largoDef.prefab = Object.Instantiate(baseLargo.prefab, prefabHolder.transform);
        largoDef.prefab.name = $"slime{name}";
        largoDef.prefab.GetComponent<Identifiable>().identType = largoDef;
        largoDef.prefab.GetComponent<SlimeEat>().SlimeDefinition = largoDef;
        largoDef.prefab.GetComponent<SlimeAppearanceApplicator>().SlimeDefinition = largoDef;
        largoDef.prefab.GetComponent<PlayWithToys>().SlimeDefinition = largoDef;
        largoDef.prefab.GetComponent<ReactToToyNearby>().SlimeDefinition = largoDef;
        largoDef.prefab.RemoveComponent<RockSlimeRoll>();
        largoDef.prefab.RemoveComponent<DamagePlayerOnTouch>();

        SlimeAppearance appearance = Object.Instantiate(baseLargo.AppearancesDefault[0]);
        largoDef.AppearancesDefault[0] = appearance;
        Object.DontDestroyOnLoad(appearance);
        appearance.name = firstSlimeDef.AppearancesDefault[0].name + secondSlimeDef.AppearancesDefault[0].name;

        appearance._dependentAppearances = new[]
        {
            firstSlimeDef.AppearancesDefault[0], secondSlimeDef.AppearancesDefault[0]
        };

        appearance._structures = PrismLibMerging.MergeStructures(appearance._dependentAppearances[0],
            appearance._dependentAppearances[1], largoMergeSettings);

        try
        {
            largoDef.Diet = PrismLibMerging.MergeDiet(firstSlimeDef.Diet, secondSlimeDef.Diet);
        }
        catch
        {
            switch (largoMergeSettings.body)
            {
                case PrismTwoMergeStrategy.KeepSecond:
                    largoDef.Diet = secondSlimeDef.Diet;
                    MelonLogger.BigError("Largo Error",
                        "Failed to merge diet, and largo settings are incorrectly set! Defaulting to slime 2's diet.");
                    break;
                default:
                    largoDef.Diet = firstSlimeDef.Diet;
                    MelonLogger.BigError("Largo Error",
                        "Failed to merge diet, and largo settings are incorrectly set! Defaulting to slime 1's diet.");
                    break;
            }
        }
        largoDef.Diet.RefreshEatMap(CottonLibrary.slimeDefinitions, largoDef);

        gameContext.SlimeDefinitions.Slimes = gameContext.SlimeDefinitions.Slimes.AddToNew(largoDef);
        gameContext.SlimeDefinitions._slimeDefinitionsByIdentifiable.TryAdd(largoDef, largoDef);
        CottonLibrary.mainAppearanceDirector.RegisterDependentAppearances(largoDef, largoDef.AppearancesDefault[0]);
        CottonLibrary.mainAppearanceDirector.UpdateChosenSlimeAppearance(largoDef, largoDef.AppearancesDefault[0]);

        largoDef.AddToGroup("LargoGroup");
        largoDef.AddToGroup("SlimesGroup");
        CottonLibrary.Saving.INTERNAL_SetupLoadForIdent(largoDef.referenceId, largoDef);

        firstSlime.RefreshEatMap();
        secondSlime.RefreshEatMap();


        gameContext.SlimeDefinitions.RefreshDefinitions();
        gameContext.SlimeDefinitions.RefreshIndexes();

        if(largoMergeSettings.mergeComponents)
            PrismLibMerging.MergeComponents(largoDef.prefab, firstSlimeDef.prefab, secondSlimeDef.prefab);
        
        
        
        
        var prismLargo = new PrismLargo(largoDef, false);
        PrismLibDiet.RefreshEatMap(prismLargo);
        
        //if(plort!=null)
         //   PrismLibDiet.AddEatProduction(prismLargo, plort);
        
        _createdLargo = prismLargo;
        PrismShortcuts._prismLargoBases.Add(_createdLargo,(firstSlime,secondSlime));
        PrismShortcuts._prismLargos.Add(largoDef,_createdLargo);
        return _createdLargo;
}