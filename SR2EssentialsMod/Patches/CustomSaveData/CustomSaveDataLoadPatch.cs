using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Persist;
using SR2E.Saving;
using SR2E.Storage;

namespace SR2E.Patches.CustomSaveData;
   
[HarmonyPriority(99999999)]
[HarmonyPatch(typeof(GameModelPushHelpers), nameof(GameModelPushHelpers.PushGame))]
internal static class CustomSaveDataLoadPatch
{
    internal static string prefix = "SR2EDataV01";
    internal static void Prefix(ActorIdProvider actorIdProvider, ISaveReferenceTranslation saveReferenceTranslation, GameV09 gameState, GameModel gameModel)
    {
        foreach (var entry in gameState.ZoneIndex.IndexTable)
            if (entry.StartsWith(prefix))
            {
                string remaining = entry.Substring(prefix.Length);
                
                if (remaining.Length >= 32)
                {
                    string md5Hash = remaining.Substring(0, 32);

                    foreach (var expansion in SR2EEntryPoint.expansionsV3)
                        try
                        {
                            if(expansion.MelonBase.Info.Name.CreateMD5() == md5Hash)
                                try
                                {
                                    var rawBytes = remaining.Substring(32).DecodeFromBase128();
                                    var rootSave = RootSave.FromBytes(rawBytes);
                                    expansion.OnEarlyCustomSaveDataReceived(rootSave, new LoadingGameSessionData(actorIdProvider, saveReferenceTranslation, saveReferenceTranslation.toNonIVariant(), gameState, gameModel));
                                } 
                                catch (Exception e)
                                {
                                    MelonLogger.Error($"Failed to save custom save data for expansion {expansion.MelonBase.Info.Name}: {e}");
                                }
                        } catch { }
                }
                else MelonLogger.Error("An error occured while loading some custom save data!");
            }
    }
}