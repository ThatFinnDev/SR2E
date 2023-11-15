using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Input;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController.Abilities;
using Il2CppMonomiPark.SlimeRancher.Ranch;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppMonomiPark.UnitPropertySystem;
using Il2CppSystem.Collections.Generic;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EventTrigger = UnityEngine.EventSystems.EventTrigger;
using Object = UnityEngine.Object;


namespace SR2E
{
    public static class BuildInfo
    {
        public const string Name = "SR2Essentials"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "Essentials for Slime Rancher 2"; // Description for the Mod.  (Set as null if none)
        public const string Author = "ThatFinn"; // Author of the Mod.  (MUST BE SET)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.0.0"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class SR2EMain : MelonMod
    {
        public static bool infEnergy = false;
        static List<IdentifiableType> vaccables = new List<IdentifiableType>();
        
        static T Get<T>(string name) where T : UnityEngine.Object
        {
            return Resources.FindObjectsOfTypeAll<T>().FirstOrDefault((T x) => x.name == name);
        }

        
        public static IdentifiableType getVaccableByName(string name)
        {
            foreach (IdentifiableType type in vaccables)
            {
                if (type.name.ToUpper() == name.ToUpper())
                {
                    return type;
                }
            }

            return null;
        }
        bool moreVaccabalesInstalled = false;
        public override void OnInitializeMelon()
        {
            
            foreach (MelonBase melonBase in MelonBase.RegisteredMelons)
            {
                if (melonBase.Info.Name == "MoreVaccablesMod")
                {
                    moreVaccabalesInstalled = true;
                    break;
                }
            }
        }


        internal static Sprite modsMenuIcon;
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName == "SystemCore")
            {
                        
                Assembly asm = Assembly.GetExecutingAssembly();
                Stream stream = asm.GetManifestResourceStream("TestMod.srtwoessentials.assetbundle");
                
                byte[] buffer = new byte[16*1024];
                MemoryStream ms = new MemoryStream();
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                AssetBundle bundle = AssetBundle.LoadFromMemory(ms.ToArray());
                foreach (var obj in bundle.LoadAllAssets())
                {
                    if (obj != null)
                    {
                        switch (obj.name)
                        {
                            case "AllMightyMenus":
                                GameObject instance = GameObject.Instantiate(obj) as GameObject;
                                break;
                        }
                    }
                }

            }
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            if (sceneName == "GameCore")
            {
                vaccables = null;
            }
            
            if (sceneName == "MainMenuUI")
                mainMenuLoaded = false;
        

        }

        public static bool CheckIfLargo(string Value)
        {
            return (Value.Remove(0, 1)).Any(char.IsUpper);
        }

        public static bool consoleFinishedCreating = false;
        bool mainMenuLoaded = false;
  
        public override void OnUpdate()
        {
            if (mainMenuLoaded)
            {
                if (!iconChanged)
                {
                    Sprite sprite =
                        SR2Console.transform.GetChild(4).GetChild(3).GetComponent<Image>().sprite;
                    if (sprite == null)
                    {
                        MelonLogger.Msg((SR2Console.transform.GetChild(4).GetChild(3).GetComponent<Image>().sprite)==null);
                    }
                    else
                    {
                        modsMenuIcon = sprite;
                    }


                    if (modsMenuIcon != null && modsButtonIconImage != null)
                    {
                        modsButtonIconImage.sprite = modsMenuIcon;
                        iconChanged = true;
                    }
                }
            }
            if (infEnergy)
            {
                if(SceneContext.Instance != null)
                {
                    if (SceneContext.Instance.PlayerState != null)
                    {
                        SceneContext.Instance.PlayerState.SetEnergy(int.MaxValue);
                    }
                }
            }
            if (!consoleFinishedCreating)
            {
                GameObject obj = GameObject.FindGameObjectWithTag("Respawn");
                if (obj != null)
                {
                    consoleFinishedCreating = true;
                    SR2Console.gameObject = obj;
                    obj.name = "SR2Console";
                    GameObject.DontDestroyOnLoad(obj);
                    SR2Console.transform = obj.transform;
                    SR2Console.Start();
                }
            }
            SR2Console.Update();
        }

        private static bool iconChanged = false;
        internal static Image modsButtonIconImage;
        internal static void CreateModMenuButton()
        {
            MainMenuLandingRootUI rootUI = Object.FindObjectOfType<MainMenuLandingRootUI>();
            Transform buttonHolder = rootUI.transform.GetChild(0).GetChild(3);
            for (int i = 0; i < buttonHolder.childCount; i++)
            {
                if (buttonHolder.GetChild(i).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text == "Mods")
                    Object.Destroy(buttonHolder.GetChild(i).gameObject);
            }

            //Create Button
            GameObject newButton = Object.Instantiate(buttonHolder.GetChild(0).gameObject, buttonHolder);
            newButton.transform.SetSiblingIndex(3);
            newButton.name = "ModsButton";
            newButton.transform.GetChild(0).GetComponent<CanvasGroup>().enabled = false;
            newButton.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Mods";
            modsButtonIconImage = newButton.transform.GetChild(0).GetChild(2).GetComponent<Image>();

            iconChanged = false;
            
            Object.Destroy(newButton.transform.GetChild(0).GetChild(1).GetComponent<LocalizedVersionText>());
            
            //Fix selected SpaceBar Icon Dupe
            for (int i = 0; i < newButton.transform.GetChild(0).GetChild(0).GetChild(0).childCount; i++)
                newButton.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(false);
            newButton.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);

            Button button = newButton.GetComponent<Button>();
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener((Action)(() => {SR2ModMenu.Open();}));
        }
        public override void OnSceneWasInitialized(int buildindex, string sceneName)
        {
            if (sceneName == "GameCore")
            {
                Il2CppArrayBase<IdentifiableType> allTypes = Resources.FindObjectsOfTypeAll<IdentifiableType>();

                foreach (IdentifiableType type in allTypes)
                {
                    string r = type.ReferenceId;
                    if((r.StartsWith("SlimeDefinition.") || r.StartsWith("IdentifiableType."))&&!r.EndsWith("Gordo"))
                    {
                        if (r != "IdentifiableType.None" && r != "IdentifiableType.Player")
                        {
                            if(r.StartsWith("SlimeDefinition."))
                            {
                                if (moreVaccabalesInstalled||!(CheckIfLargo(r.Remove(0,16))))
                                {
                                    vaccables.Add(type);
                                }
                            }
                            else
                            {
                                vaccables.Add(type);
                            }
                        }
                    }
                }
            }
            
            if (sceneName == "MainMenuUI")
            {
                mainMenuLoaded = true;
                CreateModMenuButton();
            }

        }
    }
}