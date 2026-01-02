using System;
using System.Linq;
using System.Reflection;
using Il2CppInterop.Runtime.Attributes;
using Il2CppMonomiPark.SlimeRancher.Input;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppTMPro;
using SR2E.Components;
using SR2E.Enums;
using SR2E.Enums.Features;
using SR2E.Enums.Sounds;
using SR2E.Expansion;
using SR2E.Managers;
using SR2E.Popups;
using SR2E.Storage;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Action = System.Action;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Toggle = UnityEngine.UI.Toggle;

namespace SR2E.Menus;

public class SR2EModMenu : SR2EMenu
{
    public new static MenuIdentifier GetMenuIdentifier() => new MenuIdentifier("modmenu",SR2EMenuFont.SR2,SR2EMenuTheme.Default,"ModMenu");
    public override bool createCommands => true;
    public override bool inGameOnly => false;
    
    protected override void OnAwake()
    {
        requiredFeatures = new List<FeatureFlag>() { EnableModMenu }.ToArray();
        openActions = new List<MenuActions> { MenuActions.PauseGame, MenuActions.HideMenus }.ToArray();
        closeActions = new List<MenuActions> { MenuActions.UnPauseGame, MenuActions.UnHideMenus, MenuActions.EnableInput }.ToArray();
    }
    
    
    internal static Dictionary<MelonPreferences_Entry, System.Action> entriesWithActions = new Dictionary<MelonPreferences_Entry, Action>();
    TextMeshProUGUI modInfoText;
    GameObject entryTemplate;
    GameObject headerTemplate;
    GameObject warningText;
    Texture2D modMenuTabImage;
    List<Key> allPossibleUnityKeys = new List<Key>();
    private List<KeyCode> allPossibleUnityKeyCodes = new List<KeyCode>();
    private List<LKey> allPossibleLKey = new List<LKey>();
    TextMeshProUGUI themeMenuText;
    Button themeButton;
    private Transform modContent;
    private Transform modConfigContent;

    protected override void OnClose()
    {
        gameObject.GetObjectRecursively<Button>("ModMenuModMenuSelectionButtonRec").onClick.Invoke();
        for (int i = 0; i < modContent.childCount; i++)
            Object.Destroy(modContent.GetChild(i).gameObject);
    }
    
    private InputEvent inputDown;
    private InputEvent inputUp;
    public override void OnGameContext(GameContext gameContext)
    {
        inputDown = Get<InputEvent>("ItemDown");
        inputUp = Get<InputEvent>("ItemUp");
        var refScroll = modContent.parent.parent;
        if (!refScroll.HasComponent<ScrollByMenuKeys>())
        {
            var comp = refScroll.gameObject.AddComponent<ScrollByMenuKeys>();
            comp._scrollDownInput = inputDown;
            comp._scrollUpInput = inputUp;
            comp._scrollPerFrame = 9f;
        }
        var gadgetScroll = modConfigContent.parent.parent;
        if (!gadgetScroll.HasComponent<ScrollByMenuKeys>())
        {
            var comp = gadgetScroll.gameObject.AddComponent<ScrollByMenuKeys>();
            comp._scrollDownInput = inputDown;
            comp._scrollUpInput = inputUp;
            comp._scrollPerFrame = 9f;
        }
    }

    private Dictionary<GameObject, string> modButtons = new ();
    [HideFromIl2Cpp] void ProcessMelon(string melonName, string author, string version,string downloadLink,Assembly assembly,bool isRotten,GameObject buttonPrefab, List<object> rottenInfo)
    {
        bool isSR2EExpansion = false;
        bool usePrism = false;
        var sr2EExpansionAttribute = assembly.GetCustomAttribute<SR2EExpansionAttribute>();
        if (sr2EExpansionAttribute != null)
        {
            isSR2EExpansion = true;
            if (sr2EExpansionAttribute.usePrism) usePrism = true;
        }
        if (!isSR2EExpansion)
        {
            foreach (var meta in assembly.GetCustomAttributes<AssemblyMetadataAttribute>())
            {
                if (meta == null) continue;
                if (meta.Key == SR2EExpansionAttributes.IsExpansion) isSR2EExpansion = meta.Value.ToLower() == "true";
                if (meta.Key == SR2EExpansionAttributes.UsePrism) usePrism = meta.Value.ToLower() == "true";
            }
            if (usePrism && !isSR2EExpansion)
                usePrism = true;
        }

        var obj = Instantiate(buttonPrefab, modContent);
        modButtons.Add(obj,melonName);
        Button b = obj.GetComponent<Button>();
        b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = melonName;
        obj.SetActive(true);
        if (isSR2EExpansion)
        {
            var colorBlock = b.colors;
            colorBlock.normalColor = new Color(0.149f, 0.7176f, 0.3961f, 1);
            colorBlock.highlightedColor = new Color(0.1098f, 0.6314f, 0.2157f, 1);
            colorBlock.pressedColor = new Color(0.1371f, 0.7248f, 0.3792f, 1f);
            colorBlock.selectedColor = new Color(0.8706f, 0.5298f, 0.4216f, 1f);
            b.colors = colorBlock;
        }

        if (isRotten)
        {
            var colorBlock = b.colors;colorBlock.normalColor = new Color(0.5f, 0.5f, 0.5f, 1);
            colorBlock.highlightedColor = new Color(0.7f, 0.7f, 0.7f, 1); 
            colorBlock.pressedColor = new Color(0.3f, 0.3f, 0.3f, 1); 
            colorBlock.selectedColor = new Color(0.6f, 0.6f, 0.6f, 1); 
            b.colors = colorBlock;
        }
        bool useIcon = false;
        try
        {
            var sprite = EmbeddedResourceEUtil.LoadSprite("icon.png",assembly).CopyWithoutMipmaps();
            if (sprite == null) throw new Exception();
            b.transform.GetChild(1).GetComponent<Image>().sprite = sprite;
            b.transform.GetChild(1).gameObject.SetActive(true);
            useIcon = true;
        }
        catch
        {
        }

        if (!useIcon)
        {
            try
            {
                var sprite = EmbeddedResourceEUtil.LoadSprite("Assets.icon.png", assembly).CopyWithoutMipmaps();
                if (sprite == null) throw new Exception();
                b.transform.GetChild(1).GetComponent<Image>().sprite = sprite;
                b.transform.GetChild(1).gameObject.SetActive(true);
                useIcon = true;
            }
            catch
            {
            }
        }

        b.onClick.AddListener((Action)(() =>
        {
            AudioEUtil.PlaySound(MenuSound.Click);
            themeButton.gameObject.SetActive(melonName=="SR2E");
            if (isRotten)
            {
                modInfoText.text = translation("modmenu.modinfo.brokenmod", melonName);
                if (isSR2EExpansion)
                    modInfoText.text = translation("modmenu.modinfo.brokenexpansion", melonName);
            }
            else
            {
                modInfoText.text = translation("modmenu.modinfo.mod", melonName);
                if (isSR2EExpansion)
                    modInfoText.text = translation("modmenu.modinfo.expansion", melonName);
            }
            modInfoText.text += "\n" + translation("modmenu.modinfo.author", author);

            if(isSR2EExpansion)
                modInfoText.text += "\n" + translation("modmenu.modinfo.useprism", usePrism);
            string versionText = "\n" + translation("modmenu.modinfo.version", version);
            

            foreach (var meta in assembly.GetCustomAttributes<AssemblyMetadataAttribute>())
            {
                if (meta == null) continue;
                if (string.IsNullOrWhiteSpace(meta.Key)) continue;
                if (string.IsNullOrWhiteSpace(meta.Value)) continue;
                if (meta.Key == null) continue;
                if (meta.Value == null) continue;
                switch (meta.Key)
                {
                    case SR2EExpansionAttributes.DisplayVersion: try { versionText = "\n" + translation("modmenu.modinfo.version", meta.Value); }catch { } break;
                    case SR2EExpansionAttributes.CoAuthors: try { modInfoText.text += "\n" + translation("modmenu.modinfo.coauthor", meta.Value); }catch { } break;
                    case SR2EExpansionAttributes.Contributors: try { modInfoText.text += "\n" + translation("modmenu.modinfo.contributors", meta.Value); }catch { } break;
                    case SR2EExpansionAttributes.IconB64: if (useIcon) break; try { b.transform.GetChild(1).gameObject.SetActive(true); b.transform.GetChild(1).GetComponent<Image>().sprite = ConvertEUtil.Base64ToTexture2D(meta.Value).Texture2DToSprite(); }catch { } break;
                }
            }

            modInfoText.text += versionText+"\n";
            foreach (var meta in assembly.GetCustomAttributes<AssemblyMetadataAttribute>())
            {
                if (meta == null) continue;
                if (string.IsNullOrWhiteSpace(meta.Key)) continue;
                if (string.IsNullOrWhiteSpace(meta.Value)) continue;
                switch (meta.Key)
                {
                    case SR2EExpansionAttributes.SourceCode: try { modInfoText.text += "\n" + translation("modmenu.modinfo.sourcecode", FormatLink(meta.Value)); }catch { } break;
                    case SR2EExpansionAttributes.Nexus: try { modInfoText.text += "\n" + translation("modmenu.modinfo.nexus", FormatLink(meta.Value)); }catch { } break;
                    case SR2EExpansionAttributes.Discord: try { modInfoText.text += "\n" + translation("modmenu.modinfo.discord", FormatLink(meta.Value)); }catch { } break;
                }
            }

            if (!string.IsNullOrWhiteSpace(downloadLink))
                modInfoText.text += "\n" + translation("modmenu.modinfo.link", FormatLink(downloadLink));

            string universalModName = translation("modmenu.modinfo.unknown");
            var universalMod = assembly.GetCustomAttribute<MelonGameAttribute>();
            if (universalMod != null) universalModName = universalMod.Universal.ToString();
            modInfoText.text += "\n" + translation("modmenu.modinfo.isuniversal", universalModName);

            var desc = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
            if (desc != null)
                if (!string.IsNullOrWhiteSpace(desc.Description))
                    modInfoText.text += "\n" + translation("modmenu.modinfo.description", desc.Description + "\n");

            if (isRotten&&rottenInfo!=null&rottenInfo.Count>=3)
            {
                modInfoText.text += "\n";
                try {modInfoText.text += "\n" + translation("modmenu.modinfo.path", rottenInfo[0]);} catch {}
                try {modInfoText.text += "\n" + translation("modmenu.modinfo.exception", rottenInfo[1]);} catch {}
                try {modInfoText.text += "\n" + translation("modmenu.modinfo.errormessage", rottenInfo[2]);} catch {}
            }
        }));
    }
    protected override void OnOpen()
    {
        GameObject buttonPrefab = transform.GetObjectRecursively<GameObject>("ModMenuModMenuTemplateButtonRec");
        buttonPrefab.SetActive(false);
        modButtons = new();
        foreach (var loadedAssembly in MelonAssembly.LoadedAssemblies) foreach (dynamic rotten in loadedAssembly.RottenMelons)
        {
            // Do it this way to support ML 0.7.1 and newer versions
            try
            {
                Assembly assembly = null;
                string exception = null;
                string errorMessage = null;
                try
                {
                    assembly = rotten.assembly;
                    exception = rotten.exception?.ToString();
                    errorMessage = rotten.errorMessage;
                }
                catch
                {
                    try
                    {
                        assembly = rotten.Assembly.assembly;
                        exception = rotten.exception?.ToString();
                        errorMessage = rotten.errorMessage;
                    }
                    catch { }
                }
                if (assembly == null) break;
                
                string melonName = "";
                try {melonName = assembly.FullName; } catch {}
                if (string.IsNullOrEmpty(melonName)) melonName = translation("modmenu.modinfo.brokentitle");
                ProcessMelon(melonName,"<unknown>","<unknown>",null,
                    assembly, true, buttonPrefab, 
                    new List<object>() { assembly.Location, exception, errorMessage });

            }
            catch (Exception e) { MelonLogger.Error(e); }
        }
        foreach (MelonBase melonBase in MelonBase.RegisteredMelons)
        {
            try
            {
                ProcessMelon(melonBase.Info.Name,melonBase.Info.Author,melonBase.Info.Version,melonBase.Info.DownloadLink,
                    melonBase.MelonAssembly.Assembly,false,buttonPrefab,null);
            }
            catch (Exception e) { MelonLogger.Error(e); }
        }

        foreach (var group in SR2EEntryPoint.brokenExpansions)
        {
            try
            {
                ProcessMelon(group.Item1,group.Item2,group.Item3,group.Item4, group.Item5,true,buttonPrefab,
                    new List<object>() { group.Item5.Location, "Error initializing expansion",  group.Item6 });
            }
            catch (Exception e) { MelonLogger.Error(e); }
        }
        
        var sortedButtons = modButtons.Keys.ToList().OrderBy(obj =>
        {
            var text = modButtons[obj];
            if (text == "SR2E") return " ";
            return text;
        }).ToList();

        for (int i = 0; i < sortedButtons.Count; i++)
            sortedButtons[i].transform.SetSiblingIndex(i);
        modButtons = new ();
        
        modContent.transform.GetChild(0).GetComponent<Button>().onClick.Invoke();
    }

    string FormatLink(string url)
    {
        return $"<link=\"{url}\"><color=#2C6EC8><u>{url}</u></color></link>";
    }

    private List<Action> configTabActions = new List<Action>();
    protected override void OnLateAwake()
    {
        modContent = transform.GetObjectRecursively<Transform>("ModMenuModMenuContentRec");
        modConfigContent = transform.GetObjectRecursively<Transform>("ModMenuModConfigurationContentRec");
        entryTemplate = transform.GetObjectRecursively<GameObject>("ModMenuModConfigurationTemplateEntryRec");
        headerTemplate = transform.GetObjectRecursively<GameObject>("ModMenuModConfigurationTemplateHeaderRec");
        warningText = transform.GetObjectRecursively<GameObject>("ModMenuModConfigurationRestartWarningRec");
        toTranslate.Add(warningText.GetComponent<TextMeshProUGUI>(),"modmenu.warning.restart");
        modInfoText = transform.GetObjectRecursively<TextMeshProUGUI>("ModMenuModInfoTextRec");
        modInfoText.AddComponent<ClickableTextLink>();
        foreach (string stringKey in System.Enum.GetNames(typeof(Key)))
            if (!string.IsNullOrEmpty(stringKey))
                if (stringKey != "None")
                {
                    Key key;
                    if (Key.TryParse(stringKey, out key))
                        if (key != null)
                            allPossibleUnityKeys.Add(key);
                }
        allPossibleUnityKeys.Remove(Key.LeftCommand);
        allPossibleUnityKeys.Remove(Key.RightCommand);
        
        foreach (string stringKey in System.Enum.GetNames(typeof(KeyCode)))
            if (!string.IsNullOrEmpty(stringKey))
                if (stringKey != "None")
                {
                    KeyCode key;
                    if (KeyCode.TryParse(stringKey, out key))
                        if (key != null)
                            allPossibleUnityKeyCodes.Add(key);
                }
        allPossibleUnityKeyCodes.Remove(KeyCode.LeftWindows);
        allPossibleUnityKeyCodes.Remove(KeyCode.RightWindows);
        
        foreach (string stringKey in System.Enum.GetNames(typeof(LKey)))
            if (!string.IsNullOrEmpty(stringKey))
                if (stringKey != "None")
                {
                    LKey key;
                    if (LKey.TryParse(stringKey, out key))
                        if (key != null)
                            allPossibleLKey.Add(key);
                }
        var button1 = transform.GetObjectRecursively<Image>("ModMenuModMenuSelectionButtonRec");
        button1.GetComponent<Button>().onClick.AddListener(SelectCategorySound);
        button1.sprite = whitePillBg;
        var button2 = transform.GetObjectRecursively<Image>("ModMenuConfigurationSelectionButtonRec");
        button2.sprite = whitePillBg;
        button2.GetComponent<Button>().onClick.AddListener(SelectCategorySound);
        button2.GetComponent<Button>().onClick.AddListener((Action)(() =>
        {
            ExecuteInTicks((() =>
            {
                
                foreach (var action in configTabActions)
                    action.Invoke();
            }),1);
        }));
        var button3 = transform.GetObjectRecursively<Image>("ModMenuRepoSelectionButtonRec");
        button3.sprite = whitePillBg;
        button3.GetComponent<Button>().onClick.AddListener(SelectCategorySound);
        button3.GetComponent<Button>().onClick.AddListener((Action)(() =>
        {
            if (EnableRepoMenu.HasFlag())
            {
                Close(); MenuEUtil.GetMenu<SR2ERepoMenu>().OpenC(this);
            }
            else
            {
                AudioEUtil.PlaySound(MenuSound.Error);
                SR2ETextViewer.Open(translation("feature.indevelopment"));
            }
        }));
        toTranslate.Add(button1.transform.GetChild(0).GetComponent<TextMeshProUGUI>(),"modmenu.category.modmenu");
        toTranslate.Add(button2.transform.GetChild(0).GetComponent<TextMeshProUGUI>(),"modmenu.category.modconfig");
        toTranslate.Add(button3.transform.GetChild(0).GetComponent<TextMeshProUGUI>(),"modmenu.category.repo");
        toTranslate.Add(transform.GetObjectRecursively<TextMeshProUGUI>("TitleTextRec"),"modmenu.title");
        
        themeButton = transform.GetObjectRecursively<Button>("ThemeMenuButtonRec");
        themeButton.onClick.AddListener((Action)(() =>{ AudioEUtil.PlaySound(MenuSound.Click); Close(); MenuEUtil.GetMenu<SR2EThemeMenu>().OpenC(this); }));
        toTranslate.Add(themeButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>(),"buttons.thememenu.label");
        foreach (MelonPreferences_Category category in MelonPreferences.Categories)
        {
            GameObject header = Instantiate(headerTemplate, modConfigContent);
            header.SetActive(true);
            header.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = category.DisplayName;
            foreach (MelonPreferences_Entry entry in category.Entries)
            {
                if (!entry.IsHidden)
                {
                    GameObject obj = Instantiate(entryTemplate, modConfigContent);
                    obj.SetActive(true);
                    var entryName = obj.GetObjectRecursively<TextMeshProUGUI>("EntryName");
                    entryName.text = entry.DisplayName;
                    if (!string.IsNullOrEmpty(entry.Description))
                        entryName.text +=
                            $"\n<size=60%>{entry.Description.Replace("\n", " ")}</size>";
                    //entryName.autoSizeTextContainer = true;
                    obj.GetObjectRecursively<TextMeshProUGUI>("Value").text = entry.GetEditedValueAsString();
                    configTabActions.Add(() =>
                    {
                        var rectT = obj.GetComponent<RectTransform>();
                        var newValue = entryName.GetRenderedHeight() + 5;
                        if (newValue < rectT.sizeDelta.y) newValue = rectT.sizeDelta.y;
                        rectT.sizeDelta = new Vector2(rectT.sizeDelta.x, newValue);
                    });
                    if (entry.BoxedEditedValue is bool)
                    {
                        obj.GetObjectRecursively<GameObject>("EntryToggle").SetActive(true);
                        obj.GetObjectRecursively<Toggle>("EntryToggle").isOn =
                            bool.Parse(entry.GetEditedValueAsString());
                        obj.GetObjectRecursively<Toggle>("EntryToggle").onValueChanged.AddListener((Action<bool>)(
                            (isOn) =>
                            {
                                if(isOpen) AudioEUtil.PlaySound(MenuSound.Click);
                                entry.BoxedEditedValue = isOn;
                                category.SaveToFile(false);
                                if (!entriesWithActions.ContainsKey(entry))
                                    warningText.SetActive(true);
                                else
                                {
                                    System.Action action = entriesWithActions[entry];
                                    if (action != null)
                                        action.Invoke();
                                }

                                obj.GetObjectRecursively<TextMeshProUGUI>("Value").text = isOn.ToString();
                            }));

                    }
                    else if (entry.BoxedEditedValue is int)
                    {
                        obj.GetObjectRecursively<GameObject>("Value").SetActive(false);
                        obj.GetObjectRecursively<GameObject>("EntryInput").SetActive(true);
                        TMP_InputField inputField = obj.GetObjectRecursively<TMP_InputField>("EntryInput");
                        inputField.restoreOriginalTextOnEscape = false;
                        inputField.text = entry.GetEditedValueAsString();
                        inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
                        inputField.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                            translation("modmenu.modconfig.enterint");
                        inputField.onValueChanged.AddListener((Action<string>)(
                            (text) =>
                            {
                                if(isOpen) AudioEUtil.PlaySound(MenuSound.Click);
                                if (string.IsNullOrEmpty(text))
                                    text = "0";
                                int value;
                                if (int.TryParse(text, out value))
                                {
                                    entry.BoxedEditedValue = value;
                                    category.SaveToFile(false);
                                    if (!entriesWithActions.ContainsKey(entry))
                                        warningText.SetActive(true);
                                    else
                                    {
                                        System.Action action = entriesWithActions[entry];
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
                        obj.GetObjectRecursively<GameObject>("Value").SetActive(false);
                        obj.GetObjectRecursively<GameObject>("EntryInput").SetActive(true);
                        TMP_InputField inputField = obj.GetObjectRecursively<TMP_InputField>("EntryInput");
                        inputField.restoreOriginalTextOnEscape = false;
                        inputField.text = entry.GetEditedValueAsString();
                        inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                        inputField.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                            translation("modmenu.modconfig.enterfloat");
                        inputField.onValueChanged.AddListener((Action<string>)(
                            (text) =>
                            {
                                if(isOpen) AudioEUtil.PlaySound(MenuSound.Click);
                                if (string.IsNullOrEmpty(text))
                                    text = "0.0";
                                float value;
                                if (float.TryParse(text, out value))
                                {
                                    entry.BoxedEditedValue = value;
                                    category.SaveToFile(false);
                                    obj.GetObjectRecursively<TextMeshProUGUI>("Value").text = text;
                                    if (!entriesWithActions.ContainsKey(entry))
                                        warningText.SetActive(true);
                                    else
                                    {
                                        System.Action action = entriesWithActions[entry];
                                        if (action != null)
                                            action.Invoke();
                                    }
                                }
                                else
                                    inputField.text = obj.GetObjectRecursively<TextMeshProUGUI>("Value").text;
                            }));
                    }
                    else if (entry.BoxedEditedValue is double)
                    {
                        obj.GetObjectRecursively<GameObject>("Value").SetActive(false);
                        obj.GetObjectRecursively<GameObject>("EntryInput").SetActive(true);
                        TMP_InputField inputField = obj.GetObjectRecursively<TMP_InputField>("EntryInput");
                        inputField.restoreOriginalTextOnEscape = false;
                        inputField.text = entry.GetEditedValueAsString();
                        inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                        inputField.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                            translation("modmenu.modconfig.enterdouble");
                        inputField.onValueChanged.AddListener((Action<string>)(
                            (text) =>
                            {
                                if(isOpen) AudioEUtil.PlaySound(MenuSound.Click);
                                if (string.IsNullOrEmpty(text))
                                    text = "0.0";
                                double value;
                                if (double.TryParse(text, out value))
                                {
                                    entry.BoxedEditedValue = value;
                                    category.SaveToFile(false);
                                    obj.GetObjectRecursively<TextMeshProUGUI>("Value").text = text;
                                    if (!entriesWithActions.ContainsKey(entry))
                                        warningText.SetActive(true);
                                    else
                                    {
                                        System.Action action = entriesWithActions[entry];
                                        if (action != null)
                                            action.Invoke();
                                    }
                                }
                                else
                                    inputField.text = obj.GetObjectRecursively<TextMeshProUGUI>("Value").text;
                            }));
                    }
                    else if (entry.BoxedEditedValue is string)
                    {
                        obj.GetObjectRecursively<GameObject>("Value").SetActive(false);
                        obj.GetObjectRecursively<GameObject>("EntryInput").SetActive(true);
                        TMP_InputField inputField = obj.GetObjectRecursively<TMP_InputField>("EntryInput");
                        inputField.restoreOriginalTextOnEscape = false;
                        inputField.text = entry.GetEditedValueAsString();
                        inputField.contentType = TMP_InputField.ContentType.Standard;
                        inputField.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                            translation("modmenu.modconfig.entertext");
                        inputField.onValueChanged.AddListener((Action<string>)((text) =>
                        {
                            if(isOpen) AudioEUtil.PlaySound(MenuSound.Click);
                            entry.BoxedEditedValue = text;
                            category.SaveToFile(false);
                            if (!entriesWithActions.ContainsKey(entry))
                                warningText.SetActive(true);
                            else
                            {
                                System.Action action = entriesWithActions[entry];
                                if (action != null)
                                    action.Invoke();
                            }
                        }));
                    }
                    else if (entry.BoxedEditedValue is Key)
                    {
                        obj.GetObjectRecursively<GameObject>("Value").SetActive(false);
                        obj.GetObjectRecursively<GameObject>("Button").SetActive(true);
                        var button = obj.GetObjectRecursively<Button>("Button");
                        var textMesh = obj.GetObjectRecursively<Transform>("Button").GetChild(0).GetComponent<TextMeshProUGUI>();
                        textMesh.text = entry.GetEditedValueAsString();
                        button.onClick.AddListener((Action)(() =>
                        {
                            if(isOpen) AudioEUtil.PlaySound(MenuSound.Click);
                            textMesh.text = translation("modmenu.modconfig.keylistening");
                            listeningType = 3;
                            listeningAction = ((Action<int>)((integer) =>
                            {
                                Key inputKey = (Key) integer;
                                Key key = inputKey == Key.Escape ? Key.None : inputKey;
                                if (key == null)
                                {
                                    textMesh.text = entry.GetEditedValueAsString();
                                }
                                else
                                {
                                    if (entry.BoxedEditedValue is Key)
                                    {
                                        textMesh.text = key.ToString();
                                        entry.BoxedEditedValue = key;
                                        if (!entriesWithActions.ContainsKey(entry))
                                            warningText.SetActive(true);
                                        else
                                        {
                                            System.Action action = entriesWithActions[entry];
                                            if (action != null)
                                                action.Invoke();
                                        }
                                    }
                                }

                                listeningAction = null;
                            }));
                        }));
                    }
                    else if (entry.BoxedEditedValue is KeyCode)
                    {
                        obj.GetObjectRecursively<GameObject>("Value").SetActive(false);
                        obj.GetObjectRecursively<GameObject>("Button").SetActive(true);
                        var button = obj.GetObjectRecursively<Button>("Button");
                        var textMesh = obj.GetObjectRecursively<Transform>("Button").GetChild(0).GetComponent<TextMeshProUGUI>();
                        textMesh.text = entry.GetEditedValueAsString();
                        button.onClick.AddListener((Action)(() =>
                        {
                            if(isOpen) AudioEUtil.PlaySound(MenuSound.Click);
                            textMesh.text = translation("modmenu.modconfig.keylistening");
                            listeningType = 2;
                            listeningAction = ((Action<int>)((integer) =>
                            {
                                KeyCode inputKey = (KeyCode) integer;
                                KeyCode key = inputKey == KeyCode.Escape ? KeyCode.None : inputKey;
                                if (key == null)
                                {
                                    textMesh.text = entry.GetEditedValueAsString();
                                }
                                else
                                {
                                    if (entry.BoxedEditedValue is KeyCode)
                                    {
                                        textMesh.text = key.ToString();
                                        entry.BoxedEditedValue = key;
                                        if (!entriesWithActions.ContainsKey(entry))
                                            warningText.SetActive(true);
                                        else
                                        {
                                            System.Action action = entriesWithActions[entry];
                                            if (action != null)
                                                action.Invoke();
                                        }
                                    }
                                }

                                listeningAction = null;
                            }));
                        }));
                    }
                    else if (entry.BoxedEditedValue is LKey)
                    {
                        obj.GetObjectRecursively<GameObject>("Value").SetActive(false);
                        obj.GetObjectRecursively<GameObject>("Button").SetActive(true);
                        var button = obj.GetObjectRecursively<Button>("Button");
                        var textMesh = obj.GetObjectRecursively<Transform>("Button").GetChild(0).GetComponent<TextMeshProUGUI>();
                        textMesh.text = entry.GetEditedValueAsString();
                        button.onClick.AddListener((Action)(() =>
                        {
                            if(isOpen) AudioEUtil.PlaySound(MenuSound.Click);
                            textMesh.text = translation("modmenu.modconfig.keylistening");
                            listeningType = 1;
                            listeningAction = ((Action<int>)((integer) =>
                            {
                                LKey inputKey = (LKey) integer;
                                LKey key = inputKey == LKey.Escape ? LKey.None : inputKey;
                                if (key == null)
                                {
                                    textMesh.text = entry.GetEditedValueAsString();
                                }
                                else
                                {
                                    if (entry.BoxedEditedValue is LKey)
                                    {
                                        textMesh.text = key.ToString();
                                        entry.BoxedEditedValue = key;
                                        if (!entriesWithActions.ContainsKey(entry))
                                            warningText.SetActive(true);
                                        else
                                        {
                                            System.Action action = entriesWithActions[entry];
                                            if (action != null)
                                                action.Invoke();
                                        }
                                    }
                                }

                                listeningAction = null;
                            }));
                        }));
                    }
                }
            }
        }
    }

    private static int listeningType = 0;
    static Action<int> listeningAction = null;

    public override void OnCloseUIPressed()
    {
        if (listeningAction != null) return;
        if (MenuEUtil.isAnyPopUpOpen) return;
        
        Close();
    }

    protected override void OnUpdate()
    {
        if(listeningAction !=null) switch (listeningType)
        {
            case 1:
                foreach (LKey key in allPossibleLKey)
                    try { if(key.OnKeyDown()) { listeningAction.Invoke(Convert.ToInt32(key)); } } catch { }
                break;
            case 2:
                foreach (KeyCode key in allPossibleUnityKeyCodes)
                    try { if(key.OnKeyDown()) listeningAction.Invoke(Convert.ToInt32(key)); } catch { }
                break;
            case 3:
                foreach (Key key in allPossibleUnityKeys)
                    try { if(Keyboard.current[key].wasPressedThisFrame) listeningAction.Invoke(Convert.ToInt32(key)); }catch { }
                break;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame ||
            Mouse.current.rightButton.wasPressedThisFrame ||
            Mouse.current.middleButton.wasPressedThisFrame ||
            Mouse.current.backButton.wasPressedThisFrame ||
            Mouse.current.forwardButton.wasPressedThisFrame ||
            Mouse.current.leftButton.wasPressedThisFrame ||
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (listeningAction != null)
                listeningAction.Invoke(0);
        }
    }
}