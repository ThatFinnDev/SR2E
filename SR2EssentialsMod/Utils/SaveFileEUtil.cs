using System.Linq;
using Il2CppMonomiPark.SlimeRancher.Persist;
using Il2CppSystem.Linq;
using SR2E.Enums;
using SR2E.Storage;
using static SR2E.Enums.SR2EError;
namespace SR2E.Utils;

public static class SaveFileEUtil
{
    
    public static SR2ESaveFileV01 ExportSaveV01(Summary summary)
    {
        if (ExportSaveV01(summary.Name, summary.SaveName, out SR2ESaveFileV01 data) == NoError)
            return data;
        return null;
    }
    public static SR2ESaveFileV01 ExportSaveV01(string gameName, string latestSaveName)
    {
        if (ExportSaveV01(gameName, latestSaveName, out SR2ESaveFileV01 data) == NoError)
            return data;
        return null;
    }
    public static SR2EError ExportSaveV01(string gameName, string latestSaveName, out SR2ESaveFileV01 data)
    {
        data = null;
        if (!ExperimentalSaveExport.HasFlag()) return NeedExperiment;
        
        if (gameName.Split("_").Length !=2) return InvalidGameName;
        if (!gameName.Split("_")[0].All(char.IsDigit)) return InvalidGameName;
        
        var summaries = autoSaveDirector.GetSavesByGameName(gameName);
        Dictionary<int,byte[]> savesData = new Dictionary<int,byte[]>();
        if(summaries.Count==0) return NoValidSummaries;
        var storageProvider = autoSaveDirector._storageProvider;
        
        var sr2ESaveFile = new SR2ESaveFileV01(savesData,gameName.Split("_")[0], 0);
        var hasLatest = false;
        foreach (var summary in summaries)
        {
            if(summary.IsInvalid) continue;
            var gameWithSaveIDName = summary.SaveName;
            var split = gameWithSaveIDName.Split("_");
            if (split.Length != 3) continue;
            var saveID = -1;
            try
            {
                saveID = int.Parse(split[2]);
            } catch { }
            if (saveID < 0) continue;
            //If save file names are messed up, prefer current one if the case:
            bool removeBefore = false;
            if (savesData.ContainsKey(saveID))
            {
                if(summary.SaveName!=latestSaveName) continue;
                removeBefore = true;
            }
            //In the storageProvider the game name also includes the auto save id
            //Normally gameName is without and SaveName with
            byte[] gameBytes = null;
            try
            {
                var stream = new Il2CppSystem.IO.MemoryStream();
                storageProvider.GetGameData(gameWithSaveIDName,stream);
                gameBytes = stream.ToArray();
                if (stream != null && stream.CanRead) stream.Close();
                                        
            }catch { }
            if (gameBytes == null || gameBytes.Length == 0) continue;
            if(removeBefore)
                savesData.Remove(saveID);
            savesData.Add(saveID, gameBytes);
            if (summary.SaveName == latestSaveName)
            {
                hasLatest = true;
                sr2ESaveFile.latest = saveID;
                sr2ESaveFile.metaFeralEnabled = summary.FeralEnabled;
                sr2ESaveFile.metaTarrEnabled = summary.TarrEnabled;
                sr2ESaveFile.metaDisplayName = summary.DisplayName; 
                sr2ESaveFile.metaGameName = gameName;
                sr2ESaveFile.metaSaveSlotIndex = summary.SaveSlotIndex;
                sr2ESaveFile.metaLatestSaveNumber = summary.SaveNumber;
            }     
        }
        if (savesData.Count == 0) return NoValidSaves;
        if (!hasLatest) return LatestSaveInvalid;
        return NoError;
    }
    public static SR2EError ImportSaveV01(SR2ESaveFileV01 sr2ESaveFile, int slotThatStartWithOne, bool loadMenuMenuOnSuccess)
    {
        if (!ExperimentalSaveExport.HasFlag()) return NeedExperiment;
        if(sr2ESaveFile==null) return SaveInvalidGeneral;
        if (!sr2ESaveFile.IsValid()) return SaveInvalidGeneral;
        if(gameContext==null||autoSaveDirector==null||autoSaveDirector._storageProvider==null) return GameNotLoadedYet;
        var storageProvider = autoSaveDirector._storageProvider;
        List<Summary> summariesToDelete = new List<Summary>();
        foreach (var summary in autoSaveDirector.EnumerateAllSaveGamesIncludingBackups().ToList())
            if(summary.SaveSlotIndex==slotThatStartWithOne-1)
                summariesToDelete.Add(summary);
        foreach (var summary in summariesToDelete)
        {
            autoSaveDirector.DeleteGame(summary.Name);
            autoSaveDirector._storageProvider.DeleteGameData(summary.SaveName);
        }
        bool failedSome = false;
        foreach (var pair in sr2ESaveFile.savesData)
        {
            try
            {
                var stream = new Il2CppSystem.IO.MemoryStream(pair.Value);
                var gameState = new GameV08();

                gameState.Load(stream);
                if (stream != null && stream.CanRead) stream.Close();

                stream = new Il2CppSystem.IO.MemoryStream();

                var newDisplayName = slotThatStartWithOne.ToString();
                var newGameName = sr2ESaveFile.stamp + "_" + newDisplayName;
                gameState.DisplayName = newDisplayName;
                gameState.GameName = newGameName;
                gameState.SaveSlotIndex = slotThatStartWithOne-1;
                gameState.Write(stream);
                var gameBytes = stream.ToArray();
                if (stream != null && stream.CanRead) stream.Close();

                stream = new Il2CppSystem.IO.MemoryStream(gameBytes);
                storageProvider.StoreGameData(newDisplayName,
                    newGameName + "_" + pair.Key, stream);
                if (stream != null && stream.CanRead) stream.Close();
            }
            catch (Exception e)
            {
                failedSome = true;
                bool isMain = pair.Key == sr2ESaveFile.latest;
                if(isMain||DebugLogging.HasFlag())
                {
                    MelonLogger.Error(e);
                    MelonLogger.Error("Error loading saveId: " + pair.Key);
                }
                if (isMain)
                {
                    foreach (var pair2 in sr2ESaveFile.savesData)
                    {
                        try
                        {
                            var newDisplayName = slotThatStartWithOne.ToString();
                            var newGameName = sr2ESaveFile.stamp + "_" + newDisplayName;
                            storageProvider.DeleteGameData(newGameName + "_" + pair2.Key);
                        }
                        catch{ }
                    }
                    return MainSaveIDFailed;
                }
            }
        }
        if(loadMenuMenuOnSuccess) ExecuteInTicks((System.Action)(() => {
            SystemContext.Instance.SceneLoader.LoadMainMenuSceneGroup();
        }), 1);
        if (failedSome) return SomeSaveIDFailed;
        return NoError;
    }
}