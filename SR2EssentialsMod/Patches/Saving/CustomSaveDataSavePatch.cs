using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Persist;
using SR2E.Saving;
using SR2E.Storage;

namespace SR2E.Patches.Saving;

[HarmonyPriority(99999999)]
[HarmonyPatch(typeof(GameModelPullHelpers), nameof(GameModelPullHelpers.PullGame))]
internal static class CustomSaveDataSavePatch
{
    internal static string prefix = "SR2EDataV01";
    internal static string prefixown = "SR2EOwnDataV01";
    internal static void Postfix(GameModel gameModel,SavedGameInfoProvider savedGameInfoProvider, ISaveReferenceTranslation saveReferenceTranslation, GameMetadata metadata, ref GameV09 __result )
    {
        try
        {
            var rootSave = SR2EOptionsButtonManager.OnInGameSave(new SavingGameSessionData(saveReferenceTranslation,
                saveReferenceTranslation.toNonIVariant(), __result, gameModel,metadata,savedGameInfoProvider));
            if (rootSave != null)
            {
                var base128 = rootSave.ToBytes().EncodeToBase128();
                var finalEntry = $"{prefixown}{base128}";
                __result.ZoneIndex.IndexTable = __result.ZoneIndex.IndexTable.AddToNew(finalEntry);
            }
        }
        catch (Exception e) { MelonLogger.Error(e); }
        
        
        foreach (var expansion in SR2EEntryPoint.expansionsV3)
        {
            try
            {
                var rootSave = expansion.OnSaveCustomSaveData(new SavingGameSessionData(saveReferenceTranslation,
                    saveReferenceTranslation.toNonIVariant(), __result, gameModel,metadata,savedGameInfoProvider)); 
                if (rootSave == null) continue;
                var base128 = rootSave.ToBytes().EncodeToBase128();
                var md5Hash = expansion.MelonBase.Info.Name.CreateMD5();
                var finalEntry = $"{prefix}{md5Hash}{base128}";
                __result.ZoneIndex.IndexTable = __result.ZoneIndex.IndexTable.AddToNew(finalEntry);
            }
            catch (Exception e)
            {
                MelonLogger.Error($"Failed to save custom save data for expansion {expansion.MelonBase.Info.Name}: {e}");
            }
        }
    }
}