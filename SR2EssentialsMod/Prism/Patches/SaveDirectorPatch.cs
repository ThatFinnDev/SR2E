using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Pedia;
using SR2E.Expansion;
using SR2E.Prism.Lib;
using SR2E.Storage;

namespace SR2E.Prism.Patches;


//[PrismPatch()]
//[HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.Awake))]
internal static class SaveDirectorPatch
{
    internal static void Postfix(AutoSaveDirector __instance)
    {
        PrismShortcuts.emptyTranslation = AddTranslation("");
        PrismShortcuts.unavailableIcon = Get<Sprite>("unavailableIcon");
        PrismLibPedia.PediaDetailTypesInitialize();
        
        foreach (var category in GetAll<PediaCategory>())
            try
            {
                category.GetRuntimeCategory();
            }
            catch  {  }
        
        
        foreach (var expansion in SR2EEntryPoint.expansionsV3)
            try { expansion.OnPrismCreateAdditions(); }
            catch (Exception e) { MelonLogger.Error(e); }
        foreach (var expansion in SR2EEntryPoint.expansionsV2)
            try { expansion.OnPrismCreateAdditions(); }
            catch (Exception e) { MelonLogger.Error(e); }
        
        
        foreach (var expansion in SR2EEntryPoint.expansionsV3)
            try { expansion.AfterPrismCreateAdditions(); }
            catch (Exception e) { MelonLogger.Error(e); }
        foreach (var expansion in SR2EEntryPoint.expansionsV2)
            try { expansion.AfterPrismCreateAdditions(); }
            catch (Exception e) { MelonLogger.Error(e); }
        
        
        // Doing this so it executes after all mods have made their slimes.
        foreach (var largoAction in PrismShortcuts.createLargoActions)
            try { largoAction.Invoke(); }
            catch (Exception e) { MelonLogger.Error(e); }
        
        
        foreach (var expansion in SR2EEntryPoint.expansionsV3)
            try { expansion.AfterPrismLargosCreated(); }
            catch (Exception e) { MelonLogger.Error(e); }
        foreach (var expansion in SR2EEntryPoint.expansionsV2)
            try { expansion.AfterPrismLargosCreated(); }
            catch (Exception e) { MelonLogger.Error(e); }
        
        
    }
}
