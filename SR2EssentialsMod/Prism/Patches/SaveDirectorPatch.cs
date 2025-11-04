using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Pedia;
using SR2E.Cotton;
using SR2E.Expansion;
using SR2E.Prism.Lib;
using SR2E.Storage;

namespace SR2E.Prism.Patches;


[PrismPatch()]
[HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.Awake))]
internal static class SaveDirectorPatch
{
    static void Prefix(AutoSaveDirector __instance)
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

        foreach (SR2EExpansionV2 expansion in SR2EEntryPoint.expansionsV2)
            try
            {
                expansion.BeforeSaveDirectorLoaded(__instance);
            }
            catch (Exception e) { MelonLogger.Error(e); }
    }
    internal static void Postfix()
    {
        PrismShortcuts.emptyTranslation = AddTranslation("");
        PrismShortcuts.unavailableIcon = Get<Sprite>("unavailableIcon");
        PrismaLibPedia.PediaDetailTypesInitialize();
        
        // 0.6: ffs why
        //var steamToy = Get<ToyDefinition>("SteamFox");
        //if (steamToy)
        //    INTERNAL_SetupLoadForIdent(steamToy.ReferenceId, steamToy);
        // add more platforms please
        foreach (SR2EExpansionV2 expansion in SR2EEntryPoint.expansionsV2)
            try
            {
                expansion.OnPrismCreateAdditions();
            }
            catch (Exception e) { MelonLogger.Error(e); }
        
        
        foreach (SR2EExpansionV2 expansion in SR2EEntryPoint.expansionsV2)
            try
            {
                expansion.AfterPrismCreateAdditions();
            }
            catch (Exception e) { MelonLogger.Error(e); }
        
        
        // Doing this so it executes after all mods have made their slimes.
        foreach (var largoAction in CottonLibrary.createLargoActions)
            try
            {
                largoAction.Invoke();
            }
            catch (Exception e) { MelonLogger.Error(e); }
        
        
        foreach (SR2EExpansionV2 expansion in SR2EEntryPoint.expansionsV2)
            try
            {
                expansion.AfterPrismLargosCreated();
            }
            catch (Exception e) { MelonLogger.Error(e); }
        
        foreach (var category in Resources.FindObjectsOfTypeAll<PediaCategory>())
            try
            {
                category.GetRuntimeCategory();
            }
            catch  {  }
        
    }
}
