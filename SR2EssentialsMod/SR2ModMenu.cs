using System;
using System.Reflection;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppMonomiPark.SlimeRancher.UI.Map;
using Il2CppTMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Action = System.Action;

namespace SR2E
{

    internal static class SR2ModMenu
    {
        internal static Transform parent;
        internal static Transform transform;
        internal static GameObject gameObject;
        static GameObject modMenuBlock;
        static TextMeshProUGUI modInfoText;
        static UIPrefabLoader _uiActivator;
        
        static void Close()
        {
            if (Object.FindObjectsOfType<MapUI>().Length != 0)
                return;
            modMenuBlock.SetActive(false);
            gameObject.SetActive(false);
            gameObject.getObjRec<Button>("ModMenu").onClick.Invoke();

            if(SR2EEntryPoint.mainMenuLoaded)
            {
                foreach (UIPrefabLoader loader in Object.FindObjectsOfType<UIPrefabLoader>())
                    if (loader.gameObject.name == "UIActivator" && loader.uiPrefab.name == "MainMenu" &&
                        loader.parentTransform.name == "MainMenuRoot")
                    {
                        loader.Start();
                        break;
                    }
            }
            else
                SystemContext.Instance.SceneLoader.UnpauseGame();
                
            

            Transform modContent = SR2EUtils.getObjRec<Transform>(transform, "ModContent");
            for (int i = 0; i < modContent.childCount; i++)
                GameObject.Destroy(modContent.GetChild(i).gameObject);
        }

        static MainMenuLandingRootUI _mainMenuLandingRootUI;

        internal static void Open()
        {
            if (SR2Console.isOpen)
                return;
            modMenuBlock.SetActive(true);
            gameObject.SetActive(true);

            if(SR2EEntryPoint.mainMenuLoaded)
                try
                {
                    _mainMenuLandingRootUI = Object.FindObjectOfType<MainMenuLandingRootUI>();
                    _mainMenuLandingRootUI.gameObject.SetActive(false);
                    _mainMenuLandingRootUI.enabled = false;
                    _mainMenuLandingRootUI.Close(true, null);
                }catch{}
            else
                try
                {
                    PauseMenuRoot pauseMenuRoot = Object.FindObjectOfType<PauseMenuRoot>();
                    pauseMenuRoot.Close();
                    SystemContext.Instance.SceneLoader.TryPauseGame();
                }catch{}



            GameObject buttonPrefab = SR2EUtils.getObjRec<GameObject>(transform, "TemplateModButton");
            Transform modContent = SR2EUtils.getObjRec<Transform>(transform, "ModContent");
            foreach (MelonBase melonBase in MelonBase.RegisteredMelons)
            {
                GameObject obj = GameObject.Instantiate(buttonPrefab, modContent);
                Button b = obj.GetComponent<Button>();
                b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = melonBase.Info.Name;
                obj.SetActive(true);
                b.onClick.AddListener((Action)(() =>
                {
                    modInfoText.text = "Mod: " + melonBase.Info.Name;
                    modInfoText.text += "\nby: " + melonBase.Info.Author;
                    modInfoText.text += "\nVersion: " + melonBase.Info.Version;
                    modInfoText.text += "\n";

                    if (!String.IsNullOrEmpty(melonBase.Info.DownloadLink))
                        modInfoText.text += "\nDownloadLink: " + melonBase.Info.DownloadLink;

                    string universalModName = "Unknown";
                    MelonGameAttribute universalMod =
                        melonBase.MelonAssembly.Assembly.GetCustomAttribute<MelonGameAttribute>();
                    if (universalMod != null)
                        universalModName = universalMod.Universal.ToString();
                    modInfoText.text += "\nIsUniversalMod: " + universalModName;


                    AssemblyDescriptionAttribute desc = melonBase.MelonAssembly.Assembly
                        .GetCustomAttribute<AssemblyDescriptionAttribute>();
                    if (desc != null)
                        if (!String.IsNullOrEmpty(desc.Description))
                            modInfoText.text += "\nDescription: " + desc.Description + "\n";

                }));
            }

        }

        static void Toggle()
        {
            if (isOpen)
                Close();
            else
                Open();
        }

        internal static bool isOpen
        {
            get { return gameObject.activeSelf; }
        }

        static GameObject entryTemplate;
        static GameObject headerTemplate;
        static GameObject warningText;
        static Texture2D modMenuTabImage;
        static List<Key> allPossibleKeys=new List<Key>();
        internal static void Start()
        {
            modMenuBlock = parent.getObjRec<GameObject>("modMenuBlock");
            entryTemplate = transform.getObjRec<GameObject>("ModConfigurationEntryTemplate");
            headerTemplate = transform.getObjRec<GameObject>("ModConfigurationHeaderTemplate");
            warningText = transform.getObjRec<GameObject>("ModConfigRestartWarning");
            Transform content = transform.getObjRec<Transform>("ModConfigurationContent");
            modInfoText = transform.getObjRec<TextMeshProUGUI>("ModInfoText");
            gameObject.SetActive(false);
            foreach (string stringKey in System.Enum.GetNames(typeof(Key)))
                if (!String.IsNullOrEmpty(stringKey))
                    if (stringKey != "None")
                    {
                        Key key;
                        if (Key.TryParse(stringKey, out key))
                            if(key!=null)
                                allPossibleKeys.Add(key);
                    }
            allPossibleKeys.Remove(Key.LeftCommand);
            allPossibleKeys.Remove(Key.RightCommand);
            allPossibleKeys.Remove(Key.LeftWindows);
            allPossibleKeys.Remove(Key.RightCommand);


            modMenuTabImage = SR2EUtils.Get<AssetBundle>("cc50fee78e6b7bdd6142627acdaf89fa.bundle").LoadAsset("Assets/UI/Textures/MenuDemo/whitePillBg.png").Cast<Texture2D>();
            var spr = Sprite.Create(modMenuTabImage, new Rect(0f, 0f, modMenuTabImage.width, modMenuTabImage.height), new Vector2(0.5f, 0.5f), 1f);
            transform.getObjRec<Image>("ModMenu").sprite = spr;
            transform.getObjRec<Image>("ModConfiguration").sprite = spr;


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
                            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text += $"\n{entry.Description}";
                        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = entry.GetEditedValueAsString();
                        
                        if (entry.BoxedEditedValue is bool)
                        {
                            obj.transform.GetChild(2).gameObject.SetActive(true);
                            obj.transform.GetChild(2).GetComponent<Toggle>().isOn = bool.Parse(entry.GetEditedValueAsString());
                            obj.transform.GetChild(2).GetComponent<Toggle>().onValueChanged.AddListener((Action<bool>)(
                                (isOn) =>
                                {
                                    entry.BoxedEditedValue = isOn;
                                    category.SaveToFile(false);
                                    if(!entriesWithoutWarning.ContainsKey(entry)) 
                                        warningText.SetActive(true);
                                    else
                                    {
                                        System.Action action = entriesWithoutWarning[entry];
                                        if(action!=null)
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
                            inputField.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Please enter a non decimal number";
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
                                        if(!entriesWithoutWarning.ContainsKey(entry)) 
                                            warningText.SetActive(true);
                                        else
                                        {
                                            System.Action action = entriesWithoutWarning[entry];
                                            if(action!=null)
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
                            inputField.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Please enter a decimal number";
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
                                        if(!entriesWithoutWarning.ContainsKey(entry)) 
                                            warningText.SetActive(true);
                                        else
                                        {
                                            System.Action action = entriesWithoutWarning[entry];
                                            if(action!=null)
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
                            inputField.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Please enter a decimal number";
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
                                        if(!entriesWithoutWarning.ContainsKey(entry)) 
                                            warningText.SetActive(true);
                                        else
                                        {
                                            System.Action action = entriesWithoutWarning[entry];
                                            if(action!=null)
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
                            inputField.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Please enter text";
                            inputField.onValueChanged.AddListener((Action<string>)((text) =>
                            {
                                entry.BoxedEditedValue = text; category.SaveToFile(false);
                                if(!entriesWithoutWarning.ContainsKey(entry))
                                    warningText.SetActive(true);
                                else
                                {
                                    System.Action action = entriesWithoutWarning[entry];
                                    if(action!=null)
                                        action.Invoke();
                                }
                            }));
                        }
                        //KeyCode Conversion still has some issue, this is why it is disabled
                        else if (entry.BoxedEditedValue is Key/*||entry.BoxedEditedValue is KeyCode*/)
                        {
                            obj.transform.GetChild(1).gameObject.SetActive(false);
                            obj.transform.GetChild(4).gameObject.SetActive(true);
                            Button button = obj.transform.GetChild(4).GetComponent<Button>();
                            TextMeshProUGUI textMesh = obj.transform.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>();
                            textMesh.text = entry.GetEditedValueAsString();
                            button.onClick.AddListener((Action)(() =>
                            {
                                textMesh.text = "Listening";
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
                                            if(!entriesWithoutWarning.ContainsKey(entry)) 
                                                warningText.SetActive(true);
                                            else
                                            {
                                                System.Action action = entriesWithoutWarning[entry];
                                                if(action!=null)
                                                    action.Invoke();
                                            }
                                        }
                                        else if (entry.BoxedEditedValue is KeyCode)
                                        {
                                            textMesh.text = SR2EUtils.KeyToKeyCode(key.Value).ToString();
                                            entry.BoxedEditedValue = SR2EUtils.KeyToKeyCode(key.Value);
                                            if(!entriesWithoutWarning.ContainsKey(entry)) 
                                                warningText.SetActive(true);
                                            else
                                            {
                                                System.Action action = entriesWithoutWarning[entry];
                                                if(action!=null)
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

        private static Action<Nullable<Key>> listeninAction =null;
        
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
                    if (Keyboard.current.escapeKey.wasPressedThisFrame)
                        Close();
                    
                
                foreach (Key key in allPossibleKeys)
                {
                    try
                    {
                        if (Keyboard.current[key].wasPressedThisFrame)
                        { keyWasPressed(key); }
                    }catch (Exception ignored) { }
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
}