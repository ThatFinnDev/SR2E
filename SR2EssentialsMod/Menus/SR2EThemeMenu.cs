using System;
using System.Reflection;
using Il2CppTMPro;
using SR2E.Enums;
using SR2E.Managers;
using SR2E.Storage;
using UnityEngine.UI;
using Action = System.Action;

namespace SR2E.Menus;

public class SR2EThemeMenu : SR2EMenu
{
    //Check valid themes for all menus EVERYWHERE
    public new static MenuIdentifier GetMenuIdentifier() => new ("thememenu",SR2EMenuTheme.Default,"ThemeMenu");
    public new static void PreAwake(GameObject obj) => obj.AddComponent<SR2EThemeMenu>();
    public override bool createCommands => false;
    public override bool inGameOnly => false;
    

    protected override void OnAwake()
    {
        SR2EEntryPoint.menus.Add(this, new Dictionary<string, object>()
        {
            {"requiredFeatures",new List<FeatureFlag>(){EnableThemeMenu}},
            {"openActions",new List<MenuActions> { MenuActions.PauseGame,MenuActions.HideMenus }},
            {"closeActions",new List<MenuActions> { MenuActions.UnPauseGame,MenuActions.UnHideMenus,MenuActions.EnableInput }},
        });
    }

    protected override void OnClose()
    {
        content.DestroyAllChildren();
    }
    protected override void OnOpen()
    {
        List<MenuIdentifier> identifiers = new List<MenuIdentifier>();
        foreach (var pair in SR2EEntryPoint.menus)
        {
            try
            {
                var methodInfo = pair.Key.GetType().GetMethod(nameof(SR2EMenu.GetMenuIdentifier), BindingFlags.Static | BindingFlags.Public);
                var result = methodInfo.Invoke(null, null);
                if (result is MenuIdentifier identifier) identifiers.Add(identifier);
            }
            catch (Exception e) { MelonLogger.Error(e); }
        }
        foreach (var identifier in identifiers)
        {
            if(getValidThemes(identifier.saveKey).Count<2) continue;
            GameObject entry = Object.Instantiate(entryTemplate, content);
            entry.SetActive(true);
            entry.getObjRec<TextMeshProUGUI>("Title").text = translation(identifier.translationKey+".title");

            foreach (SR2EMenuTheme theme in Enum.GetValues(typeof(SR2EMenuTheme)))
            {
                Transform contentRec = entry.getObjRec<Transform>("ContentRec");
                GameObject button = Instantiate(buttonTemplate, contentRec);
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
    }
    
    GameObject entryTemplate;
    GameObject buttonTemplate;
    Transform content;
    protected override void OnLateAwake()
    {
        entryTemplate = transform.getObjRec<GameObject>("ThemeSelectorEntryRec");
        buttonTemplate = transform.getObjRec<GameObject>("ThemeSelectorEntryButtonEntryRec");
        content = transform.getObjRec<Transform>("ThemeMenuThemeSelectorContentRec");
        
        var button1 = transform.getObjRec<Image>("ThemeMenuThemeSelectorSelectionButtonRec");
        button1.sprite = whitePillBg;
        
        toTranslate.Add(button1.transform.GetChild(0).GetComponent<TextMeshProUGUI>(),"thememenu.category.selector");
        toTranslate.Add(transform.getObjRec<TextMeshProUGUI>("TitleTextRec"),"thememenu.title");
    }

    protected override void OnUpdate()
    {
        if (Key.Escape.OnKeyPressed())
            Close();
    }
}