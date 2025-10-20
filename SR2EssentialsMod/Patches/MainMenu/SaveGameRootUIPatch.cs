using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Input;
using Il2CppMonomiPark.SlimeRancher.Persist;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.UI.Framework.Layout;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu.Model;
using SR2E.Storage;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace SR2E.Patches.MainMenu;
/*
[HarmonyPatch(typeof(StorageProvider), nameof(StorageProvider.GetGameData))]
internal static class SomeCoolTestPatch
{
    internal static void Prefix(StorageProvider __instance, string fileName, Il2CppSystem.IO.MemoryStream dataStream)
    {
        MelonLogger.Msg(fileName);
    }
}*/
/*
[HarmonyPatch(typeof(Il2CppMonomiPark.SlimeRancher.Persist.PersistedDataSet), nameof(Il2CppMonomiPark.SlimeRancher.Persist.PersistedDataSet.Load),typeof(Il2CppSystem.IO.Stream))]
internal static class SomeTestPatch
{
    internal static void Prefix(PersistedDataSet __instance, Il2CppSystem.IO.Stream stream)
    {
        if (__instance.TryCast<GameV08>() != null)
        {
            if (stream == null)
            {
                MelonLogger.Msg("Stream is null");
                return;
            }
            MelonLogger.Msg("Stream: "+stream is Il2CppSystem.IO.Stream;
            MelonLogger.Msg("FileStream: "+stream is Il2CppSystem.IO.FileStream));
            MelonLogger.Msg("MemoryStream: "+stream is Il2CppSystem.IO.MemoryStream);
        }
    }
}*/
[HarmonyPatch(typeof(SaveGamesRootUI), nameof(SaveGamesRootUI.FocusUI))]
internal static class SaveGameRootUIPatch
{
    internal static Button exportButton;
    internal static Button iconButton;
    internal static string path = null;
    internal static void Postfix(SaveGamesRootUI __instance)
    {
        SR2EEntryPoint.baseUIAddSliders.Add(__instance);
        if (!ExperimentalSaveExport.HasFlag()) return;
        FileStorageProvider fileStorageProvider = SystemContext.Instance.GetStorageProvider().TryCast<FileStorageProvider>();
        if (fileStorageProvider != null) path = fileStorageProvider.savePath+"/";
        if(path!=null) ExecuteInTicks((Action)(() =>
        {
            RectTransform actionPanel = __instance.gameObject.GetObjectRecursively<RectTransform>("ActionPanel");
            if (actionPanel.GetObjectRecursively<Button>("ExportButton") != null) return;
            iconButton = actionPanel.GetObjectRecursively<Button>("IconButton");
            exportButton = GameObject.Instantiate(iconButton, actionPanel);
            exportButton.name = "ExportButton";
            exportButton.onClick.RemoveAllListeners();
            RectTransform exportRectTransform = exportButton.GetComponent<RectTransform>();
            exportRectTransform.anchorMax = new Vector2(1,1);
            exportRectTransform.anchorMin = new Vector2(1,1);
            exportButton.transform.localPosition = new Vector3(566.4542f, 850.0162f, -16.1379f);
            exportButton.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);;
            exportButton.GetComponent<InputEventButton>().enabled = false;
            exportButton.GetComponent<LayoutManager>().ForceTreeRebuild();
            exportButton.onClick.AddListener((Action)(() =>
            {
                var dataBehaviours = __instance.FetchButtonBehaviorData();
                var load = dataBehaviours[__instance._selectedModelIndex];
                var loadGameBehaviorModel = load.TryCast<LoadGameBehaviorModel>();
                if (loadGameBehaviorModel==null)
                {
                    OPENFILENAME ofn = new OPENFILENAME();
                    if (GetOpenFileName(ofn))
                    {
                        string filePath = ofn.lpstrFile;
                        if (string.IsNullOrEmpty(filePath)) return;
                        var sr2ESaveFile = SR2ESaveFileV01.Load(filePath, true);
                        if (sr2ESaveFile.IsValid()) return;
                        var storageProvider = autoSaveDirector._storageProvider;
                        foreach (var pair in sr2ESaveFile.savesData)
                        {
                            try
                            {
                                var stream = new Il2CppSystem.IO.MemoryStream(pair.Value);
                                var gameState = new GameV08();

                                gameState.Load(stream);
                                if (stream != null && stream.CanRead) stream.Close();

                                stream = new Il2CppSystem.IO.MemoryStream();

                                var newDisplayName = (__instance._selectedModelIndex+1).ToString();
                                var newGameName = sr2ESaveFile.stamp + "_" + newDisplayName;
                                gameState.DisplayName = newDisplayName;
                                gameState.GameName = newGameName;
                                gameState.SaveSlotIndex = __instance._selectedModelIndex;
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
                                MelonLogger.Error(e);
                                MelonLogger.Error("Error loading save's index "+pair.Key);
                            }
                        }
                        /*using (ZipArchive archive = ZipFile.OpenRead(filePath)) foreach (var entry in archive.Entries)
                        {
                            if (entry == null) continue;
                            if (!entry.Name.EndsWith(".sav")) continue;
                            if(!entry.Name.Contains("_")) continue;
                            var split = entry.Name.Split("_");;
                            if(split.Length!=3) continue;
                            string savePath = path + split[0] + "_" + selectedSave + "_" + split[2]+".tmp";
                            entry.ExtractToFile(savePath, overwrite: true);
                            Il2CppSystem.IO.MemoryStream stream = new Il2CppSystem.IO.MemoryStream(File.ReadAllBytes(savePath));
                            File.Delete(savePath);
                            try
                            {
                                GameV08 game = new GameV08();
                                game.Load(stream);
                                if (stream != null && stream.CanRead) stream.Close();
                                stream = new Il2CppSystem.IO.MemoryStream();
                                game.DisplayName = selectedSave.ToString();
                                game.GameName = split[0] + "_" + selectedSave;
                                game.SaveSlotIndex = selectedSave-1;
                                game.Write(stream);
                                var bytes = stream.ToArray();
                                if (stream != null && stream.CanRead) stream.Close();
                                File.WriteAllBytes(path + split[0] + "_" + selectedSave + "_" + split[2],bytes);
                            }
                            catch (Exception e) { MelonLogger.Error(e); }
                        }*/
                        SystemContext.Instance.SceneLoader.LoadMainMenuSceneGroup();
                    }
                }
                else
                {
                    SAVEFILENAME sfn = new SAVEFILENAME();
                    if (GetSaveFileName(sfn))
                    {
                        string filePath = sfn.lpstrFile;
                        if (string.IsNullOrEmpty(filePath)) return;
                        string gameName = loadGameBehaviorModel.GameDataSummary.Name;
                        if (gameName.Split("_").Length !=2) return;
                        
                        var summaries = autoSaveDirector.GetSavesByGameName(gameName);
                        Dictionary<int,byte[]> savesData = new Dictionary<int,byte[]>();
                        
                        if(summaries.Count==0) return;
                        var storageProvider = autoSaveDirector._storageProvider;
                        foreach (var summary in summaries)
                        {
                            var gameWithAutoSavesName = summary.SaveName;
                            var split = gameWithAutoSavesName.Split("_");
                            if (split.Length != 3) continue;
                            var autoSaveIndex = -1;
                            try
                            {
                                autoSaveIndex = int.Parse(split[2]);
                            } catch { }
                            if (autoSaveIndex < 0) continue;
                            //In the storageProvider the game name also includes the auto save id
                            //Normally gameName is without and SaveName with
                            byte[] gameBytes = null;
                            try
                            {
                                var stream = new Il2CppSystem.IO.MemoryStream();
                                storageProvider.GetGameData(gameWithAutoSavesName,stream);
                                gameBytes = stream.ToArray();
                                if (stream != null && stream.CanRead) stream.Close();
                                        
                            }catch { }
                            if (gameBytes == null || gameBytes.Length == 0) continue;
                            savesData.Add(autoSaveIndex, gameBytes);
                        }

                        if (savesData.Count == 0) return;
                        var sr2ESaveFile = new SR2ESaveFileV01(savesData,gameName.Split("_")[0]);
                        sr2ESaveFile.metaDisplayName = (__instance._selectedModelIndex + 1).ToString(); //maybe actually get it in the future
                        sr2ESaveFile.metaGameName = gameName;
                        sr2ESaveFile.metaSaveSlotIndex = __instance._selectedModelIndex;
                        File.WriteAllText(filePath,sr2ESaveFile.Export());
                    }
                }
            }));
        }), 2);
    }
    [DllImport("comdlg32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool GetSaveFileName([In, Out] SAVEFILENAME ofn);
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private class SAVEFILENAME
    {
        public int lStructSize = Marshal.SizeOf(typeof(SAVEFILENAME));
        public IntPtr hwndOwner = IntPtr.Zero;
        public IntPtr hInstance = IntPtr.Zero;
        public string lpstrFilter = "SR2 Save Files (*.sr2save)\0*.sr2save\0All Files\0*.*\0";
        public string lpstrCustomFilter = null;
        public int nMaxCustFilter = 0;
        public int nFilterIndex = 1;
        public string lpstrFile = new string(new char[512]);
        public int nMaxFile = 260;
        public string lpstrFileTitle = null;
        public int nMaxFileTitle = 0;
        public string lpstrInitialDir = null;
        public string lpstrTitle = "Save Game File";
        public int Flags = 0x00000002 | 0x00080000;
        public short nFileOffset;
        public short nFileExtension;
        public string lpstrDefExt = "sr2save";
        public IntPtr lCustData = IntPtr.Zero;
        public IntPtr lpfnHook = IntPtr.Zero;
        public string lpTemplateName = null;
    }
    [DllImport("comdlg32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool GetOpenFileName([In, Out] OPENFILENAME ofn);
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private class OPENFILENAME
    {
        public int lStructSize = Marshal.SizeOf(typeof(OPENFILENAME));
        public IntPtr hwndOwner = IntPtr.Zero;
        public IntPtr hInstance = IntPtr.Zero;
        public string lpstrFilter = "SR2 Save Files (*.sr2save)\0*.sr2save\0All Files\0*.*\0";
        public string lpstrCustomFilter = null;
        public int nMaxCustFilter = 0;
        public int nFilterIndex = 1;
        public string lpstrFile = new string(new char[512]);
        public int nMaxFile = 260;
        public string lpstrFileTitle = null;
        public int nMaxFileTitle = 0;
        public string lpstrInitialDir = null;
        public string lpstrTitle = "Open Save File";
        public int Flags = 0x00000008 | 0x00001000;
        public short nFileOffset;
        public short nFileExtension;
        public string lpstrDefExt = "sr2save";
        public IntPtr lCustData = IntPtr.Zero;
        public IntPtr lpfnHook = IntPtr.Zero;
        public string lpTemplateName = null;
    }
}