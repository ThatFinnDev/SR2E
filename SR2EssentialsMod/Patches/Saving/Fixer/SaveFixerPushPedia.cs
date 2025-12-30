using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Persist;

namespace SR2E.Patches.Saving.Fixer;

[HarmonyPriority(-99999999)]
[HarmonyPatch(typeof(GameModelPushHelpers), nameof(GameModelPushHelpers.PushPedia))]
internal static class SaveFixerPushPedia
{
    internal static void Prefix(GameModel gameModel, PediaV01 pedia, ILoadReferenceTranslation loadReferenceTranslation)
    {
        try {
            //Remove invalid Pedia entries
            if (!SR2EEntryPoint.disableFixSaves)
                foreach (string unlockedID in pedia.UnlockedIds)
                    if(loadReferenceTranslation.IsUnknownPediaEntryId(unlockedID)||loadReferenceTranslation.GetPediaEntry(unlockedID)==null)
                        pedia.UnlockedIds.Remove(unlockedID);
        }
        catch (Exception e) { MelonLogger.Error(e); }
    }

    
}

