using System;
using Il2CppTMPro;
using SR2E.Enums;
using SR2E.Enums.Features;
using SR2E.Enums.Sounds;
using SR2E.Managers;
using SR2E.Storage;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Action = System.Action;

namespace SR2E.Menus;

public class SR2EThemeMenu : SR2EMenu
{
    //Check valid themes for all menus EVERYWHERE
    public new static MenuIdentifier GetMenuIdentifier() => new ("thememenu",SR2EMenuFont.SR2,SR2EMenuTheme.Default,"ThemeMenu");
    public override bool createCommands => false;
    public override bool inGameOnly => false;
    
    GameObject entryTemplate;
    GameObject buttonTemplate;
    GameObject dropdownTemplate;
    Transform content;

    protected override void OnAwake()
    {
        requiredFeatures = new List<FeatureFlag>() { EnableThemeMenu }.ToArray();
        openActions = new List<MenuActions> { MenuActions.PauseGame, MenuActions.HideMenus }.ToArray();
        closeActions = new List<MenuActions> { MenuActions.UnPauseGame, MenuActions.UnHideMenus, MenuActions.EnableInput }.ToArray();
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
            var ident = pair.Key.GetMenuIdentifier();
            
            if (!string.IsNullOrEmpty(ident.saveKey)) identifiers.Add(ident);
        }
        foreach (var identifier in identifiers)
        {
            GameObject entry = Object.Instantiate(entryTemplate, content);
            entry.SetActive(true);
            entry.GetObjectRecursively<TextMeshProUGUI>("Title").text = translation(identifier.translationKey+".title");

            Transform contentRec = entry.GetObjectRecursively<Transform>("ContentRec");
            GameObject dropDownObj = Instantiate(dropdownTemplate, contentRec);
            dropDownObj.SetActive(true);
            TMP_Dropdown dropdown = dropDownObj.GetObjectRecursively<TMP_Dropdown>("Dropdown");
            dropDownObj.GetObjectRecursively<Canvas>("Canvas").overrideSorting=false;
            dropdown.ClearOptions();
            //idk how to convert to il2cpp list
            var options = new Il2CppSystem.Collections.Generic.List<string>();
            var fonts = new List<SR2EMenuFont>();
            var currValue = 0;
            var z = 0;
            foreach(SR2EMenuFont font in Enum.GetValues(typeof(SR2EMenuFont)))
            {
                fonts.Add(font);
                if (SR2ESaveManager.data.fonts[identifier.saveKey] == font) currValue = z;
                options.Add(font.ToString());
                z += 1;
            }
            dropdown.AddOptions(options);
            dropdown.value = currValue;
            dropdown.RefreshShownValue();
            dropdown.onValueChanged.AddListener((Action<int>)((value) =>
            {
                AudioEUtil.PlaySound(MenuSound.Click);
                SR2ESaveManager.data.fonts[identifier.saveKey]=fonts[value];
                SR2ESaveManager.Save();
                var menu = identifier.GetMenu();
                if (menu != null)
                    menu.ReloadFont();
            }));
            foreach (SR2EMenuTheme theme in MenuEUtil.GetValidThemes(identifier.saveKey))
            {
                GameObject button = Instantiate(buttonTemplate, contentRec);
                button.SetActive(true);
                button.transform.GetChild(0).GetComponent<Button>().onClick.AddListener((Action)(() =>
                {
                    AudioEUtil.PlaySound(MenuSound.Click);
                    warningText.SetActive(true);
                    for (int i = 0; i < contentRec.childCount; i++)
                        if(!contentRec.GetChild(i).HasComponent<CanvasGroup>())
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
                button.transform.GetChild(0).GetComponent<Image>().sprite = ConvertEUtil.Texture2DToSprite(texture);
                if (SR2ESaveManager.data.themes.ContainsKey(identifier.saveKey))
                {
                    if (SR2ESaveManager.data.themes[identifier.saveKey] == theme)
                        button.GetComponent<Image>().color = Color.green;
                }
            }
        }
    }
    
    GameObject warningText;
    protected override void OnLateAwake()
    {
        entryTemplate = transform.GetObjectRecursively<GameObject>("ThemeSelectorEntryRec");
        buttonTemplate = transform.GetObjectRecursively<GameObject>("ThemeSelectorEntryButtonEntryRec");
        dropdownTemplate = transform.GetObjectRecursively<GameObject>("ThemeSelectorEntryDropdownEntryRec");
        warningText = transform.GetObjectRecursively<GameObject>("ThemeMenuRestartWarningRec");
        toTranslate.Add(warningText.GetComponent<TextMeshProUGUI>(),"thememenu.warning.restart");
        content = transform.GetObjectRecursively<Transform>("ThemeMenuThemeSelectorContentRec");
        
        var button1 = transform.GetObjectRecursively<Image>("ThemeMenuThemeSelectorSelectionButtonRec");
        button1.sprite = whitePillBg;
        
        toTranslate.Add(button1.transform.GetChild(0).GetComponent<TextMeshProUGUI>(),"thememenu.category.selector");
        toTranslate.Add(transform.GetObjectRecursively<TextMeshProUGUI>("TitleTextRec"),"thememenu.title");
    }
    public override void OnCloseUIPressed()
    {
        if (MenuEUtil.isAnyPopUpOpen) return;
        
        Close();
    }

}