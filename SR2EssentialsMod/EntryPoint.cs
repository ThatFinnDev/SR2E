using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Il2CppSystem.IO;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppTMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Il2CppKinematicCharacterController;
using MelonLoader.Utils;
using Il2CppMonomiPark.SlimeRancher.Damage;
using Il2CppMonomiPark.SlimeRancher.Options;
using Il2CppMonomiPark.SlimeRancher.Player.FirstPersonScreenEffects;
using Il2CppMonomiPark.SlimeRancher.World.Teleportation;
using UnityEngine.Localization;
using SR2E.Buttons;
using SR2E.Commands;
using SR2E.Patches.General;
using UnityEngine.Networking;

namespace SR2E
{
    public static class BuildInfo
    {
        public const string Name = "SR2E"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "Essentials for Slime Rancher 2"; // Description for the Mod.  (Set as null if none)
        public const string Author = "ThatFinn"; // Author of the Mod.  (MUST BE SET)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "2.4.8"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = "https://www.nexusmods.com/slimerancher2/mods/60"; // Download Link for the Mod.  (Set as null if none)
    }

    public class SR2EEntryPoint : MelonMod
    {
        internal static bool _mSRMLIsInstalled = false;
        internal static SR2EEntryPoint instance;
        internal static TMP_FontAsset SR2Font;
        internal static TMP_FontAsset normalFont;
        internal static bool infEnergyInstalled = false;
        internal static bool infHealthInstalled = false;
        internal static bool consoleFinishedCreating = false;
        internal static bool mainMenuLoaded = false;

        static AssetBundle bundle;


        internal static MelonPreferences_Category prefs;
        internal static float noclipAdjustSpeed { get { return prefs.GetEntry<float>("noclipAdjustSpeed").Value; } }
        internal static string onSaveLoadCommand { get { return prefs.GetEntry<string>("onSaveLoadCommand").Value; } }
        internal static string onMainMenuLoadCommand { get { return prefs.GetEntry<string>("onMainMenuLoadCommand").Value; } }
        internal static bool syncConsole { get { return prefs.GetEntry<bool>("doesConsoleSync").Value; } }
        internal static bool consoleUsesSR2Font { get { return prefs.GetEntry<bool>("consoleUsesSR2Font").Value; } }
        internal static bool debugLogging { get { return prefs.GetEntry<bool>("debugLogging").Value; } }
        internal static bool devMode { get { return prefs.GetEntry<bool>("devMode").Value; } }
        internal static bool fixSaves { get { return prefs.GetEntry<bool>("fixSaves").Value; } }
        internal static bool showUnityErrors { get { return prefs.GetEntry<bool>("showUnityErrors").Value; } }
        internal static float noclipSpeedMultiplier { get { return prefs.GetEntry<float>("noclipSprintMultiply").Value; } }
        internal static bool enableDebugDirector { get { return prefs.GetEntry<bool>("enableDebugDirector").Value; } }
        internal static bool enableCheatMenuButton { get { return prefs.GetEntry<bool>("enableCheatMenuButton").Value; } }

        internal static void RefreshPrefs()
        {
            prefs.DeleteEntry("noclipFlySpeed");
            prefs.DeleteEntry("noclipFlySprintSpeed");
            prefs.DeleteEntry("experimentalStuff");
            prefs.DeleteEntry("skipEngagementPrompt");

            if (!prefs.HasEntry("noclipAdjustSpeed"))
                prefs.CreateEntry("noclipAdjustSpeed", (float)235f, "NoClip scroll speed", false).disableWarning();
            if (!prefs.HasEntry("consoleUsesSR2Font"))
                prefs.CreateEntry("consoleUsesSR2Font", (bool)false, "Console uses SR2 font", false).disableWarning((System.Action)(
                    () =>
                    {
                        SetupFonts(); 
                    }));

            if (!prefs.HasEntry("showUnityErrors"))
                prefs.CreateEntry("showUnityErrors", (bool)false, "Shows unity errors in the console", true);
            
            if (!prefs.HasEntry("enableDebugDirector"))
                prefs.CreateEntry("enableDebugDirector", (bool)false, "Enable debug menu", false).disableWarning((System.Action)(
                    () => { SR2EDebugDirector.isEnabled = enableDebugDirector; }));
            
            if (!prefs.HasEntry("enableCheatMenuButton"))
                prefs.CreateEntry("enableCheatMenuButton", (bool)false, "Enable cheat menu button in pause menu", false).disableWarning((System.Action)(
                    () =>
                    {
                        if (!enableCheatMenuButton)
                            cheatMenuButton.Remove();
                        if (enableCheatMenuButton)
                            cheatMenuButton.AddAgain();
                    }));

            if (!prefs.HasEntry("doesConsoleSync"))
                prefs.CreateEntry("doesConsoleSync", (bool)false, "Console sync with ML log", false);
            if (!prefs.HasEntry("debugLogging"))
                prefs.CreateEntry("debugLogging", (bool)false, "Log debug info", false).disableWarning();
            if (!prefs.HasEntry("devMode"))
                prefs.CreateEntry("devMode", (bool)false, "Enable dev mode", true);
            if (!prefs.HasEntry("fixSaves"))
                prefs.CreateEntry("fixSaves", (bool)false, "Fix saves that broken through mods/updates","This is EXPERIMENTAL, it may break stuff or not work. Disable after usage!", false).disableWarning();
            if (!prefs.HasEntry("onSaveLoadCommand"))
                prefs.CreateEntry("onSaveLoadCommand", (string)"", "Execute command when save is loaded", false).disableWarning();
            if (!prefs.HasEntry("onMainMenuLoadCommand"))
                prefs.CreateEntry("onMainMenuLoadCommand", (string)"", "Execute command when main menu is loaded", false).disableWarning();
            if (!prefs.HasEntry("noclipSprintMultiply"))
                prefs.CreateEntry("noclipSprintMultiply", 2f, "NoClip sprint speed multiplier", false).disableWarning();
        }

        
        public override void OnLateInitializeMelon()
        {
            if (Get<GameObject>("SR2EPrefabHolder")) { rootOBJ = Get<GameObject>("SR2EPrefabHolder"); }
            else
            {
                rootOBJ = new GameObject();
                rootOBJ.SetActive(false);
                rootOBJ.name = "SR2EPrefabHolder";
                Object.DontDestroyOnLoad(rootOBJ);
            }

            MelonCoroutines.Start(CheckForNewVersion());
        }
        public static string MLVERSION = MelonLoader.BuildInfo.Version;
        public static string newVersion = null;
        IEnumerator CheckForNewVersion()
        {
            UnityWebRequest uwr = UnityWebRequest.Get("https://raw.githubusercontent.com/ThatFinnDev/SR2E/main/latestver.txt");
            yield return uwr.SendWebRequest();

            if (!uwr.isNetworkError)
                newVersion = uwr.downloadHandler.text.Replace(" ","").Replace("\n","");
            
        }

        
        //Logging code from Atmudia
        private static void AppLogUnity(string message, string trace, LogType type)
        {
            if (message.Equals(string.Empty))
                return;

            string toDisplay = message;
            if (!trace.Equals(string.Empty))
                toDisplay += "\n" + trace;
            toDisplay = Regex.Replace(toDisplay, @"\[INFO]\s|\[ERROR]\s|\[WARNING]\s", "");

            switch (type)
            {
                case LogType.Assert:
                    MelonLogger.Error("[Unity] "+toDisplay);
                    break;
                case LogType.Exception:
                    MelonLogger.Error("[Unity] "+toDisplay+trace);
                    break;
                case LogType.Log:
                    MelonLogger.Msg("[Unity] "+toDisplay);
                    break;
                case LogType.Error:
                    MelonLogger.Error("[Unity] "+toDisplay+trace);
                    break;
                case LogType.Warning:
                    MelonLogger.Warning("[Unity] "+toDisplay);
                    break;
            }
        }
        public override void OnInitializeMelon()
        {
            instance = this;
            prefs = MelonPreferences.CreateCategory("SR2E","SR2E");
            RefreshPrefs();
            if (showUnityErrors)
                Application.add_logMessageReceived(new Action<string, string, LogType>(AppLogUnity));
            try
            {
                SR2ELanguageManger.LoadLanguage(); }
            catch (Exception e) { MelonLogger.Error(e); }
            foreach (MelonBase melonBase in MelonBase.RegisteredMelons)
                switch (melonBase.Info.Name)
                {
                    case "InfiniteEnergy":
                        infEnergyInstalled = true;
                        break;
                    case "InfiniteHealth":
                        infHealthInstalled = true;
                        break;
                    case "mSRML":
                        _mSRMLIsInstalled = true;
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

        internal static Damage killDamage;
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

                    bundle = AssetBundle.LoadFromMemory(ms.ToArray());
                    foreach (var obj in bundle.LoadAllAssets())
                    {
                        if (obj != null)
                            if(obj.name=="AllMightyMenus")
                            {
                                Object.Instantiate(obj);
                                break;
                            }
                    }
                    
                    break;
                case "MainMenuUI":
                    
                    var optionCategories = Resources.FindObjectsOfTypeAll<OptionsItemCategory>();

                    foreach (var category in optionCategories)
                    {
                        switch (category.name)
                        {
                            case "GameSettings":
                                CustomSettingsButton.ApplyButtons(CustomSettingsButton.SettingsCategory.GameSettings, category);
                                break;
                            case "Display":
                                CustomSettingsButton.ApplyButtons(CustomSettingsButton.SettingsCategory.Display, category);
                                break;
                            case "Audio":
                                CustomSettingsButton.ApplyButtons(CustomSettingsButton.SettingsCategory.Audio, category);
                                break;
                            case "BindingsGamepad":
                                CustomSettingsButton.ApplyButtons(CustomSettingsButton.SettingsCategory.Bindings_Controller, category);
                                break;
                            case "Input":
                                CustomSettingsButton.ApplyButtons(CustomSettingsButton.SettingsCategory.Input, category);
                                break;
                            case "Gameplay_MainMenu":
                                CustomSettingsButton.ApplyButtons(CustomSettingsButton.SettingsCategory.Gameplay_MainMenu, category);
                                break;
                            case "BindingsKbm":
                                CustomSettingsButton.ApplyButtons(CustomSettingsButton.SettingsCategory.Bindings_Keyboard, category);
                                break;
                            case "Video":
                                CustomSettingsButton.ApplyButtons(CustomSettingsButton.SettingsCategory.Graphics, category);
                                break;
                            default:
                                // There are 2 other categories, but they are console only. 
                                // Also, the Gameplay_InGame is loaded somewhere after GameCore.
                                break;
                        }
                        MelonLogger.BigError("SR2E TODO","PLEASE IMPLEMENT THE GAMEPLAY_INGAME SETTINGS CATEGORY");
                    }
                    
                    Time.timeScale = 1f;
                    try
                    {
                        actionMaps = new Dictionary<string, InputActionMap>();
                        MainGameActions = new Dictionary<string, InputAction>();
                        PausedActions = new Dictionary<string, InputAction>();
                        DebugActions = new Dictionary<string, InputAction>();
                        foreach (InputActionMap map in GameContext.Instance.InputDirector._inputActions.actionMaps)
                            actionMaps.Add(map.name, map);
                        foreach (InputAction action in actionMaps["MainGame"].actions)
                            MainGameActions.Add(action.name,action); 
                        foreach (InputAction action in actionMaps["Paused"].actions)
                            PausedActions.Add(action.name,action); 
                        foreach (InputAction action in actionMaps["Debug"].actions)
                            DebugActions.Add(action.name,action); 
                    }
                    catch (Exception e)
                    {
                        MelonLogger.Error(e);
                        MelonLogger.Error("There was a problem loading SR2 action maps!");
                    }
                    
                    break;
                case "StandaloneEngagementPrompt":
                    Object.FindObjectOfType<CompanyLogoScene>().StartLoadingIndicator();
                    break;
                case "GameCore":
                    killDamage = new Damage { Amount = 99999999, DamageSource = ScriptableObject.CreateInstance<DamageSourceDefinition>(), };
                    killDamage.DamageSource.hideFlags |= HideFlags.HideAndDontSave;
                    AutoSaveDirector autoSaveDirector = GameContext.Instance.AutoSaveDirector;
                    autoSaveDirector.saveSlotCount = 75;
                    
                    foreach (ParticleSystemRenderer particle in Resources.FindObjectsOfTypeAll<ParticleSystemRenderer>())
                    {
                        var pname = particle.gameObject.name.Replace(' ', '_');
                        if (!FXLibrary.ContainsKey(particle.gameObject))
                            FXLibrary.AddItems(particle.gameObject, particle, pname);
                        if (!FXLibraryReversable.ContainsKey(pname))
                            FXLibraryReversable.AddItems(pname, particle, particle.gameObject);
                    }

                    vaccableGroup = Get<IdentifiableTypeGroup>("VaccableNonLiquids");
                    
                    
                    foreach (KeyValuePair<string, string> pair in teleportersToAdd)
                        AddTeleporter(pair.Key, pair.Value);
                    
                    
                    break;
                case "UICore":
                    if(SceneContext.Instance.Player.GetComponent<SR2EDebugDirector>()==null)
                        SceneContext.Instance.Player.AddComponent<SR2EDebugDirector>();
                    break;
                case "PlayerCore":
                    NoClipComponent.playerSettings = Get<KCCSettings>("");
                    NoClipComponent.player = SceneContext.Instance.player.transform;
                    NoClipComponent.playerController = NoClipComponent.player.GetComponent<SRCharacterController>();
                    NoClipComponent.playerMotor = NoClipComponent.player.GetComponent<KinematicCharacterMotor>();
                    player = Get<GameObject>("PlayerControllerKCC");
                    break;
            }
            SR2EConsole.OnSceneWasLoaded(buildIndex, sceneName);
        }

        static Dictionary<string,string> teleportersToAdd = new Dictionary<string, string>()
        {
            {"SceneGroup.ConservatoryFields", "TeleporterHomeBlue"},
            {"SceneGroup.RumblingGorge", "TeleporterZoneGorge"},
            {"SceneGroup.LuminousStrand", "TeleporterZoneStrand"},
            {"SceneGroup.PowderfallBluffs", "TeleporterZoneBluffs"},
            {"SceneGroup.Labyrinth", "TeleporterZoneLabyrinth"},
        };
        static void AddTeleporter(string sceneGroup, string gadgetName)
        {
            
            StaticTeleporterNode teleporter = GameObject.Instantiate(getGadgetDefByName(gadgetName).prefab.transform.getObjRec<GadgetTeleporterNode>("Teleport Collider").gameObject.GetComponent<StaticTeleporterNode>());
            teleporter.gameObject.SetActive(false); teleporter.name = "TP-"+sceneGroup; teleporter.gameObject.MakePrefab(); teleporter.gameObject.MakePrefab(); teleporter._hasDestination = true;
            SR2ESaveManager.WarpManager.teleporters.TryAdd(sceneGroup, teleporter);

        }

        public static event EventHandler RegisterOptionMenuButtons;  
        
        internal static void SetupFonts()
        {
            try
            {
                if(normalFont==null) normalFont = Get<TMP_FontAsset>("Lexend-Regular (Latin)");
            }
            catch
            {
                MelonLogger.Error("The normal font couldn't be loaded!");
                MelonLogger.Error("This happens on some platforms and I (the dev) haven't found a fix yet!");
            }
            if (consoleUsesSR2Font)
                foreach (var text in SR2EConsole.parent.getAllChildrenOfType<TMP_Text>())
                    text.font = SR2Font;
            else if(normalFont!=null)
                foreach (var text in SR2EConsole.parent.getAllChildrenOfType<TMP_Text>())
                    text.font = normalFont==null?SR2Font:normalFont;
            foreach (var text in SR2EConsole.parent.getObjRec<GameObject>("modMenu").getAllChildrenOfType<TMP_Text>())
                text.font = SR2Font;
            foreach (var text in SR2EConsole.parent.getObjRec<GameObject>("cheatMenu").getAllChildrenOfType<TMP_Text>())
                text.font = SR2Font;

        }

        internal static void OnSaveDirectorLoading(AutoSaveDirector autoSaveDirector)
        {
            
        }

        internal static CustomPauseMenuButton cheatMenuButton;
        static bool isSaveDirectorLoaded = false;
        internal static void SaveDirectorLoaded()
        {
            if (isSaveDirectorLoaded) return;
            isSaveDirectorLoaded = true;
            
            
            var scriptedValue = CustomSettingsButton.CreateScriptedInt(0);

            RegisterOptionMenuButtons += (_, _) =>
            {
                CustomSettingsButton.Create(
                    CustomSettingsButton.SettingsCategory.Gameplay_MainMenu,
                    AddTranslationFromSR2E("setting.gamesettingtest", "b.testsetting", "UI"),
                    AddTranslationFromSR2E("setting.gamesettingtest.desc", "l.testsettingdescription", "UI"),
                    0,
                    "testButton1",
                    true,
                    false,

                    new CustomSettingsButton.OptionValue("val1", AddTranslationFromSR2E("setting.gamesettingtest.value1", "l.testsettingvalue1", "UI"), scriptedValue),
                    new CustomSettingsButton.OptionValue("val2", AddTranslationFromSR2E("setting.gamesettingtest.value2", "l.testsettingvalue2", "UI"), scriptedValue),
                    new CustomSettingsButton.OptionValue("val3", AddTranslationFromSR2E("setting.gamesettingtest.value3", "l.testsettingvalue3", "UI"), scriptedValue)
                );
            };
            
            LocalizedString label = AddTranslationFromSR2E("buttons.mods.label", "b.button_mods_sr2e", "UI");
            new CustomMainMenuButton(label, LoadSprite("modsMenuIcon"), 2, (System.Action)(() => { SR2EModMenu.Open(); }));
            new CustomPauseMenuButton(label, 3, (System.Action)(() => { SR2EModMenu.Open(); }));
            cheatMenuButton = new CustomPauseMenuButton(AddTranslationFromSR2E("buttons.cheatmenu.label", "b.button_cheatmenu_sr2e", "UI"), 4, (System.Action)(() => { SR2ECheatMenu.Open(); }));
            if(!enableCheatMenuButton) cheatMenuButton.Remove();
            
            if (devMode) new CustomPauseMenuButton( AddTranslationFromSR2E("buttons.debugplayer.label", "b.debug_player_sr2e", "UI"), 3, (System.Action)(() => { SR2EDebugDirector.DebugStatsManager.TogglePlayerDebugUI();}));

            RegisterOptionMenuButtons?.Invoke(SR2EEntryPoint.instance, EventArgs.Empty);
            
        }
        public override void OnSceneWasInitialized(int buildindex, string sceneName) { if(sceneName=="MainMenuUI") mainMenuLoaded = true; }
        public override void OnSceneWasUnloaded(int buildIndex, string sceneName) { if(sceneName=="MainMenuUI") mainMenuLoaded = false; SR2ESaveManager.WarpManager.OnSceneLoaded(); }
        internal static List<BaseUI> baseUIAddSliders = new List<BaseUI>();

        public override void OnUpdate()
        {
            if (mainMenuLoaded)
            {
                foreach (BaseUI ui in new List<BaseUI>(baseUIAddSliders))
                {
                    if (ui)
                    {
                        GameObject scrollView = GameObject.Find("ButtonsScrollView");
                        if (scrollView != null)
                        {
                            ScrollRect rect = scrollView.GetComponent<ScrollRect>();
                            rect.vertical = true;
                            Scrollbar scrollBar = GameObject.Instantiate(SR2EConsole.parent.getObjRec<Scrollbar>("saveFilesSliderRec"), rect.transform);
                            rect.verticalScrollbar = scrollBar;
                            rect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
                            scrollBar.GetComponent<RectTransform>().localPosition += new Vector3(Screen.width / 250f, 0, 0);
                        }
                    }
                    baseUIAddSliders.Remove(ui);
                }
            }

            if (!consoleFinishedCreating)
            {
                GameObject obj = GameObject.FindGameObjectWithTag("Respawn");
                if (obj != null)
                {
                    consoleFinishedCreating = true;
                    obj.name = "SR2EStuff";
                    GameObject.DontDestroyOnLoad(obj);
                    
                    SR2EConsole.parent = obj.transform;
                    SR2EConsole.Start();
                }
            }
            else
            {
                try { SR2EConsole.Update(); } catch (Exception e) { MelonLogger.Error(e); }
                try { SR2ESaveManager.Update(); } catch (Exception e) { MelonLogger.Error(e); }
                try { SR2EModMenu.Update(); } catch (Exception e) { MelonLogger.Error(e); }
                try { SR2ECheatMenu.Update(); } catch (Exception e) { MelonLogger.Error(e); }
                if(devMode) SR2EDebugDirector.DebugStatsManager.Update();
            }
        }
        
    }
}