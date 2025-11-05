using Il2CppMonomiPark.SlimeRancher;
using Il2CppSystem.Reflection;

namespace SR2E.Patches.General;

[HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.Awake))]
internal static class SaveDirectorPatch
{
    internal static void Prefix(AutoSaveDirector __instance)
    {
        foreach (var expansion in SR2EEntryPoint.expansionsV2)
            try
            {
                expansion.BeforeSaveDirectorLoaded(__instance);
            } catch (Exception e) { MelonLogger.Error(e); }
        
        //OBSOLETE
        /**/foreach (var expansion in SR2EEntryPoint.expansionsAll) 
        /**/    try
        /**/    { 
        /**/        expansion.OnSaveDirectorLoading(__instance);
        /**/    } catch (Exception e) { MelonLogger.Error(e); }
        //OBSOLETE
        
        if(SR2EEntryPoint.usePrism)
            try
            {
                Prism.Patches.SaveDirectorPatch.Prefix(__instance);
            }
            catch (Exception e) { MelonLogger.Error(e); }
    }
    internal static void Postfix(AutoSaveDirector __instance)
    {
        LookupEUtil._identifiableTypeGroupList = Get<IdentifiableTypeGroupList>("All Type Groups List");
        if(SR2EEntryPoint.usePrism)
            try
            {
                Prism.Patches.SaveDirectorPatch.Postfix(__instance);
            }
            catch (Exception e) { MelonLogger.Error(e); }
        
        
        foreach (var expansion in SR2EEntryPoint.expansionsV2)
            try
            {
                expansion.AfterSaveDirectorLoaded(__instance);
            } catch (Exception e) { MelonLogger.Error(e); }
        
        //OBSOLETE
        /**/foreach (var expansion in SR2EEntryPoint.expansionsAll)
        /**/    try
        /**/    {
        /**/        expansion.SaveDirectorLoaded(__instance);
        /**/    } catch (Exception e) { MelonLogger.Error(e); }
        //OBSOLETE
        
    }
}
