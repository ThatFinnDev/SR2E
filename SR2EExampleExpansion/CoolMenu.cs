using SR2E.Enums;
using SR2E.Enums.Features;
using SR2E.Storage;
using UnityEngine.UI;

namespace SR2EExampleExpansion;

public class CoolMenu : SR2EMenu
{
    public new static MenuIdentifier GetMenuIdentifier() => new("coolmenu", SR2EMenuFont.SR2, SR2EMenuTheme.Default, "CoolMenu");

    public new static GameObject GetMenuRootObject()
    {
        // Create object manually or instantiate from AssetBundle
        GameObject newObj = new GameObject("CoolMenu");
        var c =newObj.AddComponent<Canvas>();
        c.sortingOrder = 20000;
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        newObj.AddComponent<CanvasScaler>();
        newObj.AddComponent<GraphicRaycaster>();
        return newObj;
    }
    // Whether the following commands should be created: open<savekey>, close<savekey>, translate<savekey>
    public override bool createCommands => true;
    // Whether the menu should only be openable while a save is loaded
    public override bool inGameOnly => false;
    protected override void OnAwake()
    {
        requiredFeatures = new List<FeatureFlag>() { EnableThemeMenu }.ToArray();
        openActions = new List<MenuActions> { MenuActions.PauseGame, MenuActions.HideMenus }.ToArray();
        closeActions = new List<MenuActions> { MenuActions.UnPauseGame, MenuActions.UnHideMenus, MenuActions.EnableInput }.ToArray();
    }

    protected override void OnStart()
    {
        var background = new GameObject("Background");
        background.transform.SetParent(transform);
        background.transform.localScale = new Vector3(1, 1, 1);
        background.transform.localPosition = new Vector3(0, 0, 0);
        background.transform.localRotation = Quaternion.identity;
        var rect = background.AddComponent<RectTransform>();
        rect.gameObject.AddComponent<Image>().color = new Color(0.1059f, 0.1059f, 0.1137f, 1f);
        rect.sizeDelta = new Vector2(Screen.currentResolution.width*0.9f, Screen.currentResolution.height*0.9f);
    }
    
    public override void OnCloseUIPressed()
    {
        if (MenuEUtil.isAnyPopUpOpen) return;
        
        Close();
    }
}