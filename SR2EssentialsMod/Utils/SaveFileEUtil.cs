using System.Linq;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Persist;
using Il2CppSystem.Linq;
using SR2E.Enums;
using SR2E.Storage;
using static SR2E.Enums.SR2EError;
namespace SR2E.Utils;

public static class SaveFileEUtil
{
    private static Exception noBoolException = new Exception("The value has to be a bool!");
    private static Exception noIntException = new Exception("The value has to be an inte!");
    private static Exception noFloatException = new Exception("The value has to be a float!");
    private static Exception noDoubleException = new Exception("The value has to be a double!");
    public static SR2ESaveFileV01 ExportSaveV01(Summary summary, bool sendErrorLogs = false) => ExportSaveV01(summary.Name, summary.SaveName,sendErrorLogs);
    public static SR2EError ExportSaveV01(Summary summary, out SR2ESaveFileV01 data) => ExportSaveV01(summary.Name, summary.SaveName,out data);

    public static SR2ESaveFileV01 ExportSaveV01(string gameName, string latestSaveName, bool sendErrorLogs = false)
    {
        var error = ExportSaveV01(gameName, latestSaveName, out SR2ESaveFileV01 data);
        if (error == NoError)
            return data;
        if(sendErrorLogs) MelonLogger.Msg("Error when exporting save: "+error);
        return null;
    }
    
    public static SR2EError ExportSaveV01(string gameName, string latestSaveName, out SR2ESaveFileV01 data)
    {
        data = null;
        if (!AllowSaveExport.HasFlag()) return NeedFlag;
        
        if (gameName.Split("_").Length !=2) return InvalidGameName;
        var stamp = gameName.Split("_")[0];
        if (!stamp.All(char.IsDigit)) return InvalidGameName;
        
        var summaries = autoSaveDirector.GetSavesByGameName(gameName);
        Dictionary<int,byte[]> savesData = new Dictionary<int,byte[]>();
        if(summaries.Count==0) return NoValidSummaries;
        var storageProvider = autoSaveDirector._storageProvider;
        
        var sr2ESaveFile = new SR2ESaveFileV01(savesData,gameName.Split("_")[0], 0);
        sr2ESaveFile.stamp = stamp;
        sr2ESaveFile.SR2ECodeVersion = BuildInfo.CodeVersion;
        sr2ESaveFile.SR2EDisplayVersion = BuildInfo.DisplayVersion;
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
                storageProvider.GetGameData(gameWithSaveIDName, stream);
                gameBytes = stream.ToArray();
                if (stream != null && stream.CanRead) stream.Close();

            }
            catch (Exception e)
            {
                if(DebugLogging.HasFlag())
                {
                    MelonLogger.Error(e);
                    MelonLogger.Error("Error exporting save saveId: " + saveID);
                }
            }
            if (gameBytes == null || gameBytes.Length == 0) continue;
            if(removeBefore)
                savesData.Remove(saveID);
            savesData.Add(saveID, gameBytes);
            if (summary.SaveName == latestSaveName)
            {
                hasLatest = true;
                sr2ESaveFile.latest = saveID;
                sr2ESaveFile.metaGameIcon = summary.IconId.ReferenceId;
                sr2ESaveFile.metaFeralEnabled = summary.FeralEnabled;
                sr2ESaveFile.metaTarrEnabled = summary.TarrEnabled;
                sr2ESaveFile.metaDisplayName = summary.DisplayName; 
                sr2ESaveFile.metaGameName = gameName;
                sr2ESaveFile.metaSaveSlotIndex = summary.SaveSlotIndex;
                sr2ESaveFile.metaLatestSaveNumber = summary.SaveNumber;
                sr2ESaveFile.metaSR2Version = summary.Version;
            }     
        }
        if (savesData.Count == 0) return NoValidSaves;
        if (!hasLatest) return LatestSaveInvalid;

        sr2ESaveFile.savesData = savesData;
        data = sr2ESaveFile;
        return NoError;
    }
    
    public static SR2EError ImportSaveV01(SR2ESaveFileV01 sr2ESaveFile, int slotThatStartWithOne, bool loadMenuMenuOnSuccess)
    {
        if (!AllowSaveExport.HasFlag()) return NeedFlag;
        if(sr2ESaveFile==null) return SaveInvalidGeneral;
        if (!sr2ESaveFile.IsValid()) return SaveInvalidGeneral;
        if(gameContext==null||autoSaveDirector==null||autoSaveDirector._storageProvider==null) return GameNotLoadedYet;
        var storageProvider = autoSaveDirector._storageProvider;
        try
        {
            var summariesToDelete = new List<Summary>();
            foreach (var summary in autoSaveDirector.EnumerateAllSaveGamesIncludingBackups().ToList())
                if(summary.SaveSlotIndex==slotThatStartWithOne-1)
                    summariesToDelete.Add(summary);
            foreach (var summary in summariesToDelete)
            {
                try
                {
                    autoSaveDirector.DeleteGame(summary.Name);
                    autoSaveDirector._storageProvider.DeleteGameData(summary.SaveName);
                } catch { }
            }
        } catch { }
        bool failedSome = false;
        foreach (var pair in sr2ESaveFile.savesData)
        {
            bool isMain = pair.Key == sr2ESaveFile.latest;
            try
            {
                var stream = new Il2CppSystem.IO.MemoryStream(pair.Value);
                var gameState = new GameV09();
                gameState.Load(stream);
                if (stream != null && stream.CanRead) stream.Close();

                stream = new Il2CppSystem.IO.MemoryStream();

                var newDisplayName = slotThatStartWithOne.ToString();
                if (isMain&&sr2ESaveFile.modifiers!=null)
                    foreach (var modifier in sr2ESaveFile.modifiers)
                    {
                        if (string.IsNullOrWhiteSpace(modifier.Key)) continue;
                        try
                        {
                            switch (modifier.Key)
                            {
                                case "feralEnabled":
                                    if (!(modifier.Value is bool)) throw noBoolException;
                                    var newFeralEnabled = modifier.Value.ToString()=="true";
                                    gameState.Summary.FeralEnabled = false;
                                    foreach (var option in gameState.GameSettings.OptionItems)
                                        if (option.PersistenceKey == "setting.FeralEnabled")
                                        {
                                            option.OptionValueKey = newFeralEnabled ? "on" : "off";
                                            break;
                                        }
                                    break;
                                case "tarrEnabled":
                                    if (!(modifier.Value is bool)) throw noBoolException;
                                    var newTarrEnabled = modifier.Value.ToString()=="true";
                                    gameState.Summary.TarrEnabled = false;
                                    foreach (var option in gameState.GameSettings.OptionItems)
                                        if (option.PersistenceKey == "setting.TarrEnabled")
                                        {
                                            option.OptionValueKey = newTarrEnabled ? "on" : "off";
                                            break;
                                        }
                                    break;
                                case "displayName":
                                    newDisplayName = modifier.Value.ToString();
                                    break;
                                case "gameIcon":
                                    var newGameIcon = modifier.Value.ToString();
                                    var i = 0;
                                    bool foundIcon = false;
                                    foreach (var icon in gameState.GameIconIndex.IndexTable)
                                    {
                                        if (icon == newGameIcon)
                                        {
                                            foundIcon = true;
                                            break;
                                        }
                                        i++;
                                    }
                                    if (!foundIcon) throw new Exception("Icon doesn't exist!");
                                    gameState.Summary.IconId = i;
                                    gameState.GameSettings.GameIconId = i;
                                    foreach (var option in gameState.GameSettings.OptionItems)
                                        if (option.PersistenceKey == "setting.Gameicon")
                                        {
                                            option.OptionValueKey = i.ToString();
                                            break;
                                        }
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                            MelonLogger.Error(e);
                            MelonLogger.Error("Error applying modifier: "+modifier.Key);
                        }
                    }
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
        if(loadMenuMenuOnSuccess) ExecuteInTicks(() => {
            systemContext.SceneLoader.LoadMainMenuSceneGroup();
        }, 1);
        if (failedSome) return SomeSaveIDFailed;
        return NoError;
    }
}