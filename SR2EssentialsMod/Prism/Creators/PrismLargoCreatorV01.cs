using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.Damage;
using Il2CppMonomiPark.SlimeRancher.Slime;
using SR2E.Prism.Data;
using SR2E.Prism.Lib;
using UnityEngine.Localization;


namespace SR2E.Prism.Creators;

public class PrismLargoCreatorV01
{
    private PrismLargo _createdLargo;
    
    public PrismBaseSlime firstSlime;
    public PrismBaseSlime secondSlime;
    
    public PrismLargoMergeSettings largoMergeSettings = new PrismLargoMergeSettings();
    public string name => firstSlime._slimeDefinition.Name + secondSlime._slimeDefinition.Name;
    public string referenceID => "SlimeDefinition.Modded" + name;

    
    public LocalizedString customLocalized;
    public GameObject customBasePrefab = null;


    public PrismLargoCreatorV01(PrismNativeBaseSlime firstSlime, PrismNativeBaseSlime secondSlime)
    {
        this.firstSlime = firstSlime;
        this.secondSlime = secondSlime;
    }
    public PrismLargoCreatorV01(PrismNativeBaseSlime firstSlime, PrismBaseSlime secondSlime)
    {
        this.firstSlime = firstSlime;
        this.secondSlime = secondSlime;
    }
    public PrismLargoCreatorV01(PrismBaseSlime firstSlime, PrismBaseSlime secondSlime)
    {
        this.firstSlime = firstSlime;
        this.secondSlime = secondSlime;
    }
    public PrismLargoCreatorV01(PrismBaseSlime firstSlime, PrismNativeBaseSlime secondSlime)
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


        if (PrismLibLookup.DoesLargoComboExist(firstSlime, secondSlime)) return null;

        if (largoMergeSettings == null) largoMergeSettings = new PrismLargoMergeSettings();
        var firstSlimeDef = firstSlime.GetSlimeDefinition();
        var secondSlimeDef = secondSlime.GetSlimeDefinition();

        if (firstSlimeDef.IsLargo || secondSlimeDef.IsLargo)
            return null;

        firstSlimeDef.CanLargofy = true;
        secondSlimeDef.CanLargofy = true;

        SlimeDefinition baseLargo = null;
        if(firstSlimeDef.ReferenceId=="SlimeDefinition.Boom"||secondSlimeDef.referenceId=="SlimeDefinition.Boom") 
            baseLargo=LookupEUtil.largoTypes.GetEntryByRefID("SlimeDefinition.PinkBoom");
        
        else if(firstSlimeDef.ReferenceId=="SlimeDefinition.Phosphor"||secondSlimeDef.referenceId=="SlimeDefinition.Phosphor") 
            baseLargo=LookupEUtil.largoTypes.GetEntryByRefID("SlimeDefinition.PinkPhosphor");
        
        else if(firstSlimeDef.ReferenceId=="SlimeDefinition.Hyper"||secondSlimeDef.referenceId=="SlimeDefinition.Hyper") 
            baseLargo=LookupEUtil.largoTypes.GetEntryByRefID("SlimeDefinition.HyperPink");
        
        else if(firstSlimeDef.ReferenceId=="SlimeDefinition.Sloomber"||secondSlimeDef.referenceId=="SlimeDefinition.Sloomber") 
            baseLargo=LookupEUtil.largoTypes.GetEntryByRefID("SlimeDefinition.PinkSloomber");
        
        else if(firstSlimeDef.ReferenceId=="SlimeDefinition.Tabby"||secondSlimeDef.referenceId=="SlimeDefinition.Tabby") 
            baseLargo=LookupEUtil.largoTypes.GetEntryByRefID("SlimeDefinition.PinkTabby");
        
        else if(firstSlimeDef.ReferenceId=="SlimeDefinition.Crystal"||secondSlimeDef.referenceId=="SlimeDefinition.Crystal") 
            baseLargo=LookupEUtil.largoTypes.GetEntryByRefID("SlimeDefinition.PinkCrystal");
        
        else if(firstSlimeDef.ReferenceId=="SlimeDefinition.Saber"||secondSlimeDef.referenceId=="SlimeDefinition.Saber") 
            baseLargo=LookupEUtil.largoTypes.GetEntryByRefID("SlimeDefinition.SaberPink");
        
        else if(firstSlimeDef.ReferenceId=="SlimeDefinition.Honey"||secondSlimeDef.referenceId=="SlimeDefinition.Honey") 
            baseLargo=LookupEUtil.largoTypes.GetEntryByRefID("SlimeDefinition.PinkHoney");
        
        else if(firstSlimeDef.ReferenceId=="SlimeDefinition.Dervish"||secondSlimeDef.referenceId=="SlimeDefinition.Dervish") 
            baseLargo=LookupEUtil.largoTypes.GetEntryByRefID("SlimeDefinition.PinkDervish");
        
        else if(firstSlimeDef.ReferenceId=="SlimeDefinition.Hunter"||secondSlimeDef.referenceId=="SlimeDefinition.Hunter") 
            baseLargo=LookupEUtil.largoTypes.GetEntryByRefID("SlimeDefinition.PinkHunter");
        
        else if(firstSlimeDef.ReferenceId=="SlimeDefinition.Angler"||secondSlimeDef.referenceId=="SlimeDefinition.Angler") 
            baseLargo=LookupEUtil.largoTypes.GetEntryByRefID("SlimeDefinition.AnglerPink");
        
        else if(firstSlimeDef.ReferenceId=="SlimeDefinition.Dervish"||secondSlimeDef.referenceId=="SlimeDefinition.Dervish") 
            baseLargo=LookupEUtil.largoTypes.GetEntryByRefID("SlimeDefinition.PinkDervish");
        
        else if(firstSlimeDef.ReferenceId=="SlimeDefinition.Cotton"||secondSlimeDef.referenceId=="SlimeDefinition.Cotton") 
            baseLargo=LookupEUtil.largoTypes.GetEntryByRefID("SlimeDefinition.CottonPink");
        
        else if(firstSlimeDef.ReferenceId=="SlimeDefinition.Batty"||secondSlimeDef.referenceId=="SlimeDefinition.Batty") 
            baseLargo=LookupEUtil.largoTypes.GetEntryByRefID("SlimeDefinition.BattyPink");
        
        else if(firstSlimeDef.ReferenceId=="SlimeDefinition.Flutter"||secondSlimeDef.referenceId=="SlimeDefinition.Flutter") 
            baseLargo=LookupEUtil.largoTypes.GetEntryByRefID("SlimeDefinition.FlutterPink");
        
        else if(firstSlimeDef.ReferenceId=="SlimeDefinition.Twin"||secondSlimeDef.referenceId=="SlimeDefinition.Twin") 
            baseLargo=LookupEUtil.largoTypes.GetEntryByRefID("SlimeDefinition.PinkTwin");
        
        else if(firstSlimeDef.ReferenceId=="SlimeDefinition.Tangle"||secondSlimeDef.referenceId=="SlimeDefinition.Tangle") 
            baseLargo=LookupEUtil.largoTypes.GetEntryByRefID("SlimeDefinition.PinkTangle");
        
        else if(firstSlimeDef.ReferenceId=="SlimeDefinition.Ringtail"||secondSlimeDef.referenceId=="SlimeDefinition.Ringtail") 
            baseLargo=LookupEUtil.largoTypes.GetEntryByRefID("SlimeDefinition.RingtailPink");
        
        else
            baseLargo=LookupEUtil.largoTypes.GetEntryByRefID("SlimeDefinition.PinkRock");
        
        SlimeDefinition largoDef = Object.Instantiate(baseLargo);
        largoDef.BaseSlimes = new[]
        {
            firstSlimeDef, secondSlimeDef
        };
        largoDef.SlimeModules = new[]
        {
            firstSlimeDef.SlimeModules[0],secondSlimeDef.SlimeModules[0]
        };

        largoDef._pediaPersistenceSuffix = "modded"+firstSlimeDef.name.ToLower() + "_" + secondSlimeDef.name.ToLower() + "_largo";

        largoDef.referenceId = referenceID;

        if (customLocalized != null)
            largoDef.localizedName = customLocalized;
        else
            largoDef.localizedName = AddTranslation(firstSlimeDef.name + " " + secondSlimeDef.name + " Largo",
                "l." + largoDef._pediaPersistenceSuffix);


        largoDef.FavoriteToyIdents = new Il2CppReferenceArray<ToyDefinition>(PrismLibMerging.MergeFavoriteToys(firstSlime, secondSlime));

        largoDef.hideFlags = HideFlags.DontUnloadUnusedAsset;
        largoDef.Name = name;
        largoDef.name = name;
        largoDef.prefab = Object.Instantiate(baseLargo.prefab, prefabHolder.transform);
        largoDef.prefab.name = $"slime{name}";
        largoDef.prefab.GetComponent<Identifiable>().identType = largoDef;
        largoDef.prefab.GetComponent<SlimeEat>().SlimeDefinition = largoDef;
        largoDef.prefab.GetComponent<SlimeAppearanceApplicator>().SlimeDefinition = largoDef;
        largoDef.prefab.GetComponent<PlayWithToys>().SlimeDefinition = largoDef;
        largoDef.prefab.GetComponent<ReactToToyNearby>().SlimeDefinition = largoDef;
        if(!firstSlimeDef.prefab.HasComponent<RockSlimeRoll>()&&!secondSlimeDef.prefab.HasComponent<RockSlimeRoll>())
            largoDef.prefab.RemoveComponent<RockSlimeRoll>();
        if(!firstSlimeDef.prefab.HasComponent<DamagePlayerOnTouch>()&&!secondSlimeDef.prefab.HasComponent<DamagePlayerOnTouch>())
            largoDef.prefab.RemoveComponent<DamagePlayerOnTouch>();

        SlimeAppearance appearance = Object.Instantiate(baseLargo.AppearancesDefault[0]);
        largoDef.AppearancesDefault[0] = appearance;
        appearance.hideFlags = HideFlags.DontUnloadUnusedAsset;
        appearance.name = firstSlimeDef.AppearancesDefault[0].name + secondSlimeDef.AppearancesDefault[0].name;

        appearance._dependentAppearances = new[]
        {
            firstSlimeDef.AppearancesDefault[0], secondSlimeDef.AppearancesDefault[0]
        };

        bool firstFace = PrismLibMerging.ShouldUseFirstStructure(largoMergeSettings.face, !PrismLibMerging.GetLargoHasDefaultFace(firstSlime.GetSlimeAppearance()));
        if (firstFace)
            appearance._face = Object.Instantiate(firstSlime.GetSlimeAppearance()._face);
        else appearance._face = Object.Instantiate(secondSlime.GetSlimeAppearance()._face);
        var optimalPriortization = PrismLibMerging.GetOptimalV01(firstSlimeDef,secondSlimeDef);
        if (largoMergeSettings.baseColors==PrismColorMergeStrategy.Merge||
            (largoMergeSettings.baseColors == PrismColorMergeStrategy.Optimal && optimalPriortization==PrismThreeMergeStrategy.Merge))
        {
            var firstPalette = firstSlime.GetSlimeAppearance()._colorPalette;
            var secondPalette = secondSlime.GetSlimeAppearance()._colorPalette;
            appearance._splatColor = Color.Lerp(firstSlime.GetSlimeAppearance()._splatColor, secondSlime.GetSlimeAppearance()._splatColor, 0.5f);
            largoDef.color = Color.Lerp(firstSlimeDef.color, secondSlimeDef.color, 0.5f);
            appearance._colorPalette = new SlimeAppearance.Palette()
            {
                Ammo = Color.Lerp(firstPalette.Ammo, secondPalette.Ammo, 0.5f),
                Bottom = Color.Lerp(firstPalette.Bottom, secondPalette.Bottom, 0.5f),
                Middle = Color.Lerp(firstPalette.Middle, secondPalette.Middle, 0.5f),
                Top = Color.Lerp(firstPalette.Top, secondPalette.Top, 0.5f),
            };
        }
        else
        {
            SlimeAppearance prioritizedAppearance = null;
            switch (optimalPriortization)
            {
                case PrismThreeMergeStrategy.PrioritizeSecond :
                    prioritizedAppearance = secondSlime.GetSlimeAppearance();
                    largoDef.color = secondSlimeDef.color;
                    break;
                default:
                    prioritizedAppearance = firstSlime.GetSlimeAppearance();
                    largoDef.color = firstSlimeDef.color;
                    break;
            }
            appearance._splatColor = prioritizedAppearance._splatColor;
            appearance._colorPalette = new SlimeAppearance.Palette()
            {
                Ammo = prioritizedAppearance._colorPalette.Ammo,
                Bottom = prioritizedAppearance._colorPalette.Bottom, 
                Middle = prioritizedAppearance._colorPalette.Middle,
                Top = prioritizedAppearance._colorPalette.Top,
            };
        }
        appearance._structures = PrismLibMerging.MergeStructuresV01(appearance._dependentAppearances[0],
            appearance._dependentAppearances[1], largoMergeSettings,optimalPriortization);

        try
        {
            largoDef.Diet = PrismLibMerging.MergeDiet(firstSlimeDef.Diet, secondSlimeDef.Diet);
        }
        catch
        {
            switch (largoMergeSettings.body)
            {
                case PrismBFMergeStrategy.KeepSecond:
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

        PrismShortcuts.mainAppearanceDirector.RegisterDependentAppearances(largoDef, largoDef.AppearancesDefault[0]);
        PrismShortcuts.mainAppearanceDirector.UpdateChosenSlimeAppearance(largoDef, largoDef.AppearancesDefault[0]);

        
        PrismLibSaving.SetupForSaving(largoDef,largoDef.referenceId);
        
        //gameContext.SlimeDefinitions.RefreshIndexes();
        gameContext.SlimeDefinitions.RefreshDefinitions();
        
        IdentifiableType firstPlort = null;
        IdentifiableType secondPlort = null; 
        foreach (var pair in gameContext.SlimeDefinitions._largoDefinitionByBasePlorts)
        {
            if(pair.value!=largoDef) continue;
            firstPlort = pair.Key.Plort1;
            secondPlort = pair.Key.Plort2;
            break;
        }
        if(firstPlort!=null&&secondPlort!=null)
        {
            firstSlime.AddEatmapToSlime(PrismLibDiet.CreateEatmapEntry(SlimeEmotions.Emotion.AGITATION, 0.5f, null, secondPlort, largoDef), true);
            secondSlime.AddEatmapToSlime(PrismLibDiet.CreateEatmapEntry(SlimeEmotions.Emotion.AGITATION, 0.5f, null, firstPlort, largoDef), true);
            firstSlime.RefreshEatMap();
            secondSlime.RefreshEatMap();
        }



        if(largoMergeSettings.mergeComponents)
            PrismLibMerging.MergeComponentsV01(largoDef.prefab, firstSlimeDef.prefab, secondSlimeDef.prefab);

        
        if (firstSlime.GetIsNative())
            largoDef.Prism_AddToGroup(firstSlimeDef.Name+"LargoGroup");
        else
        {
            if (LookupEUtil.allIdentifiableTypeGroups.ContainsKey(firstSlimeDef.Name + "ModdedLargoGroup"))
                largoDef.Prism_AddToGroup(firstSlimeDef.Name + "ModdedLargoGroup");
            else
            {
                var creator = new PrismIdentifiableTypeGroupCreatorV01(firstSlimeDef.Name + "ModdedLargoGroup", PrismShortcuts.emptyTranslation);
                creator.memberTypes = new List<IdentifiableType>() { largoDef };
                var group = creator.CreateIdentifiableTypeGroup();
                group.AddToGroup("EdibleSlimeGroup");
                group.AddToGroup("LargoGroup");
                if(firstSlime.IsInImmediateGroup("SlimesSinkInShallowWaterGroup")&&secondSlime.IsInImmediateGroup("SlimesSinkInShallowWaterGroup"))
                    group.AddToGroup("SlimesSinkInShallowWaterGroup");
            }
        }
        
        if (secondSlime.GetIsNative())
            largoDef.Prism_AddToGroup(secondSlimeDef.Name+"LargoGroup");
        else
        {
            if (LookupEUtil.allIdentifiableTypeGroups.ContainsKey(secondSlimeDef.Name + "ModdedLargoGroup"))
                largoDef.Prism_AddToGroup(secondSlimeDef.Name + "ModdedLargoGroup");
            else
            {
                var creator = new PrismIdentifiableTypeGroupCreatorV01(secondSlimeDef.Name + "ModdedLargoGroup", PrismShortcuts.emptyTranslation);
                creator.memberTypes = new List<IdentifiableType>() { largoDef };
                var group = creator.CreateIdentifiableTypeGroup();
                group.AddToGroup("EdibleSlimeGroup");
                group.AddToGroup("LargoGroup");
                if(firstSlime.IsInImmediateGroup("SlimesSinkInShallowWaterGroup")&&secondSlime.IsInImmediateGroup("SlimesSinkInShallowWaterGroup"))
                    group.AddToGroup("SlimesSinkInShallowWaterGroup");
            }
        }
        
        
        var prismLargo = new PrismLargo(largoDef, false);
        prismLargo.RefreshEatMap();
        
        //if(plort!=null)
          //  PrismLibDiet.AddEatProduction(prismLargo, plort);
        
        _createdLargo = prismLargo;
        PrismShortcuts._prismLargoBases.Add(_createdLargo,(firstSlime,secondSlime));
        PrismShortcuts._prismLargos.Add(largoDef.ReferenceId,_createdLargo);
        return _createdLargo;
    }   
}