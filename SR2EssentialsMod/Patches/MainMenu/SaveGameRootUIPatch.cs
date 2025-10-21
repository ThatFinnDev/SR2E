using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using System;
using System.Runtime.InteropServices;
using Il2CppMonomiPark.SlimeRancher.Input;
using Il2CppMonomiPark.SlimeRancher.UI.Framework.Layout;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu.Model;
using SR2E.Enums;
using SR2E.Storage;
using UnityEngine.UI;

namespace SR2E.Patches.MainMenu;

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
                        var savefile = SR2ESaveFileV01.Load(File.ReadAllBytes(filePath));
                        var error = SaveFileEUtil.ImportSaveV01(savefile, __instance._selectedModelIndex + 1, true);
                        if (error != SR2EError.NoError)
                        {
                            MelonLogger.Msg("Error when importing save: "+error);
                            return;
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
                        if (string.IsNullOrEmpty(filePath)) return;
                        var savefile = SaveFileEUtil.ExportSaveV01(loadGameBehaviorModel.GameDataSummary, true);
                        if (savefile == null) return;
                        if(filePath.EndsWith(".json")) File.WriteAllText(filePath,savefile.Export());  
                        else File.WriteAllBytes(filePath,savefile.ExportCompressed());  
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
        public string lpstrFilter = "SR2 Save Files (*.sr2save)\0*"+SR2ESaveFileV01.Extension+"\0All Files\0*.*\0";
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
        public string lpstrFilter = "SR2 Save Files (*.sr2save)\0*"+SR2ESaveFileV01.Extension+"\0All Files\0*.*\0";
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