using System.Linq;
using System.Reflection;
using Il2CppSystem.IO;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController.Abilities;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppTMPro;
using SR2E.Commands;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Il2CppInterop.Runtime.Injection;
using SR2E.Saving;
using Il2CppKinematicCharacterController;

namespace SR2E
{
    public static class BuildInfo
    {
        public const string Name = "SR2Essentials"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "Essentials for Slime Rancher 2"; // Description for the Mod.  (Set as null if none)
        public const string Author = "ThatFinn"; // Author of the Mod.  (MUST BE SET)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.4.2b2"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = "https://www.nexusmods.com/slimerancher2/mods/60"; // Download Link for the Mod.  (Set as null if none)
    }

    public class SR2EEntryPoint : MelonMod
    {
        public static SR2EEntryPoint instance;
        public static TMP_FontAsset SR2Font;
        internal static bool infEnergy = false;
        internal static bool infHealth = false;
        internal static bool infEnergyInstalled = false;
        internal static bool infHealthInstalled = false;
        internal static bool consoleFinishedCreating = false;
        bool mainMenuLoaded = false;
        private static bool _iconChanged = false;
        static Image _modsButtonIconImage;

        internal static IdentifiableType[] identifiableTypes
        { get { return GameContext.Instance.AutoSaveDirector.identifiableTypes.GetAllMembers().ToArray().Where(identifiableType => !string.IsNullOrEmpty(identifiableType.ReferenceId)).ToArray(); } }
        
        internal static IdentifiableType getIdentifiableByName(string name)
        {
            foreach (IdentifiableType type in identifiableTypes)
                if (type.name.ToUpper() == name.ToUpper())
                    return type;
            return null;
        }

        

        internal static IdentifiableType getIdentifiableByLocalizedName(string name)
        {
            foreach (IdentifiableType type in identifiableTypes)
                try
                {
                    if (type.LocalizedName.GetLocalizedString().ToUpper().Replace(" ","") == name.ToUpper())
                        return type;
                }
                catch (System.Exception ignored)
                {}
            
            return null;
        }
        static bool CheckIfLargo(string value) => (value.Remove(0, 1)).Any(char.IsUpper);
        internal static MelonPreferences_Category prefs;
        internal static float noclipAdjustSpeed
        {
            get
            {
                return prefs.GetEntry<float>("noclipAdjustSpeed").Value;
            }
        }
        static string onSaveLoadCommand
        {
            get
            {
                return prefs.GetEntry<string>("onSaveLoadCommand").Value;
            }
        }
        static string onMainMenuLoadCommand
        {
            get
            {
                return prefs.GetEntry<string>("onMainMenuLoadCommand").Value;
            }
        }
        internal static bool syncConsole
        {
            get
            {
                return prefs.GetEntry<bool>("doesConsoleSync").Value;
            }
        }
        internal static bool skipEngagementPrompt
        {
            get
            {
                return prefs.GetEntry<bool>("skipEngagementPrompt").Value;
            }
        }
        internal static bool consoleUsesSR2Font
        {
            get
            {
                return prefs.GetEntry<bool>("consoleUsesSR2Font").Value;
            }
        }
        internal static bool debugLogging
        {
            get
            {
                return prefs.GetEntry<bool>("debugLogging").Value;
            }
        }

        internal static bool devMode
        {
            get
            {
                return prefs.GetEntry<bool>("experimentalStuff").Value;
            }
        }
        internal static void RefreshPrefs()
        {
            prefs.DeleteEntry("noclipFlySpeed");
            prefs.DeleteEntry("noclipFlySprintSpeed");

            if (!prefs.HasEntry("noclipAdjustSpeed"))
                prefs.CreateEntry("noclipAdjustSpeed", (float)235f, "NoClip scroll speed", false);
            if (!prefs.HasEntry("consoleUsesSR2Font"))
                prefs.CreateEntry("consoleUsesSR2Font", (bool)false, "Console uses SR2 font", false);
            if (!prefs.HasEntry("doesConsoleSync"))
                prefs.CreateEntry("doesConsoleSync", (bool)false, "Console sync with ML log", false);
            if (!prefs.HasEntry("skipEngagementPrompt"))
                prefs.CreateEntry("skipEngagementPrompt", (bool)false, "Skip the engagement prompt", false);
            if (!prefs.HasEntry("debugLogging"))
                prefs.CreateEntry("debugLogging", (bool)false, "Log debug info", false);
            if (!prefs.HasEntry("experimentalStuff"))
                prefs.CreateEntry("experimentalStuff", (bool)false, "Enable experimental stuff", true);
            if (!prefs.HasEntry("onSaveLoadCommand"))
                prefs.CreateEntry("onSaveLoadCommand", (string)"", "Execute command when save is loaded", false);
            if (!prefs.HasEntry("onMainMenuLoadCommand"))
                prefs.CreateEntry("onMainMenuLoadCommand", (string)"", "Execute command when main menu is loaded", false);

        }
        public override void OnInitializeMelon()
        {
            instance = this;
            prefs = MelonPreferences.CreateCategory("SR2Essentials");
            ClassInjector.RegisterTypeInIl2Cpp<SR2ESlimeDataSaver>();
            ClassInjector.RegisterTypeInIl2Cpp<SR2EGordoDataSaver>();
            RefreshPrefs();
            foreach (MelonBase melonBase in MelonBase.RegisteredMelons)
                switch (melonBase.Info.Name)
                {
                    case "InfiniteEnergy":
                        infEnergyInstalled = true;
                        break;
                    case "InfiniteHealth":
                        infHealthInstalled = true;
                        break;
                }
        }

        public override void OnApplicationQuit()
        {
            if (SystemContext.Instance.SceneLoader.IsCurrentSceneGroupGameplay())
            {
                GameContext.Instance.AutoSaveDirector.SaveGame();
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            switch (sceneName)
            {
                case "SystemCore":
                    System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SR2E.srtwoessentials.assetbundle");
                    byte[] buffer = new byte[16 * 1024];
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    int read;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                        ms.Write(buffer, 0, read);

                    AssetBundle bundle = AssetBundle.LoadFromMemory(ms.ToArray());
                    foreach (var obj in bundle.LoadAllAssets())
                        if (obj != null)
                            if (obj.name == "AllMightyMenus")
                            {
                                Object.Instantiate(obj);

                                

                                if (skipEngagementPrompt)
                                {
                                    SR2EUtils.Get<GameObject>("EngagementSkipMessage").SetActive(true);
                                }
                            }
                    break;
                case "MainMenuUI":
                    if (!System.String.IsNullOrEmpty(onMainMenuLoadCommand)) 
                        SR2Console.ExecuteByString(onMainMenuLoadCommand);
                    infEnergy = false;
                    //SceneContext.Instance.PlayerState._model.maxHealth = InvincibleCommand.normalHealth;
                    infHealth = false;
                    if (skipEngagementPrompt)
                    {
                        SR2Console.transform.getObjRec<GameObject>("EngagementSkipMessage").SetActive(false);
                    }
                    break;
                case "StandaloneEngagementPrompt":
                    PlatformEngagementPrompt prompt = Object.FindObjectOfType<PlatformEngagementPrompt>();
                    Object.FindObjectOfType<CompanyLogoScene>().StartLoadingIndicator();
                    prompt.EngagementPromptTextUI.SetActive(false);
                    prompt.OnInteract(new InputAction.CallbackContext());
                    break;
                case "GameCore":
                    AutoSaveDirector autoSaveDirector = GameContext.Instance.AutoSaveDirector;
                    autoSaveDirector.saveSlotCount = 50;
                    break;
                case "UICore":
                    InfiniteEnergyCommand.energyMeter = SR2EUtils.Get<EnergyMeter>("Energy Meter");
                    if (!System.String.IsNullOrEmpty(onSaveLoadCommand)) 
                        SR2Console.ExecuteByString(onSaveLoadCommand);
                    break;
                case "PlayerCore":
                    NoclipComponent.playerSettings = SR2EUtils.Get<KCCSettings>("");
                    NoclipComponent.player = SceneContext.Instance.player.transform;
                    NoclipComponent.playerController = NoclipComponent.player.GetComponent<SRCharacterController>();
                    NoclipComponent.playerMotor = NoclipComponent.player.GetComponent<KinematicCharacterMotor>();
                    InfiniteEnergyCommand.jetpackAbilityData = SR2EUtils.Get<JetpackAbilityData>("Jetpack");
                    
                    break;
            }

        }

        internal static void SetupFonts()
        {
            SR2Font = SR2EUtils.Get<AssetBundle>("bee043ef39f15a1d9a10a5982c708714.bundle").LoadAsset("Assets/UI/Font/HemispheresCaps2/Runsell Type - HemispheresCaps2 (Latin).asset").Cast<TMP_FontAsset>();
            if (consoleUsesSR2Font)
            {
                foreach (var text in SR2EUtils.Get<GameObject>("SR2Console").getAllChildrenOfType<TMP_Text>())
                {
                    text.font = SR2Font;
                }
            }
            else
            {
                foreach (var text in SR2EUtils.Get<GameObject>("modMenu").getAllChildrenOfType<TMP_Text>())
                {
                    text.font = SR2Font;
                }
                foreach (var text in SR2EUtils.Get<GameObject>("EngagementSkipMessage").getAllChildrenOfType<TMP_Text>())
                {
                    text.font = SR2Font;
                }
            }
        }

        public override void OnSceneWasInitialized(int buildindex, string sceneName)
        {
            switch (sceneName)
            {
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
                case "MainMenuUI":
                    mainMenuLoaded = false;
                    break;
                case "LoadScene":
                    SR2Warps.OnSceneLoaded(sceneName);
                    break;
            }
        }


        static SRCharacterController Player;
        static SRCameraController camera;
        private static bool saveCountChange = false;

        public override void OnUpdate()
        {
            if (mainMenuLoaded)
            {
                if (!saveCountChange)
                {
                    SaveGamesRootUI ui = GameObject.FindObjectOfType<SaveGamesRootUI>();
                    if (ui != null)
                    {
                        GameObject scrollView = GameObject.Find("ButtonsScrollView");
                        if (scrollView != null)
                        {
                            ScrollRect rect = scrollView.GetComponent<ScrollRect>();
                            rect.vertical = true;
                            rect.verticalScrollbar = GameObject.Instantiate(SR2EUtils.getObjRec<Scrollbar>(SR2Console.transform, "saveFilesSlider"), rect.transform);
                            rect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
                            saveCountChange = true;
                        }
                    }
                    
                }
                if (!_iconChanged)
                {
                    Sprite sprite = SR2EUtils.getObjRec<Image>(SR2Console.transform, "modsButtonIconImage").sprite;
                    if (sprite != null)
                        if (_modsButtonIconImage != null)
                        {
                            _modsButtonIconImage.sprite = sprite;
                            _iconChanged = true;
                        }
                }
            }

            if (infEnergy)
                if (SceneContext.Instance != null)
                    if (SceneContext.Instance.PlayerState != null)
                        SceneContext.Instance.PlayerState.SetEnergy(int.MaxValue);
            if(infHealth)
                if (SceneContext.Instance != null)
                    if (SceneContext.Instance.PlayerState != null)
                        SceneContext.Instance.PlayerState.SetHealth(int.MaxValue);
            if (SceneContext.Instance != null)
                if (SceneContext.Instance.Player != null)
                    if (!SR2Console.isOpen)
                        if (!SR2ModMenu.isOpen)
                            if (Time.timeScale != 0)
                            {
                                if (Player == null)
                                    Player = SR2EUtils.Get<SRCharacterController>("PlayerControllerKCC");

                                if (camera == null)
                                    camera = SR2EUtils.Get<SRCameraController>("PlayerCameraKCC");
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


        internal static void CreateModMenuButton()
        {
            MainMenuLandingRootUI rootUI = Object.FindObjectOfType<MainMenuLandingRootUI>();
            Transform buttonHolder = rootUI.transform.GetChild(0).GetChild(3);
            for (int i = 0; i < buttonHolder.childCount; i++)
                if (buttonHolder.GetChild(i).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text == "Mods")
                    Object.Destroy(buttonHolder.GetChild(i).gameObject);

            //Create Button
            GameObject newButton = Object.Instantiate(buttonHolder.GetChild(0).gameObject, buttonHolder);
            newButton.transform.SetSiblingIndex(buttonHolder.childCount==4?2:3);
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
            button.onClick.AddListener((System.Action)(() => { SR2ModMenu.Open(); }));
        }
    }
}