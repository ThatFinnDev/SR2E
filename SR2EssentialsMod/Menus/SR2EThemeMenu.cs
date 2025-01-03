using System;
using System.Reflection;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppMonomiPark.SlimeRancher.UI.Map;
using Il2CppTMPro;
using SR2E.Expansion;
using SR2E.Managers;
using SR2E.Storage;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Action = System.Action;

namespace SR2E.Menus;

public static class SR2EThemeMenu
{
    public static MenuIdentifier menuIdentifier = new MenuIdentifier(false,"thememenu",SR2EMenuTheme.Default,"ThemeMenu");
    internal static Transform parent;
    internal static Transform transform;
    internal static GameObject gameObject;
    private static Dictionary<TextMeshProUGUI, string> toTranslate = new Dictionary<TextMeshProUGUI, string>();

    /// <summary>
    /// Closes the theme menu
    /// </summary>
    public static void Close()
    {
        if (!EnableThemeMenu.HasFlag()) return;
        menuBlock.SetActive(false);
        gameObject.SetActive(false);


        content.DestroyAllChildren();
        TryUnPauseGame();
        TryUnHideMenus();
        TryEnableSR2Input();
        
    }

    /// <summary>
    /// Opens the theme menu
    /// </summary>
    public static void Open()
    {
        if (!EnableThemeMenu.HasFlag()) return;
        if (isAnyMenuOpen) return;
        menuBlock.SetActive(true);
        gameObject.SetActive(true);
        TryPauseAndHide();
        //TryDisableSR2Input();
        
        foreach (var identifier in new MenuIdentifier[]{SR2EConsole.menuIdentifier,SR2ECheatMenu.menuIdentifier,SR2EModMenu.menuIdentifier,SR2EThemeMenu.menuIdentifier})
        {
            if(!identifier.hasThemes) continue;
            GameObject entry = Object.Instantiate(entryTemplate, content);
            entry.SetActive(true);
            entry.getObjRec<TextMeshProUGUI>("Title").text = translation(identifier.translationKey+".title");

            foreach (SR2EMenuTheme theme in Enum.GetValues(typeof(SR2EMenuTheme)))
            {
                Transform contentRec = entry.getObjRec<Transform>("ContentRec");
                GameObject button = Object.Instantiate(buttonTemplate, contentRec);
                button.SetActive(true);
                button.transform.GetChild(0).GetComponent<Button>().onClick.AddListener((Action)(() =>
                {
                    for (int i = 0; i < contentRec.childCount; i++)
                        contentRec.GetChild(i).GetComponent<Image>().color = contentRec.GetChild(i) == button.transform ? Color.green : Color.red;
                    SR2ESaveManager.data.themes[identifier.saveKey] = theme;
                    SR2ESaveManager.Save();
                }));
                Texture2D texture = new Texture2D(3, 1, TextureFormat.RGBA32, false)
                { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };
                switch (theme)
                {
                    case SR2EMenuTheme.SR2E: if (true) {
                            if(ColorUtility.TryParseHtmlString("#303846FF", out var pixel0)) texture.SetPixel(0,0,pixel0);
                            if(ColorUtility.TryParseHtmlString("#2C6EC8FF", out var pixel1)) texture.SetPixel(1,0,pixel1);
                            if(ColorUtility.TryParseHtmlString("#1B1B1DFF", out var pixel2)) texture.SetPixel(2,0,pixel2);
                    } break;
                    case SR2EMenuTheme.Black: if (true) {
                            if(ColorUtility.TryParseHtmlString("#000000", out var pixel0)) texture.SetPixel(0,0,pixel0);
                            if(ColorUtility.TryParseHtmlString("#000000", out var pixel1)) texture.SetPixel(1,0,pixel1);
                            if(ColorUtility.TryParseHtmlString("#000000", out var pixel2)) texture.SetPixel(2,0,pixel2);
                    } break;
                    default: if (true) {
                        if(ColorUtility.TryParseHtmlString("#F0E1C8FF", out var pixel0)) texture.SetPixel(0,0,pixel0);
                        if(ColorUtility.TryParseHtmlString("#D2B394FF", out var pixel1)) texture.SetPixel(1,0,pixel1);
                        if(ColorUtility.TryParseHtmlString("#FFFFFFFF", out var pixel2)) texture.SetPixel(2,0,pixel2);
                    } break;
                }

                texture.Apply();
                button.transform.GetChild(0).GetComponent<Image>().sprite = SR2EUtils.ConvertToSprite(texture);
                if (SR2ESaveManager.data.themes.ContainsKey(identifier.saveKey))
                {
                    if (SR2ESaveManager.data.themes[identifier.saveKey] == theme)
                        button.GetComponent<Image>().color = Color.green;
                }
            }
        }
        
        
        
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
    
    static GameObject entryTemplate;
    static GameObject buttonTemplate;
    static Transform content;
    internal static void Start()
    {
        entryTemplate = transform.getObjRec<GameObject>("ThemeSelectorEntryRec");
        buttonTemplate = transform.getObjRec<GameObject>("ThemeSelectorEntryButtonEntryRec");
        content = transform.getObjRec<Transform>("ThemeMenuThemeSelectorContentRec");
        
        var button1 = transform.getObjRec<Image>("ThemeMenuThemeSelectorSelectionButtonRec");
        button1.sprite = whitePillBg;
        
        toTranslate.Add(button1.transform.GetChild(0).GetComponent<TextMeshProUGUI>(),"thememenu.category.selector");
        toTranslate.Add(transform.getObjRec<TextMeshProUGUI>("TitleTextRec"),"thememenu.title");
    }

    internal static void Update()
    {
        if (isOpen)
        {
            if (Key.Escape.OnKeyPressed())
                Close();
        }
    }
}