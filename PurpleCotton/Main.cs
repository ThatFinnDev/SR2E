using SR2E.Expansion;
using SR2E.Prism;
using SR2E.Prism.Creators;
using SR2E.Prism.Lib;
using SR2E.Prism.Data;
using Object = UnityEngine.Object;

namespace PurpleCotton;

public static class BuildInfo
{
    public const string Name = "Purple Cotton Slime"; // Name of the Expansion. 
    public const string Description = "Adds a purple Cotton slimes"; // Description for the Expansion.
    public const string Author = "Thatfinn"; // Author of the Expansion.
    public const string CoAuthors = null; // CoAuthor(s) of the Expansion.  (optional, set as null if none)
    public const string Contributors = null; // Contributor(s) of the Expansion.  (optional, set as null if none)
    public const string Company = null; // Company that made the Expansion.  (optional, set as null if none)
    public const string Version = "1.0.0"; // Version of the Expansion.
    public const string DownloadLink = null; // Download Link for the Expansion.  (optional, set as null if none)
    public const string SourceCode = null; // Source Link for the Expansion.  (optional, set as null if none)
    public const string Nexus = null; // Nexus Link for the Expansion.  (optional, set as null if none)
    public const bool UsePrism = true; // Enable if you use Prism
}

public class SlimeMain : SR2EExpansionV2
{
    public static Color32 vacColor_purplecotton = new Color32(199, 43, 255, 255);
    public static Color32 topColor_purplecotton = new Color32(168, 52, 235, 255);
    public static Color32 middleColor_purplecotton = new Color32(145, 23, 207, 255);
    public static Color32 bottomColor_purplecotton = new Color32(18, 1, 48, 255);
    public static PrismBaseSlime purplecottonSlime;
    public static PrismPlort purplecottonPlort;
    public static PrismLargo purplecottonPinkLargo;
    public static PrismIdentifiablePediaEntry purplecottonPedia;

    public override void OnNormalInitializeMelon()
    {
        AddLanguages(EmbeddedResourceEUtil.LoadString("translations.csv"));
    }
    public override void OnPrismCreateAdditions()
    {
        //Create PurplecottonPlort
        var purplecottonPlortCreator = new PrismPlortCreatorV01(
            "Purplecotton",
            EmbeddedResourceEUtil.LoadSprite("Assets.iconPlortPurplecotton.png"),
            AddTranslationFromSR2E("purplecotton.plort", "l.purplecottonPlort"));
        purplecottonPlortCreator.moddedMarketData = new PrismMarketData(27f, 85f); //Controls the market values
        purplecottonPlortCreator.customBasePrefab = PrismNativePlort.Tabby.GetPrismPlort().GetPrefab();
        purplecottonPlortCreator.vacColor = vacColor_purplecotton; // The color of the plort in the vac
        purplecottonPlort = purplecottonPlortCreator.CreatePlort();
        
        //Some color adjustments one the plort
        purplecottonPlort.SetPlortBaseColors(topColor_purplecotton, middleColor_purplecotton, bottomColor_purplecotton);
        
        //Images
        purplecottonPlort.GetPrefab().GetComponent<MeshRenderer>().material.SetTexture("_StripeTexture", EmbeddedResourceEUtil.LoadTexture2D("Assets.purplecottonPlort_falloff_map.png"));
        
        //Create Purplecotton Slime
        var purplecottonSlimeCreator = new PrismBaseSlimeCreatorV01(
            "Purplecotton",
            EmbeddedResourceEUtil.LoadSprite("Assets.iconSlimePurplecotton.png"),
            AddTranslationFromSR2E("purplecotton.slime", "l.purplecottonSlime"));
        purplecottonSlimeCreator.disableVaccable = false; // Controls whether a slime is vaccable
        purplecottonSlimeCreator.plort = purplecottonPlort; // The plort of the slime, can either be an IdentifiableType or PrismPlort
        purplecottonSlimeCreator.vacColor = vacColor_purplecotton; // The color of the slime in the vac and its splat color
        purplecottonSlimeCreator.canLargofy = true; // Can this slime have largo forms, used for createAllLargos, gets overwritten if you create one manually
        purplecottonSlimeCreator.createAllLargos = true; // Automatically create all largos, requires canLargofy on both slimes
        purplecottonSlimeCreator.disableAutoModdedLargos = false; // Disables autolargos for combini between 2 non-native slimes. Do this if your slime has custom components that break in auto largo
        //purplecottonSlimeCreator.customAutoLargoMergeSettings = new PrismLargoMergeSettings(); //Custom auto largo settings
        purplecottonSlimeCreator.customBaseAppearance = PrismNativeBaseSlime.Cotton.GetPrismBaseSlime().GetSlimeAppearance(); // If not set, Pink is default, it will duplicate it
        purplecottonSlimeCreator.customBasePrefab = PrismNativeBaseSlime.Cotton.GetPrismBaseSlime().GetPrefab();; // If not set, Pink is default, it will duplicate it
        //if(!purplecottonSlimeCreator.IsValid()) DoStuff(); // Optionally check if valid
        purplecottonSlime = purplecottonSlimeCreator.CreateSlime();
        
        //Some color adjustments on the slime
        purplecottonSlime.SetSlimeBaseColorsSpecific(topColor_purplecotton, middleColor_purplecotton, bottomColor_purplecotton, middleColor_purplecotton, 0, 0, false, 0);
        purplecottonSlime.SetSlimeBaseColorsSpecific(topColor_purplecotton, middleColor_purplecotton, bottomColor_purplecotton, middleColor_purplecotton, 0, 0, false, 2);
        purplecottonSlime.SetSlimeBaseColorsSpecific(topColor_purplecotton, middleColor_purplecotton, bottomColor_purplecotton, middleColor_purplecotton, 0, 0, false, 3);
        purplecottonSlime.SetSlimeBaseColorsSpecific(topColor_purplecotton, middleColor_purplecotton, bottomColor_purplecotton, middleColor_purplecotton, 0, 0, false, 4);
        
        //Some food management
        purplecottonSlime.AddFoodGroup(PrismLibLookup.fruitFoodGroup); //Adds what the slime can eat
        purplecottonSlime.AddAdditionalFood(PrismNativeBaseSlime.Cotton.GetPrismBaseSlime());
        purplecottonSlime.AddFavoriteFood(PrismNativeBaseSlime.Cotton.GetPrismBaseSlime());
        purplecottonSlime.RefreshEatMap(); //Refreshes everything

        //Make it spawn
        var purplecottonSpawnLocations = new PrismSpawnLocations[] { PrismSpawnLocations.RainbowFields };
        PrismLibSpawning.MakeSpawnable(purplecottonSlime, PrismSpawnerActiveTime.AllTheTime, purplecottonSpawnLocations,0.3f, PrismSpawnerType.Slime);
        
        //Create Pedia entry
        var purplecottonPediaCreator = new PrismIdentifiablePediaEntryCreatorV01(
            purplecottonSlime, PrismPediaCategoryType.Slimes,
            AddTranslationFromSR2E("purplecotton.pedia.intro"));
        purplecottonPediaCreator.factSet = PrismPediaFactSetType.Slime; //What set of facts should be displayed
        purplecottonPediaCreator.details = //Create details, you can add absolutely anything you want
            PrismPediaDetail.From(
                PrismPediaDetail.Create(PrismPediaDetailType.Slimeology,AddTranslationFromSR2E("purplecotton.pedia.slimeology")),
                PrismPediaDetail.Create(PrismPediaDetailType.RancherRisks,AddTranslationFromSR2E("purplecotton.pedia.rancherrisks")),
                PrismPediaDetail.Create(PrismPediaDetailType.Plortonomics,AddTranslationFromSR2E("purplecotton.pedia.plortonomics"))
            );
        //Custom additional fact
        purplecottonPediaCreator.additionalFacts = PrismPediaAdditionalFact.From(
            PrismPediaAdditionalFact.Create(
                AddTranslationFromSR2E("purplecotton.pedia.fact.purple.title"), 
            AddTranslationFromSR2E("purplecotton.pedia.fact.purple.description"), 
            EmbeddedResourceEUtil.LoadSprite("Assets.icon.png"))
            );
        purplecottonPedia = purplecottonPediaCreator.CreateIdentifiablePediaEntry();
        
        
        //Create largo manually
        //var purplecottonPinkLargoCreator = new PrismLargoCreator(purplecottonSlime, PrismNativeBaseSlime.Pink);
        //purplecottonPinkLargoCreator.largoMergeSettings = new PrismLargoMergeSettings();
        //purplecottonPinkLargo = purplecottonPinkLargoCreator.CreateLargo();

    }



}
