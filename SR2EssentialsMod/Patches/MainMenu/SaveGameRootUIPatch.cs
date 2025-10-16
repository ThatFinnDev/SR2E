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
using UnityEngine.UI;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace SR2E.Patches.MainMenu;

[HarmonyPatch(typeof(SaveGamesRootUI), nameof(SaveGamesRootUI.FocusUI))]
internal static class SaveGameRootUIPatch
{
    internal static Button exportButton;
    internal static Button iconButton;
    internal static int selectedSave = 1;
    internal static string path = null;
    internal static bool load = false;
    internal static void Postfix(SaveGamesRootUI __instance)
    {
        SR2EEntryPoint.baseUIAddSliders.Add(__instance);
        if (!ExperimentalSaveExport.HasFlag()) return;
        selectedSave = 1;
        FileStorageProvider provider = SystemContext.Instance.GetStorageProvider().TryCast<FileStorageProvider>();
        if (provider != null) path = provider.savePath+"/";
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
            exportButton.onClick.AddListener((Action)(() => { 
                if (load)
                {
                    OPENFILENAME ofn = new OPENFILENAME();
                    if (GetOpenFileName(ofn))
                    {
                        string filePath = ofn.lpstrFile;
                        if (!string.IsNullOrEmpty(filePath)) return;
                        using (ZipArchive archive = ZipFile.OpenRead(filePath)) for (int i = 1; i < 6; i++)
                        {
                            ZipArchiveEntry entry = archive.GetEntry("saves/" + i + ".sav");
                            if (entry != null) continue;
                            
                            string savePath = path + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + selectedSave + "_" + i + ".sav";
                            entry.ExtractToFile(savePath, overwrite: true);
                            Il2CppSystem.IO.FileStream stream = new Il2CppSystem.IO.FileStream(savePath, Il2CppSystem.IO.FileMode.Open);
                            try
                            {
                                GameV06 game = new GameV06();
                                game.Load(stream);
                                game.DisplayName = selectedSave.ToString();
                                game.SaveSlotIndex = selectedSave-1;
                                game.Write(stream);
                                if(stream!=null) if(!stream.SafeFileHandle.IsClosed) stream.Close();
                            }
                            catch (Exception e) { MelonLogger.Error(e); }
                        }
                        SystemContext.Instance.SceneLoader.LoadMainMenuSceneGroup();
                    }
                }
                else
                {
                    SAVEFILENAME sfn = new SAVEFILENAME();
                    if (GetSaveFileName(sfn))
                    {
                        string filePath = sfn.lpstrFile;
                        if (!string.IsNullOrEmpty(filePath)) return;
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create)) using (ZipArchive archive = new ZipArchive(fileStream, ZipArchiveMode.Create, true)) foreach (string savePath in Directory.GetFiles(path))
                        {
                            string file = Path.GetFileName(savePath);
                            if(!file.EndsWith(".sav")) continue;
                            if(!file.Contains("_")) continue;
                            var split = file.Split("_");
                            if(split.Length!=3) continue;
                            if(split[1]!=selectedSave.ToString()) continue;
                            var entry = archive.CreateEntry("saves/"+split[2],CompressionLevel.Fastest);
                            using (var zipStream = entry.Open())
                            {
                                var bytes = File.ReadAllBytes(savePath); 
                                zipStream.Write(bytes, 0, bytes.Length);
                            }
                        }
                        
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