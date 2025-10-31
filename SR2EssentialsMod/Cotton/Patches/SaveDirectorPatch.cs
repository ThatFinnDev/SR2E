
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Pedia;
using SR2E.Expansion;
using SR2E.Storage;

namespace SR2E.Cotton.Patches;


[LibraryPatch()]
[HarmonyPatch(typeof(AutoSaveDirector), "Awake")]
public static class SaveDirectorPatch
{
    public static void Prefix(AutoSaveDirector __instance)
    {
        CottonLibrary.slimes = Get<IdentifiableTypeGroup>("SlimesGroup");
        CottonLibrary.baseSlimes = Get<IdentifiableTypeGroup>("BaseSlimeGroup");
        CottonLibrary.largos = Get<IdentifiableTypeGroup>("LargoGroup");
        CottonLibrary.food = Get<IdentifiableTypeGroup>("FoodGroup");
        CottonLibrary.meat = Get<IdentifiableTypeGroup>("MeatGroup");
        CottonLibrary.veggies = Get<IdentifiableTypeGroup>("VeggieGroup");
        CottonLibrary.fruits = Get<IdentifiableTypeGroup>("FruitGroup");
        CottonLibrary.nectar = Get<IdentifiableTypeGroup>("NectarFoodGroup");
        CottonLibrary.plorts = Get<IdentifiableTypeGroup>("PlortGroup");
        CottonLibrary.crafts = Get<IdentifiableTypeGroup>("CraftGroup");
        CottonLibrary.chicks = Get<IdentifiableTypeGroup>("ChickGroup");

        foreach (SR2EExpansionV2 lib in SR2EEntryPoint.expansionsV2)
        {
            lib.SaveDirectorLoading(__instance);
        }
    }
    public static void Postfix()
    {
        CottonLibrary.Pedia.PediaDetailInitialize();
        
        // 0.6: ffs why
        //var steamToy = Get<ToyDefinition>("SteamFox");
        //if (steamToy)
        //    INTERNAL_SetupLoadForIdent(steamToy.ReferenceId, steamToy);
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
        foreach (var largoAction in CottonLibrary.createLargoActions)
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
