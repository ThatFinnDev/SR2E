using System;
using System.Reflection;
using Il2CppMonomiPark.SlimeRancher.Script.UI.Pause;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppMonomiPark.SlimeRancher.UI.Map;
using Il2CppTMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Action = System.Action;

namespace SR2E;

public static class SR2EModMenu
{
    internal static Transform parent;
    internal static Transform transform;
    internal static GameObject gameObject;
    static GameObject modMenuBlock;
    static TextMeshProUGUI modInfoText;
    static UIPrefabLoader _uiActivator;

    /// <summary>
    /// Closes the mod menu
    /// </summary>
    public static void Close()
    {
        if (Object.FindObjectsOfType<MapUI>().Length != 0) return;
        modMenuBlock.SetActive(false);
        gameObject.SetActive(false);
        gameObject.getObjRec<Button>("ModMenuModMenuSelectionButtonRec").onClick.Invoke();

        if (SR2EEntryPoint.mainMenuLoaded)
        {
            foreach (UIPrefabLoader loader in Object.FindObjectsOfType<UIPrefabLoader>())
                if (loader.gameObject.name == "UIActivator" && loader.uiPrefab.name == "MainMenu" &&
                    loader.parentTransform.name == "MainMenuRoot")
                {
                    loader.Start();
                    break;
                }
        }
        else SystemContext.Instance.SceneLoader.UnpauseGame();



        Transform modContent = transform.getObjRec<Transform>("ModMenuModMenuContentRec");
        for (int i = 0; i < modContent.childCount; i++)
            Object.Destroy(modContent.GetChild(i).gameObject);
    }

    static MainMenuLandingRootUI _mainMenuLandingRootUI;

    /// <summary>
    /// Opens the mod menu
    /// </summary>
    public static void Open()
    {
        if (SR2EConsole.isOpen) return;
        if (SR2ECheatMenu.isOpen) return;
        modMenuBlock.SetActive(true);
        gameObject.SetActive(true);

        if (SR2EEntryPoint.mainMenuLoaded)
            try
            {
                _mainMenuLandingRootUI = Object.FindObjectOfType<MainMenuLandingRootUI>();
                _mainMenuLandingRootUI.gameObject.SetActive(false);
                _mainMenuLandingRootUI.enabled = false;
                _mainMenuLandingRootUI.Close(true, null);
            }
            catch
            {
            }
        else
        {
            try
            {
                PauseMenuRoot pauseMenuRoot = Object.FindObjectOfType<PauseMenuRoot>(); 
                pauseMenuRoot.Close();
            }catch { }
            try
            {
                SystemContext.Instance.SceneLoader.TryPauseGame();
            }catch { }
            try
            {
                PauseMenuDirector pauseMenuDirector = Object.FindObjectOfType<PauseMenuDirector>(); 
                pauseMenuDirector.PauseGame();
            }catch { }
        }



        GameObject buttonPrefab = transform.getObjRec<GameObject>("ModMenuModMenuTemplateButtonRec");
        Transform modContent = transform.getObjRec<Transform>("ModMenuModMenuContentRec");
        foreach (MelonBase melonBase in MelonBase.RegisteredMelons)
        {
            GameObject obj = GameObject.Instantiate(buttonPrefab, modContent);
            Button b = obj.GetComponent<Button>();
            b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = melonBase.Info.Name;
            obj.SetActive(true);
            b.onClick.AddListener((Action)(() =>
            {
                modInfoText.text = translation("modmenu.modinfo.mod",melonBase.Info.Name);
                modInfoText.text += "\n" + translation("modmenu.modinfo.author",melonBase.Info.Author);
                modInfoText.text += "\n" + translation("modmenu.modinfo.version",melonBase.Info.Version);
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
    }

    /// <summary>
    /// Toggles the mod menu
    /// </summary>
    public static void Toggle()
    {

        if (isOpen) Close();
        else Open();
    }

    internal static bool isOpen
    {
        get { return gameObject == null ? false : gameObject.activeSelf; }
    }

    static GameObject entryTemplate;
    static GameObject headerTemplate;
    static GameObject warningText;
    static Texture2D modMenuTabImage;
    static List<Key> allPossibleKeys = new List<Key>();

    internal static void Start()
    {
        modMenuBlock = parent.getObjRec<GameObject>("modMenuBlockRec");
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
        allPossibleKeys.Remove(Key.RightCommand);
        
        transform.getObjRec<Image>("ModMenuModMenuSelectionButtonRec").sprite = whitePillBg;
        transform.getObjRec<Image>("ModMenuConfigurationSelectionButtonRec").sprite = whitePillBg;


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
                    //KeyCode Conversion doesnt work (because of different keyboard layouts, this is why it is disabled
                    else if (entry.BoxedEditedValue is Key /*||entry.BoxedEditedValue is KeyCode*/)
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
                                    else if (entry.BoxedEditedValue is KeyCode)
                                    {
                                        textMesh.text = KeyToKeyCode(key.Value).ToString();
                                        entry.BoxedEditedValue = KeyToKeyCode(key.Value);
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
        {
            listeninAction.Invoke(key);
        }
    }

    internal static void Update()
    {
        if (isOpen)
        {
            if (listeninAction == null)
                if (Key.Escape.kc().wasPressedThisFrame)
                    Close();


            foreach (Key key in allPossibleKeys)
            {
                try
                {
                    if(key.kc().wasPressedThisFrame)
                    {
                        keyWasPressed(key);
                    }
                }
                catch (Exception ignored)
                {
                }
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
                {
                    listeninAction.Invoke(null);
                }
            }


        }
    }
}