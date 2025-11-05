using SR2E.Expansion;
using SR2E.Prism;
using SR2E.Prism.Creators;
using SR2E.Prism.Lib;
using SR2E.Prism.Data;
using Object = UnityEngine.Object;

namespace VirtualSlime;

public static class BuildInfo
{
    public const string Name = "Virtual Slimes"; // Name of the Expansion. 
    public const string Description = "Adds cool slimes"; // Description for the Expansion.
    public const string Author = "Virtual Slime Contributors"; // Author of the Expansion.
    public const string CoAuthors = null; // CoAuthor(s) of the Expansion.  (optional, set as null if none)
    public const string Contributors = "ThatFinn, PinkTarr"; // Contributor(s) of the Expansion.  (optional, set as null if none)
    public const string Company = null; // Company that made the Expansion.  (optional, set as null if none)
    public const string Version = "1.0.1"; // Version of the Expansion.
    public const string DownloadLink = null; // Download Link for the Expansion.  (optional, set as null if none)
    public const string SourceCode = null; // Source Link for the Expansion.  (optional, set as null if none)
    public const string Nexus = null; // Nexus Link for the Expansion.  (optional, set as null if none)
    public const bool UsePrism = true; // Enable if you use Prism
}

public class SlimeMain : SR2EExpansionV2
{
    public static Color32 vacColor_byte = new Color32(199, 43, 255, 255);
    public static Color32 topColor_byte = new Color32(168, 52, 235, 255);
    public static Color32 middleColor_byte = new Color32(145, 23, 207, 255);
    public static Color32 bottomColor_byte = new Color32(18, 1, 48, 255);
    public static PrismBaseSlime byteSlime;
    public static PrismPlort bytePlort;
    public static PrismLargo bytePinkLargo;
    public static PrismIdentifiablePediaEntry bytePedia;

    public override void OnPrismCreateAdditions()
    {
        //Create BytePlort
        var bytePlortCreator = new PrismPlortCreator(
            "Byte",
            EmbeddedResourceEUtil.LoadSprite("Assets.iconPlortByte.png"),
            AddTranslation("Byte Plort", "l.bytePlort"));
        bytePlortCreator.moddedMarketData = new PrismMarketData(27f, 85f); //Controls the market values
        bytePlortCreator.customBasePrefab = PrismNativePlort.Tabby.GetPrismPlort().GetPrefab();
        bytePlortCreator.vacColor = vacColor_byte; // The color of the plort in the vac
        bytePlort = bytePlortCreator.CreatePlort();
        
        //Some color adjustments one the plort
        bytePlort.SetPlortBaseColors(topColor_byte, middleColor_byte, bottomColor_byte);
        
        //Images
        bytePlort.GetPrefab().GetComponent<MeshRenderer>().material.SetTexture("_StripeTexture", EmbeddedResourceEUtil.LoadTexture2D("Assets.bytePlort_falloff_map.png"));
        
        //Create Byte Slime
        var byteSlimeCreator = new PrismBaseSlimeCreator(
            "Byte",
            EmbeddedResourceEUtil.LoadSprite("Assets.iconSlimeByte.png"),
            AddTranslation("Byte Slime", "l.byteSlime"));
        byteSlimeCreator.disableVaccable = false; // Controls whether a slime is vaccable
        byteSlimeCreator.plort = bytePlort; // The plort of the slime, can either be an IdentifiableType or PrismPlort
        byteSlimeCreator.vacColor = vacColor_byte; // The color of the slime in the vac and its splat color
        byteSlimeCreator.canLargofy = true; // Can this slime have largo forms, used for createAllLargos, gets overwritten if you create one manually
        byteSlimeCreator.createAllLargos = true; // Automatically create all largos, requires canLargofy on both slimes
        byteSlimeCreator.disableAutoModdedLargos = false; // Disables autolargos for combini between 2 non-native slimes. Do this if your slime has custom components that break in auto largo
        //byteSlimeCreator.customAutoLargoMergeSettings = new PrismLargoMergeSettings(); //Custom auto largo settings
        byteSlimeCreator.customBaseAppearance = PrismNativeBaseSlime.Cotton.GetPrismBaseSlime().GetSlimeAppearance(); // If not set, Pink is default, it will duplicate it
        byteSlimeCreator.customBasePrefab = PrismNativeBaseSlime.Cotton.GetPrismBaseSlime().GetPrefab();; // If not set, Pink is default, it will duplicate it
        //if(!byteSlimeCreator.IsValid()) DoStuff(); // Optionally check if valid
        byteSlime = byteSlimeCreator.CreateSlime();
        
        //Some color adjustments on the slime
        byteSlime.SetSlimeBaseColorsSpecific(topColor_byte, middleColor_byte, bottomColor_byte, middleColor_byte, 0, 0, false, 0);
        byteSlime.SetSlimeBaseColorsSpecific(topColor_byte, middleColor_byte, bottomColor_byte, middleColor_byte, 0, 0, false, 2);
        byteSlime.SetSlimeBaseColorsSpecific(topColor_byte, middleColor_byte, bottomColor_byte, middleColor_byte, 0, 0, false, 3);
        byteSlime.SetSlimeBaseColorsSpecific(topColor_byte, middleColor_byte, bottomColor_byte, middleColor_byte, 0, 0, false, 4);
        
        //Some food management
        byteSlime.AddFoodGroup(PrismLibLookup.fruitFoodGroup); //Adds what the slime can eat
        byteSlime.RefreshEatMap(); //Refreshes everything

        //Make it spawn
        var byteSpawnLocations = new PrismSpawnLocations[] { PrismSpawnLocations.RainbowFields };
        PrismLibSpawning.MakeSpawnable(byteSlime, PrismSpawnerActiveTime.AllTheTime, byteSpawnLocations,0.3f, PrismSpawnerType.Slime);
        
        //Create Pedia entry
        var bytePediaCreator = new PrismIdentifiablePediaEntryCreator(
            byteSlime, PrismPediaCategoryType.Slimes,
            AddTranslation("Some intro", "l.byte_pedia_intro"));
        bytePediaCreator.factSet = PrismPediaFactSetType.Slime; //What set of facts should be displayed
        bytePediaCreator.details = //Create details, you can add absolutely anything you want
            PrismPediaDetail.From(
                PrismPediaDetail.Create(PrismPediaDetailType.Slimeology,AddTranslation("Some Slimeology", "l.byte_pedia_slimeology")),
                PrismPediaDetail.Create(PrismPediaDetailType.RancherRisks,AddTranslation("Some Rancherrisks", "l.byte_pedia_rancherrisks")),
                PrismPediaDetail.Create(PrismPediaDetailType.Plortonomics,AddTranslation("Some Plortonomics", "l.byte_pedia_plortonomics"))
            );
        //Custom additional fact
        bytePediaCreator.additionalFacts = PrismPediaAdditionalFact.From(
            PrismPediaAdditionalFact.Create(
            AddTranslation("Virtual"), 
            AddTranslation("Yes"), 
            EmbeddedResourceEUtil.LoadSprite("Assets.icon.png"))
            );
        bytePedia = bytePediaCreator.CreateIdentifiablePediaEntry();
        
        
        //Create largo manually
        //var bytePinkLargoCreator = new PrismLargoCreator(byteSlime, PrismNativeBaseSlime.Pink);
        //bytePinkLargoCreator.largoMergeSettings = new PrismLargoMergeSettings();
        //bytePinkLargo = bytePinkLargoCreator.CreateLargo();

    }



}
