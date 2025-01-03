using System;
using System.Reflection;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppMonomiPark.SlimeRancher.UI.Map;
using Il2CppTMPro;
using SR2E.Expansion;
using SR2E.Managers;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Action = System.Action;

namespace SR2E.Menus;

public static class SR2EThemeMenu
{
    internal static Transform parent;
    internal static Transform transform;
    internal static GameObject gameObject;
    private static Dictionary<TextMeshProUGUI, string> toTranslate = new Dictionary<TextMeshProUGUI, string>();

    /// <summary>
    /// Closes the theme menu
    /// </summary>
    public static void Close()
    {
        return;
        if (!EnableThemeMenu.HasFlag()) return;
        menuBlock.SetActive(false);
        gameObject.SetActive(false);
        
        TryUnPauseGame();
        TryUnHideMenus();
        TryEnableSR2Input();
        
    }

    /// <summary>
    /// Opens the theme menu
    /// </summary>
    public static void Open()
    {
        return;
        if (!EnableThemeMenu.HasFlag()) return;
        if (isAnyMenuOpen) return;
        menuBlock.SetActive(true);
        gameObject.SetActive(true);
        TryPauseAndHide();
        //TryDisableSR2Input();
        
        foreach (var pair in toTranslate) pair.Key.SetText(translation(pair.Value));
    }

    /// <summary>
    /// Toggles the theme menu
    /// </summary>
    public static void Toggle()
    {
        if (!EnableThemeMenu.HasFlag()) return;
        if (isOpen) Close();
        else Open();
    }

    public static bool isOpen
    {
        get
        {
            if (!EnableThemeMenu.HasFlag()) return false;
            if(gameObject==null) return false;
            return gameObject.activeSelf;
        }
    }
    
    internal static void Start()
    {
        
    }

    internal static void Update()
    {
        if (isOpen)
        {
            
        }
    }
}