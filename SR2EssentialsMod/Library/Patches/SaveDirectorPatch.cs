using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CottonLibrary;
using static CottonLibrary.Library;

using HarmonyLib;
using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Pedia;
using Il2CppMonomiPark.SlimeRancher.UI.Localization;
using Il2CppTMPro;
using SR2E.Storage;
using UnityEngine;

namespace CottonLibrary.Patches;


[LibraryPatch()]
[HarmonyPatch(typeof(AutoSaveDirector), "Awake")]
public static class SaveDirectorPatch
{
    public static void Prefix(AutoSaveDirector __instance)
    {

        slimes = Get<IdentifiableTypeGroup>("SlimesGroup");
        baseSlimes = Get<IdentifiableTypeGroup>("BaseSlimeGroup");
        largos = Get<IdentifiableTypeGroup>("LargoGroup");
        food = Get<IdentifiableTypeGroup>("FoodGroup");
        meat = Get<IdentifiableTypeGroup>("MeatGroup");
        veggies = Get<IdentifiableTypeGroup>("VeggieGroup");
        fruits = Get<IdentifiableTypeGroup>("FruitGroup");
        nectar = Get<IdentifiableTypeGroup>("NectarFoodGroup");
        plorts = Get<IdentifiableTypeGroup>("PlortGroup");
        crafts = Get<IdentifiableTypeGroup>("CraftGroup");
        chicks = Get<IdentifiableTypeGroup>("ChickGroup");

        foreach (SR2EExpansionV2 lib in SR2EEntryPoint.expansionsV2)
        {
            lib.SaveDirectorLoading(__instance);
        }
    }
    public static void Postfix()
    {
        PediaDetailInitialize();
        
        // 0.6: ffs why
        var steamToy = Get<ToyDefinition>("SteamFox");
        if (steamToy)
            INTERNAL_SetupLoadForIdent(steamToy.ReferenceId, steamToy);
        // add more platforms please

        foreach (SR2EExpansionV2 lib in SR2EEntryPoint.expansionsV2)
        {
            lib.SaveDirectorLoaded();
        }
        
        foreach (SR2EExpansionV2 lib in SR2EEntryPoint.expansionsV2)
        {
            lib.LateSaveDirectorLoaded();
        }
        
        // Doing this so it executes after all mods have made their slimes.
        foreach (var largoAction in createLargoActions)
        {
            largoAction();
        }
        
        foreach (SR2EExpansionV2 lib in SR2EEntryPoint.expansionsV2)
        {
            lib.AutoLargosLoaded();
        }
        
        foreach (var category in Resources.FindObjectsOfTypeAll<PediaCategory>())
        {
            category.GetRuntimeCategory();
        }
    }
}
