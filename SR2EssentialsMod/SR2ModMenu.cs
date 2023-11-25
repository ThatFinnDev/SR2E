using System;
using System.Collections.Generic;
using System.Reflection;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppMonomiPark.SlimeRancher.UI.Map;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Action = System.Action;
using Object = UnityEngine.Object;

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

        internal static T getObjRec<T>(Transform transform, string name) where T : class
        {
            List<GameObject> totalChildren = getAllChildren(transform);
            for (int i = 0; i < totalChildren.Count; i++)
                if(totalChildren[i].name==name)
                {
                    if (typeof(T) == typeof(GameObject))
                        return totalChildren[i] as T;
                    if (typeof(T) == typeof(Transform))
                        return totalChildren[i].transform as T;
                    if (totalChildren[i].GetComponent<T>() != null)
                        return totalChildren[i].GetComponent<T>();
                }
            return null;
        }

        static List<GameObject> getAllChildren(Transform container)
        {
            List<GameObject> allChildren = new List<GameObject>();
            for (int i = 0; i < container.childCount; i++)
            {
                var child = container.GetChild(i);
                allChildren.Add(child.gameObject);
                allChildren.AddRange(getAllChildren(child));
            }
            return allChildren;
        }
        static void Close()
        {
            if (Object.FindObjectsOfType<MapUI>().Length != 0)
                return;
            modMenuBlock.SetActive(false);
            gameObject.SetActive(false);

            foreach (UIPrefabLoader loader in Object.FindObjectsOfType<UIPrefabLoader>())
            {
                if (loader.gameObject.name == "UIActivator" && loader.uiPrefab.name == "MainMenu" &&
                    loader.parentTransform.name == "MainMenuRoot")
                {
                    loader.Start();
                    break;
                }
            }


            SR2EEntryPoint.CreateModMenuButton();

            Transform modContent = getObjRec<Transform>(transform, "ModContent");
            for (int i = 0; i < modContent.childCount; i++)
            {
                GameObject.Destroy(modContent.GetChild(i).gameObject);
            }

        }

        private static MainMenuLandingRootUI _mainMenuLandingRootUI;

        internal static void Open()
        {
            if (SR2Console.isOpen)
                return;
            if (Object.FindObjectsOfType<MapUI>().Length != 0)
                return;
            modMenuBlock.SetActive(true);
            gameObject.SetActive(true);

            _mainMenuLandingRootUI = Object.FindObjectOfType<MainMenuLandingRootUI>();
            _mainMenuLandingRootUI.gameObject.SetActive(false);
            _mainMenuLandingRootUI.enabled = false;
            _mainMenuLandingRootUI.Close(true, null);



            GameObject buttonPrefab = transform.GetChild(0).gameObject;
            Transform modContent = getObjRec<Transform>(transform, "ModContent");
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
        internal static void Start()
        {
            modMenuBlock = getObjRec<GameObject>(parent, "modMenuBlock");
            entryTemplate = getObjRec<GameObject>(transform, "ModConfigurationEntryTemplate");
            headerTemplate = getObjRec<GameObject>(transform, "ModConfigurationHeaderTemplate");
            warningText = getObjRec<GameObject>(transform, "ModConfigRestartWarning");
            Transform content = getObjRec<Transform>(transform, "ModConfigurationContent");
            modInfoText = getObjRec<TextMeshProUGUI>(transform, "ModInfoText");
            gameObject.SetActive(false);
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
                        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = entry.GetEditedValueAsString();
                        
                        if (entry.BoxedEditedValue is bool)
                        {
                            obj.transform.GetChild(2).gameObject.SetActive(true);
                            obj.transform.GetChild(2).GetComponent<Toggle>().isOn = bool.Parse(entry.GetEditedValueAsString());
                            obj.transform.GetChild(2).GetComponent<Toggle>().onValueChanged.AddListener((Action<bool>)(
                                (isOn) =>
                                {
                                    warningText.SetActive(true);
                                    obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = isOn.ToString();
                                    entry.BoxedEditedValue = isOn;
                                    category.SaveToFile(false);
                                }));

                        }
                        else if (entry.BoxedEditedValue is int)
                        {
                            obj.transform.GetChild(1).gameObject.SetActive(false);
                            obj.transform.GetChild(3).gameObject.SetActive(true);
                            TMP_InputField inputField = obj.transform.GetChild(3).GetComponent<TMP_InputField>();
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
                                        warningText.SetActive(true);
                                        entry.BoxedEditedValue = value;
                                        category.SaveToFile(false);
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
                                        warningText.SetActive(true);
                                        entry.BoxedEditedValue = value;
                                        category.SaveToFile(false);
                                        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
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
                                        warningText.SetActive(true);
                                        entry.BoxedEditedValue = value;
                                        category.SaveToFile(false);
                                        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
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
                            inputField.text = entry.GetEditedValueAsString();
                            inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                            inputField.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Please enter text";
                            inputField.onValueChanged.AddListener((Action<string>)((text) =>
                            {
                                warningText.SetActive(true);
                                entry.BoxedEditedValue = text; category.SaveToFile(false);
                            }));
                        }
                    }
                }
            }
        }

        internal static void Update()
        {
            if (isOpen)
            {
                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                    Close();

            }
        }
    }
}