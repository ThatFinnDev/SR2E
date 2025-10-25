﻿using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using System;
using System.Runtime.InteropServices;
using Il2CppInterop.Common;
using Il2CppInterop.Runtime.Startup;
using Il2CppMonomiPark.SlimeRancher.Input;
using Il2CppMonomiPark.SlimeRancher.UI.Framework.CommonControls;
using Il2CppMonomiPark.SlimeRancher.UI.Framework.Layout;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu.Model;
using SR2E.Components;
using SR2E.Enums;
using SR2E.Storage;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SR2E.Patches.MainMenu;
[HarmonyPatch()]
internal static class SaveGameRootUIPatch
{
    static Button exportButton; 
    static Button iconButton; 
    static bool addedAction = false;
    static InputActionReference onPress;
    private static InputEvent inputEvent;
    private static SaveGamesRootUI ui;

    private static Action<InputEventData> action = (Action<InputEventData>)((data) =>
    {
        OnExportButtonPressed();
    });
    static void OnExportButtonPressed()
    {
        if (ui == null) return;
        if (!exportButton.gameObject.active) return;
        var dataBehaviours = ui.FetchButtonBehaviorData();
        var load = dataBehaviours[ui._selectedModelIndex];
        var loadGameBehaviorModel = load.TryCast<LoadGameBehaviorModel>();
        if (loadGameBehaviorModel==null)
        {
            OPENFILENAME ofn = new OPENFILENAME();
            if (GetOpenFileName(ofn))
            {
                string filePath = ofn.lpstrFile;
                if (string.IsNullOrEmpty(filePath)) return;
                var savefile = SR2ESaveFileV01.Load(File.ReadAllBytes(filePath));
                var error = SaveFileEUtil.ImportSaveV01(savefile, ui._selectedModelIndex + 1, true);
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
    }

    /*[HarmonyPostfix, HarmonyPatch(typeof(SaveGamesRootUI), nameof(SaveGamesRootUI.DeInit))]
    internal static void DeInit()
    {
        if(addedAction)
        {
            inputEvent.remove_Performed(action);
            addedAction = false;
        }
    }*/
    [HarmonyPostfix,HarmonyPatch(typeof(SaveGamesRootUI), nameof(SaveGamesRootUI.FocusUI))]
    internal static void Postfix(SaveGamesRootUI __instance)
    {
        SR2EEntryPoint.baseUIAddSliders.Add(__instance);
        if (!AllowSaveExport.HasFlag()) return;
        ui = __instance;
        ExecuteInTicks((Action)(() =>
        {
            RectTransform actionPanel = ui.gameObject.GetObjectRecursively<RectTransform>("ActionPanel");
            if (actionPanel.GetObjectRecursively<Button>("ExportButton") != null) return;
            iconButton = actionPanel.GetObjectRecursively<Button>("IconButton");
            exportButton = GameObject.Instantiate(iconButton, actionPanel);
            exportButton.name = "ExportButton";
            exportButton.onClick.RemoveAllListeners();
            RectTransform exportRectTransform = exportButton.GetComponent<RectTransform>();
            exportRectTransform.anchorMax = new Vector2(1,1);
            exportRectTransform.anchorMin = new Vector2(1,1);
            exportButton.transform.localPosition = new Vector3(566.4542f, 850.0162f, -16.1379f);
            exportButton.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = EmbeddedResourceEUtil.LoadSprite("Assets.exportImportButton.png");
            if(onPress==null) onPress = Get<InputActionReference>("MainGame/Open Map");
            if(inputEvent==null) inputEvent = Get<InputEvent>("OpenMap");
            var inputEventDisplay = exportButton.gameObject.GetObjectRecursively<InputEventDisplay>("KeyIcon");
            inputEventDisplay._inputEvent = inputEvent;
            inputEventDisplay.HandleKeysChanged();
            exportButton.GetComponent<InputEventButton>().InputEvent = inputEvent;
            exportButton.GetComponent<InputEventButton>().Awake();
            exportButton.GetComponent<LayoutManager>().ForceTreeRebuild();
            onPress.action.Enable();
            if(!addedAction)
            {
                inputEvent.add_Performed(action);
                addedAction = true;
            }
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
        public string lpstrFilter = "SR2 Save Files (*"+SR2ESaveFileV01.Extension+")\0*"+SR2ESaveFileV01.Extension+"\0All Files\0*.*\0";
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
        public string lpstrDefExt = SR2ESaveFileV01.Extension.Substring(1);
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
        public string lpstrFilter = "SR2 Save Files (*"+SR2ESaveFileV01.Extension+")\0*"+SR2ESaveFileV01.Extension+"\0All Files\0*.*\0";
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
        public string lpstrDefExt = SR2ESaveFileV01.Extension.Substring(1);
        public IntPtr lCustData = IntPtr.Zero;
        public IntPtr lpfnHook = IntPtr.Zero;
        public string lpTemplateName = null;
    }
}