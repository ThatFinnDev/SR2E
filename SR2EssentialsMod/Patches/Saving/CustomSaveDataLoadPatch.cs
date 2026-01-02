using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Persist;
using SR2E.Expansion;
using SR2E.Saving;
using SR2E.Storage;

namespace SR2E.Patches.Saving;
   
[HarmonyPriority(99999999)]
[HarmonyPatch(typeof(GameModelPushHelpers), nameof(GameModelPushHelpers.PushGame))]
internal static class CustomSaveDataLoadPatch
{
    static Dictionary<SR2EExpansionV3, (RootSave, LoadingGameSessionData)> rootSaves = new();
    static Dictionary<SR2EExpansionV3, LoadingGameSessionData> noRootSaves = new();
    internal static string prefix = "SR2EDataV01";
    internal static string prefixown = "SR2EOwnDataV01";
    internal static void ExecSaveDataReceived()
    {
        foreach (var expansion in SR2EEntryPoint.expansionsV3)
            if (rootSaves.ContainsKey(expansion))
                try { expansion.OnCustomSaveDataReceived(rootSaves[expansion].Item1, rootSaves[expansion].Item2); } 
                catch (Exception e) { MelonLogger.Error(e); }
            else if(noRootSaves.ContainsKey(expansion))
                try { expansion.OnNoCustomSaveDataReceived(noRootSaves[expansion]); }
                catch (Exception e) { MelonLogger.Error(e); }
        rootSaves = new Dictionary<SR2EExpansionV3, (RootSave, LoadingGameSessionData)>();
        noRootSaves = new Dictionary<SR2EExpansionV3, LoadingGameSessionData>();
    }
    internal static void Prefix(ActorIdProvider actorIdProvider, ISaveReferenceTranslation saveReferenceTranslation, GameV09 gameState, GameModel gameModel)
    {
        bool hasExecutedOwn = false;
        foreach (var entry in gameState.ZoneIndex.IndexTable)
            if (entry.StartsWith(prefixown))
            {
                try
                {
                    string remaining = entry.Substring(prefixown.Length);
                    var rawBytes = remaining.DecodeFromBase128();
                    var rootSave = RootSave.FromBytes<SR2EOptionsButtonManager.CustomOptionsInGameSave>(rawBytes);
                    var sessionData = new LoadingGameSessionData(actorIdProvider, saveReferenceTranslation, saveReferenceTranslation.toNonIVariant(), gameState, gameModel);
                    hasExecutedOwn = true;
                    SR2EOptionsButtonManager.OnInGameLoad(rootSave,sessionData);
                }catch (Exception e) { MelonLogger.Error(e); }
            }
        if(!hasExecutedOwn)
            try
            {
                var sessionData = new LoadingGameSessionData(actorIdProvider, saveReferenceTranslation, saveReferenceTranslation.toNonIVariant(), gameState, gameModel);
                SR2EOptionsButtonManager.OnInGameLoad(null,sessionData);
            }catch (Exception e) { MelonLogger.Error(e); }
        
        
        rootSaves = new Dictionary<SR2EExpansionV3, (RootSave, LoadingGameSessionData)>();
        noRootSaves = new Dictionary<SR2EExpansionV3, LoadingGameSessionData>();
        var executedExpansions = new List<SR2EExpansionV3>();
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
                            {
                                var sessionData = new LoadingGameSessionData(actorIdProvider, saveReferenceTranslation, saveReferenceTranslation.toNonIVariant(), gameState, gameModel);
                                RootSave rootSave = null; 
                                try
                                {
                                    var rawBytes = remaining.Substring(32).DecodeFromBase128();
                                    rootSave = RootSave.FromBytes(rawBytes);
                                    if (rootSave == null) throw new Exception("Save Data is null!");;
                                    rootSaves.Add(expansion, (rootSave, sessionData));
                                    executedExpansions.Add(expansion);
                                }
                                catch (Exception e)
                                {
                                    MelonLogger.Error(
                                        $"Failed to save custom save data for expansion {expansion.MelonBase.Info.Name}: {e}");
                                }
                                if(rootSave!=null)
                                    try
                                    {
                                        expansion.OnEarlyCustomSaveDataReceived(rootSave, sessionData);
                                    }
                                    catch (Exception e) { MelonLogger.Error(e); }
                            }
                        } catch { }
                }
                else MelonLogger.Error("An error occured while loading some custom save data!");
            }
        foreach (var expansion in SR2EEntryPoint.expansionsV3)
            if(!executedExpansions.Contains(expansion))
                try
                {
                    var sessionData = new LoadingGameSessionData(actorIdProvider, saveReferenceTranslation, saveReferenceTranslation.toNonIVariant(), gameState, gameModel);
                    noRootSaves.Add(expansion, sessionData);

                    expansion.OnEarlyNoCustomSaveDataReceived(sessionData);
                }
                catch (Exception e) { MelonLogger.Error(e); }
    }
}