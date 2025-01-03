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
using UnityEngine.UIElements;
using Action = System.Action;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Toggle = UnityEngine.UI.Toggle;

namespace SR2E.Menus;

public static class SR2EModMenu
{
    internal static Transform parent;
    internal static Transform transform;
    internal static GameObject gameObject;
    static TextMeshProUGUI modInfoText;

    /// <summary>
    /// Closes the mod menu
    /// </summary>
    public static void Close()
    {
        if (!EnableModMenu.HasFlag()) return;
        menuBlock.SetActive(false);
        gameObject.SetActive(false);
        gameObject.getObjRec<Button>("ModMenuModMenuSelectionButtonRec").onClick.Invoke();
        
        TryUnPauseGame();
        TryUnHideMenus();
        TryEnableSR2Input();
        
        Transform modContent = transform.getObjRec<Transform>("ModMenuModMenuContentRec");
        for (int i = 0; i < modContent.childCount; i++)
            Object.Destroy(modContent.GetChild(i).gameObject);
    }


    /// <summary>
    /// Opens the mod menu
    /// </summary>
    public static void Open()
    {
        if (!EnableModMenu.HasFlag()) return;
        if (isAnyMenuOpen) return;
        menuBlock.SetActive(true);
        gameObject.SetActive(true);
        TryPauseAndHide();
        //TryDisableSR2Input();
        
        GameObject buttonPrefab = transform.getObjRec<GameObject>("ModMenuModMenuTemplateButtonRec");
        Transform modContent = transform.getObjRec<Transform>("ModMenuModMenuContentRec");
        foreach (var loadedAssembly in MelonAssembly.LoadedAssemblies) foreach (RottenMelon rotten in loadedAssembly.RottenMelons)
        {
            try
            {
                SR2EExpansionAttribute sr2EExpansionAttribute =
                    rotten.assembly.Assembly.GetCustomAttribute<SR2EExpansionAttribute>();
                bool isSR2EExpansion = sr2EExpansionAttribute != null;
                GameObject obj = GameObject.Instantiate(buttonPrefab, modContent);
                Button b = obj.GetComponent<Button>();
                if (String.IsNullOrEmpty(rotten.assembly.Assembly.FullName))
                    b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = translation("modmenu.modinfo.brokenmodtitle");
                else b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = rotten.assembly.Assembly.FullName; 
                obj.SetActive(true);
                ColorBlock colorBlock = b.colors;colorBlock.normalColor = new Color(0.5f, 0.5f, 0.5f, 1);
                colorBlock.highlightedColor = new Color(0.7f, 0.7f, 0.7f, 1); 
                colorBlock.pressedColor = new Color(0.3f, 0.3f, 0.3f, 1); 
                colorBlock.selectedColor = new Color(0.6f, 0.6f, 0.6f, 1); 
                b.colors = colorBlock;
                

                b.onClick.AddListener((Action)(() =>
                {
                    string name = "";
                    try {name = rotten.assembly.Assembly.FullName; } catch {}
                    if (String.IsNullOrEmpty(name))
                        if(isSR2EExpansion) name = translation("modmenu.modinfo.brokenmodtitle");
                        else name = translation("modmenu.modinfo.brokenexpansiontitle");
                    
                    modInfoText.text = translation("modmenu.modinfo.brokenmod", name);
                    if (isSR2EExpansion)
                        modInfoText.text = translation("modmenu.modinfo.brokenexpansion", name);
                    try {modInfoText.text = translation("modmenu.modinfo.path", rotten.assembly.Assembly.Location);} catch {}
                   
                    try {modInfoText.text += "\n" + translation("modmenu.modinfo.exception", rotten.exception);} catch {}
                    try {modInfoText.text += "\n" + translation("modmenu.modinfo.errorMessage", rotten.errorMessage);} catch {}

                }));
            }
            catch {}

        }
        
        foreach (MelonBase melonBase in MelonBase.RegisteredMelons)
        {
            SR2EExpansionAttribute sr2EExpansionAttribute = melonBase.MelonAssembly.Assembly.GetCustomAttribute<SR2EExpansionAttribute>();
            bool isSR2EExpansion = sr2EExpansionAttribute != null;
            GameObject obj = GameObject.Instantiate(buttonPrefab, modContent);
            Button b = obj.GetComponent<Button>();
            b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = melonBase.Info.Name;
            obj.SetActive(true);
            if (isSR2EExpansion)
            {
                ColorBlock colorBlock = b.colors;
                colorBlock.normalColor = new Color(0.149f, 0.7176f, 0.3961f, 1);
                colorBlock.highlightedColor = new Color(0.1098f, 0.6314f, 0.2157f, 1); 
                colorBlock.pressedColor = new Color(0.1371f, 0.7248f, 0.3792f, 1f);
                colorBlock.selectedColor = new Color(0.8706f, 0.5298f, 0.4216f, 1f);
                b.colors = colorBlock;
            }
            b.onClick.AddListener((Action)(() =>
            {
                modInfoText.text = translation("modmenu.modinfo.mod",melonBase.Info.Name);
                if(isSR2EExpansion) 
                    modInfoText.text = translation("modmenu.modinfo.expansion",melonBase.Info.Name);
                modInfoText.text += "\n" + translation("modmenu.modinfo.author",melonBase.Info.Author);
                
                SR2ECoAuthorAttribute coAuthor = melonBase.MelonAssembly.Assembly.GetCustomAttribute<SR2ECoAuthorAttribute>();
                if (coAuthor != null)
                    if (!String.IsNullOrWhiteSpace(coAuthor.CoAuthor))
                        modInfoText.text += "\n" + translation("modmenu.modinfo.coauthor",coAuthor.CoAuthor);

                string versionText = "\n" + translation("modmenu.modinfo.version",melonBase.Info.Version);
                SR2EDisplayVersion display = melonBase.MelonAssembly.Assembly.GetCustomAttribute<SR2EDisplayVersion>();
                if (display != null)
                    if (!String.IsNullOrWhiteSpace(display.Version))
                        versionText = "\n" + translation("modmenu.modinfo.version",display.Version);
                modInfoText.text += versionText;
                modInfoText.text += "\n";

                if (!String.IsNullOrWhiteSpace(melonBase.Info.DownloadLink))
                    modInfoText.text += "\n" + translation("modmenu.modinfo.link",melonBase.Info.DownloadLink);

                string universalModName = translation("modmenu.modinfo.unknown");
                MelonGameAttribute universalMod =
                    melonBase.MelonAssembly.Assembly.GetCustomAttribute<MelonGameAttribute>();
                if (universalMod != null) universalModName = universalMod.Universal.ToString();
                modInfoText.text += "\n" + translation("modmenu.modinfo.isuniversal",universalModName);


                AssemblyDescriptionAttribute desc =
                    melonBase.MelonAssembly.Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
                if (desc != null)
                    if (!String.IsNullOrWhiteSpace(desc.Description))
                        modInfoText.text += "\n" + translation("modmenu.modinfo.description",desc.Description + "\n");

            }));
        }
        modContent.transform.GetChild(0).GetComponent<Button>().onClick.Invoke();
        
        foreach (var pair in toTranslate) pair.Key.SetText(translation(pair.Value));
    }

    /// <summary>
    /// Toggles the mod menu
    /// </summary>
    public static void Toggle()
    {
        if (!EnableModMenu.HasFlag()) return;
        if (isOpen) Close();
        else Open();
    }

    public static bool isOpen
    {
        get
        {
            if (!EnableModMenu.HasFlag()) return false;
            if(gameObject==null) return false;
            return gameObject.activeSelf;
        }
    }

    static GameObject entryTemplate;
    static GameObject headerTemplate;
    static GameObject warningText;
    static Texture2D modMenuTabImage;
    static List<Key> allPossibleKeys = new List<Key>();
    private static TextMeshProUGUI themeMenuText;
    private static Dictionary<TextMeshProUGUI, string> toTranslate = new Dictionary<TextMeshProUGUI, string>();
    internal static void Start()
    {
        entryTemplate = transform.getObjRec<GameObject>("ModMenuModConfigurationTemplateEntryRec");
        headerTemplate = transform.getObjRec<GameObject>("ModMenuModConfigurationTemplateHeaderRec");
        warningText = transform.getObjRec<GameObject>("ModMenuModConfigurationRestartWarningRec");
        Transform content = transform.getObjRec<Transform>("ModMenuModConfigurationContentRec");
        modInfoText = transform.getObjRec<TextMeshProUGUI>("ModMenuModInfoTextRec");
        foreach (string stringKey in System.Enum.GetNames(typeof(Key)))
            if (!String.IsNullOrEmpty(stringKey))
                if (stringKey != "None")
                {
                    Key key;
                    if (Key.TryParse(stringKey, out key))
                        if (key != null)
                            allPossibleKeys.Add(key);
                }

        allPossibleKeys.Remove(Key.LeftCommand);
        allPossibleKeys.Remove(Key.RightCommand);
        allPossibleKeys.Remove(Key.LeftWindows);
        allPossibleKeys.Remove(Key.RightWindows);

        var button1 = transform.getObjRec<Image>("ModMenuModMenuSelectionButtonRec");
        button1.sprite = whitePillBg;
        var button2 = transform.getObjRec<Image>("ModMenuConfigurationSelectionButtonRec");
        button2.sprite = whitePillBg;
        toTranslate.Add(button1.transform.GetChild(0).GetComponent<TextMeshProUGUI>(),"modmenu.category.modmenu");
        toTranslate.Add(button2.transform.GetChild(0).GetComponent<TextMeshProUGUI>(),"modmenu.category.modconfig");
        toTranslate.Add(transform.getObjRec<TextMeshProUGUI>("TitleTextRec"),"modmenu.title");
        
        Button themeButton = transform.getObjRec<Button>("ThemeMenuButtonRec");
        themeButton.onClick.AddListener((Action)(() =>{ Close(); SR2EThemeMenu.Open(); }));
        toTranslate.Add(themeButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>(),"buttons.thememenu.label");
        foreach (MelonPreferences_Category category in MelonPreferences.Categories)
        {
            GameObject header = Object.Instantiate(headerTemplate, content);
            header.SetActive(true);
            header.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = category.DisplayName;
            foreach (MelonPreferences_Entry entry in category.Entries)
            {
                if (!entry.IsHidden)
                {
                    GameObject obj = Object.Instantiate(entryTemplate, content);
                    obj.SetActive(true);
                    obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = entry.DisplayName;
                    if (!String.IsNullOrEmpty(entry.Description))
                        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text +=
                            $"\n{entry.Description.Replace("\n", " ")}";
                    obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().autoSizeTextContainer = true;
                    obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = entry.GetEditedValueAsString();

                    if (entry.BoxedEditedValue is bool)
                    {
                        obj.transform.GetChild(2).gameObject.SetActive(true);
                        obj.transform.GetChild(2).GetComponent<Toggle>().isOn =
                            bool.Parse(entry.GetEditedValueAsString());
                        obj.transform.GetChild(2).GetComponent<Toggle>().onValueChanged.AddListener((Action<bool>)(
                            (isOn) =>
                            {
                                entry.BoxedEditedValue = isOn;
                                category.SaveToFile(false);
                                if (!entriesWithoutWarning.ContainsKey(entry))
                                    warningText.SetActive(true);
                                else
                                {
                                    System.Action action = entriesWithoutWarning[entry];
                                    if (action != null)
                                        action.Invoke();
                                }

                                obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = isOn.ToString();
                            }));

                    }
                    else if (entry.BoxedEditedValue is int)
                    {
                        obj.transform.GetChild(1).gameObject.SetActive(false);
                        obj.transform.GetChild(3).gameObject.SetActive(true);
                        TMP_InputField inputField = obj.transform.GetChild(3).GetComponent<TMP_InputField>();
                        inputField.restoreOriginalTextOnEscape = false;
                        inputField.text = entry.GetEditedValueAsString();
                        inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
                        inputField.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                            translation("modmenu.modconfig.enterint");
                        inputField.onValueChanged.AddListener((Action<string>)(
                            (text) =>
                            {
                                if (String.IsNullOrEmpty(text))
                                    text = "0";
                                int value;
                                if (int.TryParse(text, out value))
                                {
                                    entry.BoxedEditedValue = value;
                                    category.SaveToFile(false);
                                    if (!entriesWithoutWarning.ContainsKey(entry))
                                        warningText.SetActive(true);
                                    else
                                    {
                                        System.Action action = entriesWithoutWarning[entry];
                                        if (action != null)
                                            action.Invoke();
                                    }
                                }
                                else
                                    inputField.text = int.MaxValue.ToString();
                            }));
                    }
                    else if (entry.BoxedEditedValue is float)
                    {
                        obj.transform.GetChild(1).gameObject.SetActive(false);
                        obj.transform.GetChild(3).gameObject.SetActive(true);
                        TMP_InputField inputField = obj.transform.GetChild(3).GetComponent<TMP_InputField>();
                        inputField.restoreOriginalTextOnEscape = false;
                        inputField.text = entry.GetEditedValueAsString();
                        inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                        inputField.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                            translation("modmenu.modconfig.enterfloat");
                        inputField.onValueChanged.AddListener((Action<string>)(
                            (text) =>
                            {
                                if (String.IsNullOrEmpty(text))
                                    text = "0.0";
                                float value;
                                if (float.TryParse(text, out value))
                                {
                                    entry.BoxedEditedValue = value;
                                    category.SaveToFile(false);
                                    obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
                                    if (!entriesWithoutWarning.ContainsKey(entry))
                                        warningText.SetActive(true);
                                    else
                                    {
                                        System.Action action = entriesWithoutWarning[entry];
                                        if (action != null)
                                            action.Invoke();
                                    }
                                }
                                else
                                    inputField.text = obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;
                            }));
                    }
                    else if (entry.BoxedEditedValue is double)
                    {
                        obj.transform.GetChild(1).gameObject.SetActive(false);
                        obj.transform.GetChild(3).gameObject.SetActive(true);
                        TMP_InputField inputField = obj.transform.GetChild(3).GetComponent<TMP_InputField>();
                        inputField.restoreOriginalTextOnEscape = false;
                        inputField.text = entry.GetEditedValueAsString();
                        inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                        inputField.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                            translation("modmenu.modconfig.enterdouble");
                        inputField.onValueChanged.AddListener((Action<string>)(
                            (text) =>
                            {
                                if (String.IsNullOrEmpty(text))
                                    text = "0.0";
                                double value;
                                if (double.TryParse(text, out value))
                                {
                                    entry.BoxedEditedValue = value;
                                    category.SaveToFile(false);
                                    obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
                                    if (!entriesWithoutWarning.ContainsKey(entry))
                                        warningText.SetActive(true);
                                    else
                                    {
                                        System.Action action = entriesWithoutWarning[entry];
                                        if (action != null)
                                            action.Invoke();
                                    }
                                }
                                else
                                    inputField.text = obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;
                            }));
                    }
                    else if (entry.BoxedEditedValue is string)
                    {
                        obj.transform.GetChild(1).gameObject.SetActive(false);
                        obj.transform.GetChild(3).gameObject.SetActive(true);
                        TMP_InputField inputField = obj.transform.GetChild(3).GetComponent<TMP_InputField>();
                        inputField.restoreOriginalTextOnEscape = false;
                        inputField.text = entry.GetEditedValueAsString();
                        inputField.contentType = TMP_InputField.ContentType.Standard;
                        inputField.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                            translation("modmenu.modconfig.entertext");
                        inputField.onValueChanged.AddListener((Action<string>)((text) =>
                        {
                            entry.BoxedEditedValue = text;
                            category.SaveToFile(false);
                            if (!entriesWithoutWarning.ContainsKey(entry))
                                warningText.SetActive(true);
                            else
                            {
                                System.Action action = entriesWithoutWarning[entry];
                                if (action != null)
                                    action.Invoke();
                            }
                        }));
                    }
                    else if (entry.BoxedEditedValue is Key)
                    {
                        obj.transform.GetChild(1).gameObject.SetActive(false);
                        obj.transform.GetChild(4).gameObject.SetActive(true);
                        Button button = obj.transform.GetChild(4).GetComponent<Button>();
                        TextMeshProUGUI textMesh =
                            obj.transform.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>();
                        textMesh.text = entry.GetEditedValueAsString();
                        button.onClick.AddListener((Action)(() =>
                        {
                            textMesh.text = translation("modmenu.modconfig.keylistening");
                            listeninAction = ((Action<Nullable<Key>>)((inputKey) =>
                            {
                                Nullable<Key> key = inputKey == Key.Escape ? Key.None : inputKey;
                                if (key == null)
                                {
                                    textMesh.text = entry.GetEditedValueAsString();
                                }
                                else
                                {
                                    if (entry.BoxedEditedValue is Key)
                                    {
                                        textMesh.text = key.ToString();
                                        entry.BoxedEditedValue = key.Value;
                                        if (!entriesWithoutWarning.ContainsKey(entry))
                                            warningText.SetActive(true);
                                        else
                                        {
                                            System.Action action = entriesWithoutWarning[entry];
                                            if (action != null)
                                                action.Invoke();
                                        }
                                    }
                                }

                                listeninAction = null;
                            }));
                        }));
                    }
                }
            }
        }
    }

    private static Action<Nullable<Key>> listeninAction = null;

    static void keyWasPressed(Key key)
    {
        if (listeninAction != null)
            listeninAction.Invoke(key);
    }

    internal static void Update()
    {
        if (isOpen)
        {
            if (listeninAction == null)
                if (Key.Escape.OnKeyPressed())
                    Close();
            foreach (Key key in allPossibleKeys)
            {
                try { if(key.OnKeyPressed()) keyWasPressed(key); }
                catch { }
            }

            if (Mouse.current.leftButton.wasPressedThisFrame ||
                Mouse.current.rightButton.wasPressedThisFrame ||
                Mouse.current.middleButton.wasPressedThisFrame ||
                Mouse.current.backButton.wasPressedThisFrame ||
                Mouse.current.forwardButton.wasPressedThisFrame ||
                Mouse.current.leftButton.wasPressedThisFrame ||
                Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (listeninAction != null)
                    listeninAction.Invoke(null);
            }


        }
    }
}