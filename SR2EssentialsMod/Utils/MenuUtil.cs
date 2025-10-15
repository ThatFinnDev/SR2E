using System;
using System.Linq;
using System.Reflection;
using Il2CppTMPro;
using SR2E.Enums;
using SR2E.Managers;
using SR2E.Storage;

namespace SR2E.Utils;

public static class MenuUtil
{
    internal static Dictionary<string, List<SR2EMenuTheme>> validThemes = new Dictionary<string, List<SR2EMenuTheme>>();
    internal static void ReloadFont(this SR2EPopUp popUp)
    {
        var ident = GetOpenMenu().GetIdentifierViaReflection();
        if (string.IsNullOrEmpty(ident.saveKey)) return;
        if (SR2ESaveManager.data.fonts.TryAdd(ident.saveKey, ident.defaultFont)) SR2ESaveManager.Save();
        var dataFont = SR2ESaveManager.data.fonts[ident.saveKey];
        TMP_FontAsset fontAsset = null;
        switch (dataFont)
        {
            case SR2EMenuFont.Default: fontAsset = SR2EEntryPoint.normalFont; break;
            case SR2EMenuFont.Bold: fontAsset = SR2EEntryPoint.boldFont; break;
            case SR2EMenuFont.Regular: fontAsset = SR2EEntryPoint.regularFont; break;
            case SR2EMenuFont.SR2: fontAsset = SR2EEntryPoint.SR2Font; break;
        }

        if (fontAsset != null) popUp.ApplyFont(fontAsset);
    }
    internal static void ReloadFont(this SR2EMenu menu)
    {
        var ident = menu.GetIdentifierViaReflection();
        if (string.IsNullOrEmpty(ident.saveKey)) return;
        if (SR2ESaveManager.data.fonts.TryAdd(ident.saveKey, ident.defaultFont)) SR2ESaveManager.Save();
        var dataFont = SR2ESaveManager.data.fonts[ident.saveKey];
        TMP_FontAsset fontAsset = null;
        switch (dataFont)
        {
            case SR2EMenuFont.Default: fontAsset = SR2EEntryPoint.normalFont; break;
            case SR2EMenuFont.Bold: fontAsset = SR2EEntryPoint.boldFont; break;
            case SR2EMenuFont.Regular: fontAsset = SR2EEntryPoint.regularFont; break;
            case SR2EMenuFont.SR2: fontAsset = SR2EEntryPoint.SR2Font; break;
        }

        if (fontAsset != null) menu.ApplyFont(fontAsset);
    }

    internal static MenuIdentifier GetIdentifierViaReflection(this SR2EMenu menu) => menu.GetType().GetIdentifierViaReflection();
    internal static MenuIdentifier GetIdentifierViaReflection(this Type type)
    {
        try
        {
            var methodInfo = type.GetMethod(nameof(SR2EMenu.GetMenuIdentifier), BindingFlags.Static | BindingFlags.Public);
            var result = methodInfo.Invoke(null, null);
            if (result is MenuIdentifier identifier) return identifier;
        }
        catch (Exception e) { MelonLogger.Error(e); }
        return new MenuIdentifier();
    }

    internal static SR2EMenu GetSR2EMenu(this MenuIdentifier identifier)
    {
        try
        {
            foreach (var pair in SR2EEntryPoint.menus)
            {
                var ident = pair.Key.GetIdentifierViaReflection();
                if (ident.saveKey == identifier.saveKey) return pair.Key;
            }
        }
        catch (Exception e) { MelonLogger.Error(e); }
        return null;
    }
    
    internal static void DoMenuActions(this MenuActions[] actions) => DoMenuActions(actions.ToList());
    internal static void DoMenuActions(this List<MenuActions> actions)
    {
        if(actions.Contains(MenuActions.UnPauseGame)) TryUnPauseGame();
        if(actions.Contains(MenuActions.UnPauseGameFalse)) TryUnPauseGame(false);
        if(actions.Contains(MenuActions.PauseGameFalse)) TryPauseGame(false);
        if(actions.Contains(MenuActions.UnHideMenus)) TryUnHideMenus();
        if(actions.Contains(MenuActions.EnableInput)) TryEnableSR2Input();
        if(actions.Contains(MenuActions.DisableInput)) TryDisableSR2Input();
        if(actions.Contains(MenuActions.PauseGame)&&actions.Contains(MenuActions.HideMenus)) TryPauseAndHide();
        else
        {
            if(actions.Contains(MenuActions.HideMenus)) TryHideMenus();
            if(actions.Contains(MenuActions.PauseGame)) TryPauseGame();
        }

    }
    
    
    public static List<SR2EMenuTheme> GetValidThemes(string menuSaveKey)
    {
        if (validThemes.ContainsKey(menuSaveKey.ToLower()))
            return validThemes[menuSaveKey.ToLower()];
        return new List<SR2EMenuTheme>();
    }



    
    public static bool isAnyMenuOpen
    {
        get
        {
            try
            {
                for (int i = 0; i < SR2EEntryPoint.SR2EStuff.transform.childCount; i++)
                    if (SR2EEntryPoint.SR2EStuff.transform.GetChild(i).name.Contains("(Clone)"))
                        if(SR2EEntryPoint.SR2EStuff.transform.GetChild(i).gameObject.activeSelf)
                            return true;
            } catch  { }
            return false;
        }
    }

    public static void CloseOpenMenu()
    {
        SR2EMenu menu = GetOpenMenu();
        if(menu!=null)
            menu.Close();
    }
    public static SR2EMenu GetOpenMenu()
    {
            for (int i = 0; i < SR2EEntryPoint.SR2EStuff.transform.childCount; i++)
                if (SR2EEntryPoint.SR2EStuff.transform.GetChild(i).name.Contains("(Clone)"))
                {
                    if (SR2EEntryPoint.SR2EStuff.transform.GetChild(i).gameObject.activeSelf)
                        return SR2EEntryPoint.SR2EStuff.transform.GetChild(i).gameObject.GetComponent<SR2EMenu>();
                }
            return null;
        
    }
    
    
    
    
}