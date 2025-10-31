﻿using Cotton;
using SR2E.Cotton;
using SR2E.Expansion;
using SR2E.Prism;
using SR2E.Prism.Creators;
using SR2E.Prism.Lib;
using SR2E.Prism.Data;
using SR2E.Prism.Enums;
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
    public const bool RequireLibrary = true; // Enable if you use Cotton
}

public class SlimeMain : SR2EExpansionV2
{
    public static Color32 vacColor_byte = new Color32(199, 43, 255, 255);
    public static Color32 topColor_byte = new Color32(168, 52, 235, 255);
    public static Color32 middleColor_byte = new Color32(145, 23, 207, 255);
    public static Color32 bottomColor_byte = new Color32(18, 1, 48, 255);
    public static PrismSlime byteSlime;
    public static PrismPlort bytePlort;

    public override void OnPrismCreateAdditions()
    {
        //Create BytePlort
        var bytePlortCreator = new PrismPlortCreator(
            "Byte",
            EmbeddedResourceEUtil.LoadSprite("Assets.iconPlortByte.png"),
            AddTranslation("Byte Plort", "l.bytePlort"));
        bytePlortCreator.moddedMarketData = new PrismMarketData(27f, 85f); //Controls the market values
        bytePlortCreator.customBasePrefab = Get<GameObject>("plortTabby");
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
        byteSlimeCreator.vaccable = true; // Controls whether a slime is vaccable
        byteSlimeCreator.plort = bytePlort; // The plort of the slime, can either be an IdentifiableType or PrismPlort
        byteSlimeCreator.vacColor = vacColor_byte; // The color of the slime in the vac and its splat color
        byteSlimeCreator.canLargofy = false; // Can this slime have largo forms
        byteSlimeCreator.createAllLargos = false; // Automatically create all largos, requires canLargofy
        byteSlimeCreator.customBaseAppearance = Get<SlimeAppearance>("CottonDefault"); // If not set, Pink is default, it will duplicate it
        byteSlimeCreator.customBasePrefab = Get<GameObject>("slimeCotton"); // If not set, Pink is default, it will duplicate it
        //if(!byteSlimeCreator.IsValid()) DoStuff(); // Optionally check if valid
        byteSlime = byteSlimeCreator.CreateSlime();
        
        //Some color adjustments on the slime
        byteSlime.SetSlimeBaseColorsSpecific(topColor_byte, middleColor_byte, bottomColor_byte, middleColor_byte, 0, 0, false, 0);
        byteSlime.SetSlimeBaseColorsSpecific(topColor_byte, middleColor_byte, bottomColor_byte, middleColor_byte, 0, 0, false, 2);
        byteSlime.SetSlimeBaseColorsSpecific(topColor_byte, middleColor_byte, bottomColor_byte, middleColor_byte, 0, 0, false, 3);
        byteSlime.SetSlimeBaseColorsSpecific(topColor_byte, middleColor_byte, bottomColor_byte, middleColor_byte, 0, 0, false, 4);
        
        //Some food management
        byteSlime.AddFoodGroup(CottonLibrary.fruits); //Adds what the slime can eat
        byteSlime.RefreshEatMap(); //Refreshes everything
        
    }



}
