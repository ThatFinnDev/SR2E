using System;
using Il2CppMonomiPark.SlimeRancher.DebugTool;
using Il2CppTMPro;
using SR2E.Components;
using SR2E.Enums;
using SR2E.Enums.Features;
using SR2E.Managers;
using SR2E.Storage;
using UnityEngine.UI;

namespace SR2E.Menus.Debug;

internal class SR2ENativeDebugUI : SR2EMenu
{
    // TODO
    // DebugUI contains like nothing :/
    // It gets activated by instantiating
    // There are 2 variants, one for keyboard, one for gamepad
    // It has a prefab and some input actions
    // Maybe some helper like:
    // GameDebugDirectorHelper
    // SceneDebugDirectorHelper
    // Also what are all of those DebugUIHandler <Things>
    internal DebugDirectorFixer ddf => DebugDirectorFixer.Instance;
    public new static MenuIdentifier GetMenuIdentifier() => new ("nativedebugui",SR2EMenuFont.SR2,SR2EMenuTheme.Default,"NativeDebugUI");
    public override bool createCommands => false;
    public override bool inGameOnly => true;
    protected override void OnAwake()
    {
        requiredFeatures = new List<FeatureFlag>() { RestoreDebugDebugUI }.ToArray();
        openActions = new List<MenuActions> { MenuActions.PauseGameFalse, }.ToArray();
        closeActions = new List<MenuActions> { MenuActions.UnPauseGameFalse, MenuActions.EnableInput }.ToArray();
    }

    private GameObject debugUIPrefab => ddf.director._uiDefaultPrefab;
    private List<DebugUI> debugUIs = new List<DebugUI>();
    private DebugUI rootDebugUI = null;

    private DebugUIEntry[] rootEntries = new []
    {
        new DebugUIEntry() { text = "TestButton" },
        new DebugUIEntry() { text = "Toggle Noclip", action = () => SR2ECommandManager.ExecuteByString("noclip")},
        new DebugUIEntry() { text = "SubMenu", closesMenu = false,action = () => MenuEUtil.GetMenu<SR2ENativeDebugUI>().OpenEntries(
            new []
            {
                new DebugUIEntry() { text = "SubButton" },
                new DebugUIEntry() { text = "SubMenu", closesMenu = false, action = () => MenuEUtil.GetMenu<SR2ENativeDebugUI>().OpenEntries(new []
                {
                    new DebugUIEntry() { text = "Subsubbutton" },
                }) },
            }) },
    };
    
    // In reality it's tab
    internal static readonly LKey openKey = LKey.F10;
    public new static GameObject GetMenuRootObject()
    {
        var obj = new GameObject("NativeDebugUI");
        var rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 1);
        rect.offsetMin = Vector2.zero; 
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
        rect.localPosition = Vector2.zero;
        rect.sizeDelta = rect.GetParentSize();
        return obj;
    }
    protected override void OnOpen()
    {
        GetComponent<RectTransform>().sizeDelta=GetComponent<RectTransform>().GetParentSize();
        foreach (var debugUI in debugUIs) Destroy(debugUI.gameObject);
        debugUIs = new();

        rootDebugUI = OpenEntries(rootEntries);
    }

    protected override void OnClose()
    {
        foreach (var debugUI in debugUIs) Destroy(debugUI.gameObject);
        debugUIs = new();
    }
    public void GoBack()
    {
        CloseEntries(debugUIs[debugUIs.Count - 1]);
    }
    
    
    public void CloseEntries(DebugUI toClose)
    {
        foreach (var ui in debugUIs) ui.gameObject.SetActive(false);
        debugUIs.Remove(toClose);
        Destroy(toClose.gameObject);
        debugUIs[debugUIs.Count-1].gameObject.SetActive(true);
    }
    public DebugUI OpenEntries(params DebugUIEntry[] buttons)
    {
        foreach (var ui in debugUIs) ui.gameObject.SetActive(false);
        var instance = Instantiate(debugUIPrefab, null);
        instance.transform.parentInternal = transform;
        var debugUI = instance.GetComponent<DebugUI>();
        debugUIs.Add(debugUI);

        foreach (var b in buttons) if(b!=null) AddButton(debugUI,b);
        
        return debugUI;
    }
    public void AddButton(DebugUI debugUI,DebugUIEntry entry)
    {
        if (entry == null) return;
        var instance = Instantiate(debugUI.buttonPrefab, debugUI.grid.transform);
        instance.GetObjectRecursively<TextMeshProUGUI>("Name").text = entry.text;
        
        if (entry.icon == null) instance.GetObjectRecursively<GameObject>("Icon").SetActive(false);
        else instance.GetObjectRecursively<Image>("Icon").sprite = entry.icon;

        var b = instance.GetObjectRecursively<Button>("Content");
        if(entry.action!=null) b.onClick.AddListener(entry.action);
        if(entry.closesMenu) b.onClick.AddListener((Action)(() => CloseEntries(debugUI)));
    }
    
    public override void OnCloseUIPressed()
    {
        if (MenuEUtil.isAnyPopUpOpen) return;
        if(debugUIs.Count>1) GoBack();
        else Close();
    }
}