using System;
using System.Linq;
using System.Reflection;
using Il2CppTMPro;
using SR2E.Enums;
using SR2E.Managers;
using SR2E.Storage;

namespace SR2E.Utils;

public static class MenuEUtil
{
    internal static Dictionary<string, List<SR2EMenuTheme>> validThemes = new Dictionary<string, List<SR2EMenuTheme>>();
    internal static List<SR2EPopUp> openPopUps = new List<SR2EPopUp>();
    internal static GameObject menuBlock;
    internal static Transform popUpBlock;

    internal static void OpenPopUpBlock(SR2EPopUp popUp)
    {
        if (popUpBlock.transform.GetParent() != popUp.transform.GetParent()) return;
        var instance = GameObject.Instantiate(popUpBlock, popUpBlock.transform);
        instance.gameObject.SetActive(true);
        instance.SetSiblingIndex(popUp.transform.GetSiblingIndex()-1);
        popUp.block = instance;
    }
    internal static void ReloadFont(this SR2EPopUp popUp)
    {
        var dataFont = SR2EMenuFont.SR2;
        try
        {
            var ident = GetOpenMenu().GetMenuIdentifier();
            if (string.IsNullOrEmpty(ident.saveKey)) return;
            if (SR2ESaveManager.data.fonts.TryAdd(ident.saveKey, ident.defaultFont)) SR2ESaveManager.Save();
             dataFont = SR2ESaveManager.data.fonts[ident.saveKey];
        }catch { }
        TMP_FontAsset fontAsset = null;
        switch (dataFont)
        {
            case SR2EMenuFont.Default: fontAsset = SR2EEntryPoint.normalFont; break;
            case SR2EMenuFont.NotoSans: fontAsset = SR2EEntryPoint.notoSansFont; break;
            case SR2EMenuFont.Bold: fontAsset = SR2EEntryPoint.boldFont; break;
            case SR2EMenuFont.Regular: fontAsset = SR2EEntryPoint.regularFont; break;
            case SR2EMenuFont.SR2: fontAsset = SR2EEntryPoint.SR2Font; break;
        }

        if (fontAsset != null) popUp.ApplyFont(fontAsset);
    }
    internal static void ReloadFont(this SR2EMenu menu)
    {
        var ident = menu.GetMenuIdentifier();
        if (string.IsNullOrEmpty(ident.saveKey)) return;
        if (SR2ESaveManager.data.fonts.TryAdd(ident.saveKey, ident.defaultFont)) SR2ESaveManager.Save();
        var dataFont = SR2ESaveManager.data.fonts[ident.saveKey];
        TMP_FontAsset fontAsset = null;
        switch (dataFont)
        {
            case SR2EMenuFont.Default: fontAsset = SR2EEntryPoint.normalFont; break;
            case SR2EMenuFont.NotoSans: fontAsset = SR2EEntryPoint.notoSansFont; break;
            case SR2EMenuFont.Bold: fontAsset = SR2EEntryPoint.boldFont; break;
            case SR2EMenuFont.Regular: fontAsset = SR2EEntryPoint.regularFont; break;
            case SR2EMenuFont.SR2: fontAsset = SR2EEntryPoint.SR2Font; break;
        }

        if (fontAsset != null) menu.ApplyFont(fontAsset);
    }
    internal static GameObject GetMenuRootObject(this Type type)
    {
        try
        {
            var methodInfo = type.GetMethod(nameof(SR2EMenu.GetMenuRootObject), BindingFlags.Static | BindingFlags.Public);
            if (methodInfo == null) return null;
            dynamic result = methodInfo.Invoke(null, null);
            if (result == null) return null;
            if (result is GameObject) return result as GameObject;
        }
        catch (Exception e) { MelonLogger.Error(e); }
        return null;
    }
    internal static MenuIdentifier GetMenuIdentifierByType(this Type type)
    {
        try
        {
            var methodInfo = type.GetMethod(nameof(SR2EMenu.GetMenuIdentifier), BindingFlags.Static | BindingFlags.Public);
            var result = methodInfo.Invoke(null, null);
            if (result == null) return new MenuIdentifier();
            if (result is MenuIdentifier identifier) return identifier;
        }
        catch (Exception e) { MelonLogger.Error(e); }
        return new MenuIdentifier();
    }

    public static MenuIdentifier GetMenuIdentifier(this SR2EMenu menu) => menu.GetType().GetMenuIdentifierByType();
    public static T GetMenu<T>() where T : SR2EMenu
    {
        foreach (var pair in SR2EEntryPoint.menus)
            if (pair.Key is T) return (T)pair.Key;
        return null;
    }
    public static SR2EMenu GetMenu(this MenuIdentifier identifier)
    {
        try
        {
            foreach (var pair in SR2EEntryPoint.menus)
            {
                var ident = pair.Key.GetMenuIdentifier();
                if (ident.saveKey == identifier.saveKey) return pair.Key;
            }
        }
        catch (Exception e) { MelonLogger.Error(e); }
        return null;
    }
    public static SR2EMenuTheme GetTheme(this SR2EMenu menu)
    {
        try
        {
            var methodInfo = menu.GetType().GetMethod(nameof(SR2EMenu.GetMenuIdentifier), BindingFlags.Static | BindingFlags.Public);
            var result = methodInfo.Invoke(null, null);
            if (result is MenuIdentifier identifier)
            {
                SR2ESaveManager.data.themes.TryAdd(identifier.saveKey, identifier.defaultTheme);
                SR2EMenuTheme currentTheme = SR2ESaveManager.data.themes[identifier.saveKey];
                List<SR2EMenuTheme> validThemes = GetValidThemes(identifier.saveKey);
                if (validThemes.Count == 0) return SR2EMenuTheme.Default;
                if(!validThemes.Contains(currentTheme)) currentTheme = validThemes.First();
                return currentTheme;
            }

        }catch (Exception e) {}

        return SR2EMenuTheme.Default;
    }
    
    
    internal static void DoMenuActions(this MenuActions[] actions) => DoMenuActions(actions.ToList());
    internal static void DoMenuActions(this List<MenuActions> actions)
    {
        if(actions.Contains(MenuActions.UnPauseGame)) NativeEUtil.TryUnPauseGame();
        if(actions.Contains(MenuActions.UnPauseGameFalse)) NativeEUtil.TryUnPauseGame(false);
        if(actions.Contains(MenuActions.PauseGameFalse)) NativeEUtil.TryPauseGame(false);
        if(actions.Contains(MenuActions.UnHideMenus)) NativeEUtil.TryUnHideMenus();
        if(actions.Contains(MenuActions.EnableInput)) NativeEUtil.TryEnableSR2Input();
        if(actions.Contains(MenuActions.DisableInput)) NativeEUtil.TryDisableSR2Input();
        if(actions.Contains(MenuActions.PauseGame)&&actions.Contains(MenuActions.HideMenus)) NativeEUtil.TryPauseAndHide();
        else
        {
            if(actions.Contains(MenuActions.HideMenus)) NativeEUtil.TryHideMenus();
            if(actions.Contains(MenuActions.PauseGame)) NativeEUtil.TryPauseGame();
        }

    }
    
    
    public static List<SR2EMenuTheme> GetValidThemes(string menuSaveKey)
    {
        if (validThemes.ContainsKey(menuSaveKey.ToLower()))
            return validThemes[menuSaveKey.ToLower()];
        return new List<SR2EMenuTheme>();
    }



    public static bool isAnyPopUpOpen => openPopUps.Count != 0;
    public static bool isAnyMenuOpen
    {
        get
        {
            try
            {
                foreach (var child in SR2EEntryPoint.SR2EStuff.GetChildren())
                    if (child.activeSelf)
                        if (child.HasComponent<SR2EMenu>())
                            return true;
            } catch  { }
            return false;
        }
    }
    public static void CloseOpenPopUps()
    {
        try
        {
            for (int i = 0; i < SR2EEntryPoint.SR2EStuff.transform.childCount; i++)
            {
                Transform child = SR2EEntryPoint.SR2EStuff.transform.GetChild(i);
                if (child.HasComponent<SR2EPopUp>())
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
        }
        catch { }
    }
    public static void CloseOpenMenu()
    {
        SR2EMenu menu = GetOpenMenu();
        if(menu!=null)
            menu.Close();
    }
    public static SR2EMenu GetOpenMenu()
    {
        foreach (var child in SR2EEntryPoint.SR2EStuff.GetChildren())
        {
            if (!child.activeSelf) continue;
            var menu = child.GetComponent<SR2EMenu>();
            if (menu != null) return menu;
        }
        return null;
    }
    
    
    
    
    private static Sprite _whitePillBg;
    private static Texture2D _whitePillBgTex;

    public static Sprite whitePillBg
    {
        get
        {
            if(_whitePillBg==null)
            {
                _whitePillBgTex = Get<AssetBundle>("cc50fee78e6b7bdd6142627acdaf89fa.bundle")
                    .LoadAsset("Assets/UI/Textures/MenuDemo/whitePillBg.png").Cast<Texture2D>();
                _whitePillBg = Sprite.Create(_whitePillBgTex,
                    new Rect(0f, 0f, _whitePillBgTex.width, _whitePillBgTex.height),
                    new Vector2(0.5f, 0.5f), 1f);
            }

            return _whitePillBg;
        }
    }
    public static Texture2D whitePillBgTex
    {
        get
        {
            if(_whitePillBgTex==null)
            {
                _whitePillBgTex = Get<AssetBundle>("cc50fee78e6b7bdd6142627acdaf89fa.bundle")
                    .LoadAsset("Assets/UI/Textures/MenuDemo/whitePillBg.png").Cast<Texture2D>();
            }

            return _whitePillBgTex;
        }
    }
    
}