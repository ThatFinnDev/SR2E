using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController.Abilities;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppSystem.Collections.Generic;
using Il2CppTMPro;
using MelonLoader;
using SR2E.Commands;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;


namespace SR2E
{
    public static class BuildInfo
    {
        public const string Name = "SR2Essentials"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "Essentials for Slime Rancher 2"; // Description for the Mod.  (Set as null if none)
        public const string Author = "ThatFinn"; // Author of the Mod.  (MUST BE SET)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.3.0"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = "https://www.nexusmods.com/slimerancher2/mods/60"; // Download Link for the Mod.  (Set as null if none)
    }

    public class SR2EMain : MelonMod
    {
        internal static bool infEnergy = false;
        bool moreVaccabalesInstalled = false;
        internal static bool consoleFinishedCreating = false;
        bool mainMenuLoaded = false;
        private static bool _iconChanged = false;
        static Image _modsButtonIconImage;
        static List<IdentifiableType> vaccables = new List<IdentifiableType>();
        static T Get<T>(string name) where T : UnityEngine.Object => Resources.FindObjectsOfTypeAll<T>().FirstOrDefault((T x) => x.name == name); 
       
        internal static IdentifiableType getVaccableByName(string name)
        {
            foreach (IdentifiableType type in vaccables)
                if (type.name.ToUpper() == name.ToUpper())
                    return type;
            return null;
        }
        internal static IdentifiableType getVaccableByLocalizedName(string name)
        {
            foreach (IdentifiableType type in vaccables)
                try
                {
                    if (type.LocalizedName.GetLocalizedString().ToUpper().Replace(" ","") == name.ToUpper())
                        return type;
                }
                catch (Exception ignored)
                {}
            
            return null;
        }
        static bool CheckIfLargo(string value) => (value.Remove(0, 1)).Any(char.IsUpper);
        public override void OnInitializeMelon()
        {
            foreach (MelonBase melonBase in MelonBase.RegisteredMelons)
                if (melonBase.Info.Name == "MoreVaccablesMod")
                {
                    moreVaccabalesInstalled = true;
                    break;
                }
        }
        
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            switch (sceneName)
            {
                case "SystemCore":
                    Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TestMod.srtwoessentials.assetbundle");
                    byte[] buffer = new byte[16 * 1024];
                    MemoryStream ms = new MemoryStream();
                    int read;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                        ms.Write(buffer, 0, read);

                    AssetBundle bundle = AssetBundle.LoadFromMemory(ms.ToArray());
                    foreach (var obj in bundle.LoadAllAssets())
                        if (obj != null)
                            if(obj.name=="AllMightyMenus")
                                GameObject.Instantiate(obj);
                    break;
                case "MainMenuUI":
                    infEnergy = false;
                    break;
                case "UICore":
                    InfiniteEnergyCommand.energyMeter = Get<EnergyMeter>("Energy Meter");
                    break;
                case "PlayerCore":
                     InfiniteEnergyCommand.jetpackAbilityData = Get<JetpackAbilityData>("Jetpack");
                    break;
            }
        }
        public override void OnSceneWasInitialized(int buildindex, string sceneName)
        {
            switch (sceneName)
            {
                case "GameCore":
                    Il2CppArrayBase<IdentifiableType> allTypes = Resources.FindObjectsOfTypeAll<IdentifiableType>();

                    foreach (IdentifiableType type in allTypes)
                    {
                        string r = type.ReferenceId;
                        if ((r.StartsWith("SlimeDefinition.") || r.StartsWith("IdentifiableType.")) && !r.EndsWith("Gordo"))
                            if (r != "IdentifiableType.None" && r != "IdentifiableType.Player")
                                if (r.StartsWith("SlimeDefinition."))
                                {
                                    if (moreVaccabalesInstalled || !(CheckIfLargo(r.Remove(0, 16))))
                                        vaccables.Add(type);
                                }
                                else
                                    vaccables.Add(type);
                    }
                    break;
                case "MainMenuUI":
                    mainMenuLoaded = true;
                    CreateModMenuButton();
                    break;
                    
            }

        }
        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            switch (sceneName)
            {
                case "GameCore":
                    vaccables = null;
                    break;
                case "MainMenuUI":
                    mainMenuLoaded = false;
                    break;
            }
        }



        public override void OnUpdate()
        {
            if (mainMenuLoaded)
                if (!_iconChanged)
                {
                    Sprite sprite = SR2Console.transform.GetChild(4).GetChild(5).GetComponent<Image>().sprite;
                    if (sprite != null)
                        if (_modsButtonIconImage != null)
                        {
                            _modsButtonIconImage.sprite = sprite;
                            _iconChanged = true;
                        }
                }

            if (infEnergy)
                if (SceneContext.Instance != null)
                    if (SceneContext.Instance.PlayerState != null)
                        SceneContext.Instance.PlayerState.SetEnergy(int.MaxValue);

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


        internal static void CreateModMenuButton()
        {
            MainMenuLandingRootUI rootUI = Object.FindObjectOfType<MainMenuLandingRootUI>();
            Transform buttonHolder = rootUI.transform.GetChild(0).GetChild(3);
            for (int i = 0; i < buttonHolder.childCount; i++)
                if (buttonHolder.GetChild(i).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text == "Mods")
                    Object.Destroy(buttonHolder.GetChild(i).gameObject);

            //Create Button
            GameObject newButton = Object.Instantiate(buttonHolder.GetChild(0).gameObject, buttonHolder);
            newButton.transform.SetSiblingIndex(3);
            newButton.name = "ModsButton";
            newButton.transform.GetChild(0).GetComponent<CanvasGroup>().enabled = false;
            newButton.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Mods";
            Object.Destroy(newButton.transform.GetChild(0).GetChild(1).GetComponent<UnityEngine.Localization.Components.LocalizeStringEvent>());
            Object.Destroy(newButton.transform.GetChild(0).GetChild(1).GetComponent<Il2CppMonomiPark.SlimeRancher.UI.Framework.Layout.GameObjectWatcher>());
            _modsButtonIconImage = newButton.transform.GetChild(0).GetChild(2).GetComponent<Image>();

            _iconChanged = false;

            Object.Destroy(newButton.transform.GetChild(0).GetChild(1).GetComponent<LocalizedVersionText>());

            //Fix Selected Button SpaceBar Icon Dupe
            for (int i = 0; i < newButton.transform.GetChild(0).GetChild(0).GetChild(0).childCount; i++)
                newButton.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(false);
            newButton.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);

            Button button = newButton.GetComponent<Button>();
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener((Action)(() => { SR2ModMenu.Open(); }));
        }
    }
}