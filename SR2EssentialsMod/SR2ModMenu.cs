using System;
using System.Reflection;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppMonomiPark.SlimeRancher.UI.Map;
using Il2CppTMPro;
using MelonLoader;
using SR2E;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using Action = System.Action;
using Object = UnityEngine.Object;

namespace SR2E
{

    public static class SR2ModMenu
    {
        internal static Transform parent;
        internal static Transform transform;
        internal static GameObject gameObject;
        static TextMeshProUGUI modInfoText;
        static UIPrefabLoader _uiActivator;
        //static EventSystem eventSystem;

        static T getObjRec<T>(Transform transform, string name)
        {

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.name == name)
                {
                    return child.gameObject.GetComponent<T>();
                }
                else if (child.childCount > 0)
                {
                    T potentionalAnswer = getObjRec<T>(child.transform, name);
                    if (potentionalAnswer != null)
                        return potentionalAnswer;
                }
            }

            return default;
        }

        static void Close()
        {
            if (Object.FindObjectsOfType<MapUI>().Length != 0)
                return;
            parent.GetChild(3).gameObject.SetActive(false);
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


            SR2EMain.CreateModMenuButton();

            Transform modContent = getObjRec<Transform>(transform, "ModContent");
            for (int i = 0; i < modContent.childCount; i++)
            {
                GameObject.Destroy(modContent.GetChild(i).gameObject);
            }

        }

        private static MainMenuLandingRootUI _mainMenuLandingRootUI;

        public static void Open()
        {
            if (SR2Console.isOpen)
                return;
            if (Object.FindObjectsOfType<MapUI>().Length != 0)
                return;
            parent.GetChild(3).gameObject.SetActive(true);
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

        internal static void Start()
        {
            //eventSystem = parent.GetChild(5).GetComponent<EventSystem>();
            modInfoText = getObjRec<TextMeshProUGUI>(transform, "ModInfoText");
            gameObject.SetActive(false);

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