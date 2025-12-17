using SR2E.Expansion;
using SR2E.Prism;
using SR2E.Prism.Creators;
using SR2E.Prism.Lib;
using SR2E.Prism.Data;
using Object = UnityEngine.Object;

namespace PrismaticSlime;

public class SlimeMain : SR2EExpansionV3
{
    public static Color32 vacColor = new Color32(199, 43, 255, 255);
    public static Color32 topColor = new Color32(255, 22, 55, 255);
    public static Color32 middleColor = new Color32(145, 23, 207, 255);
    public static Color32 bottomColor = new Color32(46, 1, 200, 255);
    public static PrismBaseSlime slime;
    public static PrismPlort plort;
    public static PrismGordo gordo;
    public static PrismIdentifiablePediaEntry pedia;

    public override void AfterSystemContext(SystemContext systemContext)
    {
        MiscEUtil.AddCustomBouncySprite(EmbeddedResourceEUtil.LoadSprite("Assets.iconPlortPrismatic.png"));
    }

    public override void OnInitializeMelon()
    {
        AddLanguages(EmbeddedResourceEUtil.LoadString("translations.csv"));
    }
    public override void OnPrismCreateAdditions()
    {
        //Create PrismaticPlort
        var prismaticPlortCreator = new PrismPlortCreatorV01(
            "Prismatic",
            EmbeddedResourceEUtil.LoadSprite("Assets.iconPlortPrismatic.png"),
            AddTranslationFromSR2E("prismatic.plort", "l.prismaticPlort"));
        prismaticPlortCreator.moddedMarketData = new PrismMarketData(27f, 85f); //Controls the market values
        prismaticPlortCreator.customBasePrefab = PrismNativePlort.Tabby.GetPrismPlort().GetPrefab();
        prismaticPlortCreator.vacColor = vacColor; // The color of the plort in the vac
        plort = prismaticPlortCreator.CreatePlort();
        
        //Some color adjustments one the plort
        plort.SetPlortBaseColors(topColor, middleColor, bottomColor);
        
        //Images
        plort.GetPrefab().GetComponent<MeshRenderer>().material.SetTexture("_StripeTexture", EmbeddedResourceEUtil.LoadTexture2D("Assets.prismaticPlort_falloff_map.png"));
        
        //Create Prismatic Slime
        var prismaticSlimeCreator = new PrismBaseSlimeCreatorV01(
            "Prismatic",
            EmbeddedResourceEUtil.LoadSprite("Assets.iconSlimePrismatic.png"),
            AddTranslationFromSR2E("prismatic.slime", "l.prismaticSlime"));
        prismaticSlimeCreator.disableVaccable = false; // Controls whether a slime is vaccable
        prismaticSlimeCreator.plort = plort; // The plort of the slime, can either be an IdentifiableType or PrismPlort
        prismaticSlimeCreator.vacColor = vacColor; // The color of the slime in the vac and its splat color
        prismaticSlimeCreator.canLargofy = true; // Can this slime have largo forms, used for createAllLargos, gets overwritten if you create one manually
        prismaticSlimeCreator.createAllLargos = true; // Automatically create all largos, requires canLargofy on both slimes
        prismaticSlimeCreator.disableAutoModdedLargos = false; // Disables autolargos for combining between 2 non-native slimes. Do this if your slime has custom components that break in auto largo
        prismaticSlimeCreator.customBaseAppearance = PrismNativeBaseSlime.Cotton.GetPrismBaseSlime().GetSlimeAppearance(); // If not set, Pink is default, it will duplicate it
        prismaticSlimeCreator.customBasePrefab = PrismNativeBaseSlime.Cotton.GetPrismBaseSlime().GetPrefab();; // If not set, Pink is default, it will duplicate it
        
        slime = prismaticSlimeCreator.CreateSlime();
        
        //Some color adjustments on the slime
        slime.SetSlimeBaseColorsSpecific(topColor, middleColor, bottomColor, middleColor, 0, 0, false, 0);
        slime.SetSlimeBaseColorsSpecific(topColor, middleColor, bottomColor, middleColor, 0, 0, false, 2);
        slime.SetSlimeBaseColorsSpecific(topColor, middleColor, bottomColor, middleColor, 0, 0, false, 3);
        slime.SetSlimeBaseColorsSpecific(topColor, middleColor, bottomColor, middleColor, 0, 0, false, 4);
        
        //Some food management
        slime.AddFoodGroup(PrismLibLookup.fruitFoodGroup); //Adds what the slime can eat
        // Make cotton slimes edible
        slime.AddAdditionalFood(PrismNativeBaseSlime.Cotton.GetPrismBaseSlime());
        slime.AddFavoriteFood(PrismNativeBaseSlime.Cotton.GetPrismBaseSlime());
        slime.RefreshEatMap(); //Refreshes everything

        //Make it spawn
        var prismaticSpawnLocations = new PrismSpawnLocations[] { PrismSpawnLocations.RainbowFields };
        PrismLibSpawning.MakeSpawnable(slime, PrismSpawnerActiveTime.AllTheTime, prismaticSpawnLocations,0.3f, PrismSpawnerType.Slime);
        
        //Create Pedia entry
        var prismaticPediaCreator = new PrismIdentifiablePediaEntryCreatorV01(
            slime, PrismPediaCategoryType.Slimes,
            AddTranslationFromSR2E("prismatic.pedia.intro"));
        prismaticPediaCreator.factSet = PrismPediaFactSetType.Slime; //What set of facts should be displayed
        prismaticPediaCreator.details = //Create details, you can add absolutely anything you want
            PrismPediaDetail.From(
                PrismPediaDetail.Create(PrismPediaDetailType.Slimeology,AddTranslationFromSR2E("prismatic.pedia.slimeology")),
                PrismPediaDetail.Create(PrismPediaDetailType.RancherRisks,AddTranslationFromSR2E("prismatic.pedia.rancherrisks")),
                PrismPediaDetail.Create(PrismPediaDetailType.Plortonomics,AddTranslationFromSR2E("prismatic.pedia.plortonomics"))
            );
        //Custom additional fact
        prismaticPediaCreator.additionalFacts = PrismPediaAdditionalFact.From(
            PrismPediaAdditionalFact.Create(
                AddTranslationFromSR2E("prismatic.pedia.fact.purple.title"), 
            AddTranslationFromSR2E("prismatic.pedia.fact.purple.description"), 
            EmbeddedResourceEUtil.LoadSprite("Assets.icon.png"))
            );
        pedia = prismaticPediaCreator.CreateIdentifiablePediaEntry();
        
        
        //Create largo manually
        //var prismaticPinkLargoCreator = new PrismLargoCreator(prismaticSlime, PrismNativeBaseSlime.Pink);
        //prismaticPinkLargoCreator.largoMergeSettings = new PrismLargoMergeSettings();
        //var prismaticPinkLargo = prismaticPinkLargoCreator.CreateLargo();
        
        
        
        //Create gordo
        var prismaticGordoCreator = new PrismGordoCreatorV01(
            slime,
            EmbeddedResourceEUtil.LoadSprite("Assets.iconSlimePrismatic.png"),
            AddTranslationFromSR2E("prismatic.gordo"));
        gordo = prismaticGordoCreator.CreateGordo();
        gordo.SetRequiredBait(LookupEUtil.fruitFoodTypes.GetEntryByRefID("IdentifiableType.PogoFruit"));
    }



}
