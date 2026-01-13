using Il2CppMonomiPark.SlimeRancher;
using Il2CppSystem.Reflection;
using SR2E.Enums;
using SR2E.Managers;

namespace SR2E.Patches.Saving;

[HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.Awake))]
internal static class SaveDirectorPatch
{
    internal static void Prefix(AutoSaveDirector __instance)
    {
        foreach (var expansion in SR2EEntryPoint.expansionsV3)
            try { expansion.BeforeSaveDirectorLoaded(__instance); }
            catch (Exception e) { MelonLogger.Error(e); }
        foreach (var expansion in SR2EEntryPoint.expansionsV2)
            try { expansion.BeforeSaveDirectorLoaded(__instance); } 
            catch (Exception e) { MelonLogger.Error(e); }
        SR2ECallEventManager.ExecuteWithArgs(CallEvent.BeforeSaveDirectorLoad,("saveDirector",__instance));
        
        //OBSOLETE
        /**/foreach (var expansion in SR2EEntryPoint.expansionsV1V2) 
        /**/    try
        /**/    { 
        /**/        expansion.OnSaveDirectorLoading(__instance);
        /**/    } catch (Exception e) { MelonLogger.Error(e); }
        //OBSOLETE
        
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
        
        
        foreach (var expansion in SR2EEntryPoint.expansionsV3)
            try { expansion.AfterSaveDirectorLoaded(__instance);
            } catch (Exception e) { MelonLogger.Error(e); }
        foreach (var expansion in SR2EEntryPoint.expansionsV2)
            try { expansion.AfterSaveDirectorLoaded(__instance);
            } catch (Exception e) { MelonLogger.Error(e); }
        SR2ECallEventManager.ExecuteWithArgs(CallEvent.AfterSaveDirectorLoad,("saveDirector",__instance));
        
        //OBSOLETE
        /**/foreach (var expansion in SR2EEntryPoint.expansionsV1V2)
        /**/    try
        /**/    {
        /**/        expansion.SaveDirectorLoaded(__instance);
        /**/    } catch (Exception e) { MelonLogger.Error(e); }
        //OBSOLETE
        
    }
}
