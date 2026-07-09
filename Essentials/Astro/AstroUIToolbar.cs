using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Starlight.Enums;
using Starlight.Menus;
using Starlight.Menus.Development;
using Starlight.Popups;
using Starlight.Storage;
using Starlight.UI;
using Starlight.UI.Blueprints;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Starlight.Astro;

//Highly in WIP
// Inspired by Lunakit from AmethystSzs for SMO
[InjectIntoIL]
internal class AstroUIToolbar : MonoBehaviour
{
    void Start()
    {
        var rect = GetComponent<RectTransform>() ?? gameObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.sizeDelta = Vector2.zero;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;

        var canvas = GetComponentInParent<Canvas>();
        if (canvas && !canvas.GetComponent<GraphicRaycaster>())
            canvas.gameObject.AddComponent<GraphicRaycaster>();

        var theme = AstroUITheme.Create();
        var fontTheme = new FontTheme();

        BuildMenubar().Render(theme, fontTheme, rect);
    }

    private GameObject _currentlyHovered;
    private int _debugFrame = 0;

    void Update()
    {
        _debugFrame++;

        if (!EventSystem.current)
        {
            if (_debugFrame % 120 == 0) Log("[AstroToolbar] EventSystem.current is NULL");
            return;
        }

        var mousePos = Mouse.current?.position.ReadValue() ?? Vector2.zero;

        if (Mouse.current?.leftButton.wasPressedThisFrame == true)
            Log($"[AstroToolbar] LEFT CLICK at {mousePos}");

        var pointerEventData = new PointerEventData(EventSystem.current) { position = mousePos };
        var results = new Il2CppSystem.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        // Log raycast results every click
        if (Mouse.current?.leftButton.wasPressedThisFrame == true)
        {
            Log($"[AstroToolbar] Raycast hit count: {results.Count}, Registry size: {MenubarRegistry.Interactions.Count}");
            foreach (var r in results)
                Log($"[AstroToolbar]   hit: {r.gameObject?.name} (id={r.gameObject?.GetInstanceID()}) | inRegistry: {MenubarRegistry.Interactions.ContainsKey(r.gameObject.GetInstanceID())}");
        }

        GameObject hitObject = null;
        foreach (var result in results)
        {
            if (MenubarRegistry.Interactions.ContainsKey(result.gameObject.GetInstanceID()))
            {
                hitObject = result.gameObject;
                break;
            }
        }

        // Handle Hover enter/exit
        if (hitObject != _currentlyHovered)
        {
            if (_currentlyHovered && MenubarRegistry.Interactions.TryGetValue(_currentlyHovered.GetInstanceID(), out var oldInt))
            {
                oldInt.OnHoverExit?.Invoke();
                if (oldInt.TargetImage) 
                    oldInt.TargetImage.color = oldInt.NormalColor;
            }

            _currentlyHovered = hitObject;

            if (_currentlyHovered && MenubarRegistry.Interactions.TryGetValue(_currentlyHovered.GetInstanceID(), out var newInt))
            {
                newInt.OnHoverEnter?.Invoke();
                if (newInt.TargetImage) 
                    newInt.TargetImage.color = newInt.HoverColor;
            }
        }

        // Handle Click
        if (Mouse.current?.leftButton.wasPressedThisFrame == true)
        {
            if (hitObject && MenubarRegistry.Interactions.TryGetValue(hitObject.GetInstanceID(), out var hitInt))
            {
                Log($"[AstroToolbar] Invoking OnClick for: {hitObject.name}");
                hitInt.OnClick?.Invoke();
            }
            else
                Log($"[AstroToolbar] Click — no matching registry object hit");
        }


        if (hitObject && MenubarRegistry.Interactions.TryGetValue(hitObject.GetInstanceID(), out var pressInt))
        {
            if (pressInt.TargetImage)
                pressInt.TargetImage.color = (Mouse.current?.leftButton.isPressed == true)
                    ? pressInt.PressedColor
                    : pressInt.HoverColor;
        }
    }


    private static bool _dummyAutoSave = true;


    private static MenubarUIBlueprintV01 BaseMenubar() => new()
    {
        Name = "AstroMenubar",
        Anchors = new Vector4(0, 1, 1, 1),
        Pivot = new Vector2(0.5f, 1f),
        Size = Vector2.zero,

        BarHeight = 28f,
        EntryHeight = 24f,
        LabelFontSize = 13f,
        DropdownWidth = 220f,
    };

    private static MenubarUIBlueprintV01 BuildMenubar()
    {
        var menubar = BaseMenubar();

        var fileItem = new MenubarItem { Label = "File" };
        fileItem.OnBeforeOpen = () =>
        {
            fileItem.Entries.Clear();
            if (!inGame)
            {
                fileItem.Entries.Add(new MenubarEntry { Label = "Requires a save", IsDisabled = true });
            }
            else
            {
                fileItem.Entries.Add(new MenubarEntry { Label = "Load", SubEntries = [
                    new MenubarEntry { Label = "File 1", OnClick = HandleFileLoad(0) },
                    new MenubarEntry { Label = "File 2", OnClick = HandleFileLoad(1) },
                    new MenubarEntry { Label = "File 3", OnClick = HandleFileLoad(2) },
                    new MenubarEntry { Label = "File 4", OnClick = HandleFileLoad(3) },
                    new MenubarEntry { Label = "File 5", OnClick = HandleFileLoad(4) },
                ]});
                fileItem.Entries.Add(new MenubarEntry { Label = "Save", OnClick = HandleFileSave() });
                fileItem.Entries.Add(new MenubarEntry { Label = "Save As...", SubEntries = [
                    new MenubarEntry { Label = "File 1", OnClick = HandleFileSaveAs(0) },
                    new MenubarEntry { Label = "File 2", OnClick = HandleFileSaveAs(1) },
                    new MenubarEntry { Label = "File 3", OnClick = HandleFileSaveAs(2) },
                    new MenubarEntry { Label = "File 4", OnClick = HandleFileSaveAs(3) },
                    new MenubarEntry { Label = "File 5", OnClick = HandleFileSaveAs(4) },
                ]});
                fileItem.Entries.Add(new MenubarEntry { Label = "Delete", SubEntries = [
                    new MenubarEntry { Label = "File 1", OnClick = HandleFileDelete(0) },
                    new MenubarEntry { Label = "File 2", OnClick = HandleFileDelete(1) },
                    new MenubarEntry { Label = "File 3", OnClick = HandleFileDelete(2) },
                    new MenubarEntry { Label = "File 4", OnClick = HandleFileDelete(3) },
                    new MenubarEntry { Label = "File 5", OnClick = HandleFileDelete(4) },
                ]});
            }
        };
        menubar.Items.Add(fileItem);

        var settingsItem = new MenubarItem { Label = "Settings" };
        settingsItem.Entries = new List<MenubarEntry>
        {
            new MenubarEntry { Label = "Auto-save", IsChecked = () => _dummyAutoSave, OnClick = () => _dummyAutoSave = !_dummyAutoSave },
        };
        menubar.Items.Add(settingsItem);

        var sceneGroupItem = new MenubarItem { Label = "SceneGroup" };
        sceneGroupItem.OnBeforeOpen = () =>
        {
            sceneGroupItem.Entries = GetAllSceneGroups();
        };
        menubar.Items.Add(sceneGroupItem);

        menubar.Items.AddRange(
        [
            new MenubarItem
            {
                Label = "Windows",
                Entries =
                [
                    //new MenubarEntry { Label = "Inspector", OnClick = :/// },
                    new MenubarEntry { Label = "Mod Menu", OnClick = (()=>{MenuEUtil.GetMenu<StarlightModMenu>().Open();}) },
                    new MenubarEntry { Label = "Studio", OnClick = (()=>{MenuEUtil.GetMenu<StarlightStudioMenu>().Open();}) },
                ]
            },
            new MenubarItem
            {
                Label = "Time",
                Entries =
                [
                    new MenubarEntry { Label = "0x",  OnClick = HandleTime(0.0f) },
                    new MenubarEntry { IsSeparator = true },
                    new MenubarEntry { Label = "0.1x",  OnClick = HandleTime(0.1f) },
                    new MenubarEntry { Label = "0.25x",  OnClick = HandleTime(0.25f) },
                    new MenubarEntry { Label = "0.5x",  OnClick = HandleTime(0.5f) },
                    new MenubarEntry { Label = "0.75x",  OnClick = HandleTime(0.75f) },
                    new MenubarEntry { IsSeparator = true },
                    new MenubarEntry { Label = "1x",  OnClick = HandleTime(1.0f) },
                    new MenubarEntry { IsSeparator = true },
                    new MenubarEntry { Label = "1.5x",  OnClick = HandleTime(1.5f) },
                    new MenubarEntry { Label = "2x",  OnClick = HandleTime(2.0f) },
                    new MenubarEntry { Label = "2.5x",  OnClick = HandleTime(2.5f) },
                    new MenubarEntry { Label = "3.0x",  OnClick = HandleTime(3.0f) },
                    new MenubarEntry { Label = "4.0x",  OnClick = HandleTime(4.0f) },
                    new MenubarEntry { Label = "5.0x",  OnClick = HandleTime(5.0f) },
                    new MenubarEntry { Label = "10.0x",  OnClick = HandleTime(10.0f) },
                    new MenubarEntry { Label = "15.0x",  OnClick = HandleTime(15.0f) },
                ]
            }
        ]);

        return menubar;
    }

    private static List<MenubarEntry> GetAllSceneGroups()
    {
        var list = new List<MenubarEntry>();
        foreach (var group in systemContext.SceneLoader._sceneGroupList.items)
        {
            list.Add(new MenubarEntry { Label = group.ReferenceId, OnClick = HandleSceneGroup(group.ReferenceId),
            });
        }
        return list;
    }
    
    
    private static SystemAction HandleFileLoad(int selection) => (() =>
    {
        Log($"[Astro/File/Load] {selection}");
    });
    private static SystemAction HandleFileSave() => (() =>
    {
        autoSaveDirector.SaveGameAndFlush();
        Log($"[Astro/File/Save]");
    });
    private static SystemAction HandleFileSaveAs(int selection) => (() =>
    {
        Log($"[Astro/File/SaveAs] {selection}");
    });
    private static SystemAction HandleFileDelete(int selection) => (() =>
    {
        Log($"[Astro/File/Delete] {selection}");
    });
    
    private static SystemAction HandleSceneGroup(string selection) => (() =>
    {
        Log($"[Astro/SceneGroup] {selection}");
        SceneGroup sc = null;
        foreach (var g in systemContext.SceneLoader.SceneGroupList.items)
            if (g.ReferenceId == selection)
            {
                sc = g;
                break;
            }
        if (!sc) return;
        if(sc._isGameplay)
        {
            if (!inGame)
            {
                StarlightTextViewerPopUp.Open("This SceneGroup requires a loaded save!");
            }
            else LocationBookmarksUtil.GoToLocationPlayer(sc, new Vector3(0, 50, 0) + new Vector3(0, LocationBookmarksUtil.PLAYER_HEIGHT / 2, 0), Vector3.zero);
        }
        else systemContext.SceneLoader.LoadSceneGroup(sc,null);
    });

    private static SystemAction HandleTime(float selection) => (() =>
    {
        
        Log($"[Astro/Time] {selection}");
        Time.timeScale = selection;
        if (selection == 0.0f)
        {
            NativeEUtil.CustomTimeScale=1f;
        }
        else NativeEUtil.CustomTimeScale=selection;
    });
}