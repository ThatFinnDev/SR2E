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
using UnityEngine.Localization;
using SR2E.Buttons;
using UnityEngine.Networking;

namespace SR2E
{
    public static class BuildInfo
    {
        public const string Name = "SR2E"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "Essentials for Slime Rancher 2"; // Description for the Mod.  (Set as null if none)
        public const string Author = "ThatFinn"; // Author of the Mod.  (MUST BE SET)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "2.4.6"; // Version of the Mod.  (MUST BE SET)
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
        internal static bool showUnityErrors { get { return prefs.GetEntry<bool>("showUnityErrors").Value; } }
        internal static float noclipSpeedMultiplier { get { return prefs.GetEntry<float>("noclipSprintMultiply").Value; } }
        internal static bool enableDebugDirector { get { return prefs.GetEntry<bool>("enableDebugDirector").Value; } }

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
                prefs.CreateEntry("enableDebugDirector", (bool)false, "Enable Debug Menu", false).disableWarning((System.Action)(
                    () => { SR2EDebugDirector.isEnabled = enableDebugDirector; }));

            if (!prefs.HasEntry("doesConsoleSync"))
                prefs.CreateEntry("doesConsoleSync", (bool)false, "Console sync with ML log", false);
            if (!prefs.HasEntry("debugLogging"))
                prefs.CreateEntry("debugLogging", (bool)false, "Log debug info", false).disableWarning();
            if (!prefs.HasEntry("devMode"))
                prefs.CreateEntry("devMode", (bool)false, "Enable Dev Mode", true);
            if (!prefs.HasEntry("onSaveLoadCommand"))
                prefs.CreateEntry("onSaveLoadCommand", (string)"", "Execute command when save is loaded", false).disableWarning();
            if (!prefs.HasEntry("onMainMenuLoadCommand"))
                prefs.CreateEntry("onMainMenuLoadCommand", (string)"", "Execute command when main menu is loaded", false).disableWarning();
            if (!prefs.HasEntry("noclipSprintMultiply"))
                prefs.CreateEntry("noclipSprintMultiply", 2f, "Noclip sprint speed multiplier", false).disableWarning();
        }

        
        public override void OnLateInitializeMelon()
        {
            if (Get<GameObject>("SR2ELibraryROOT")) { rootOBJ = Get<GameObject>("SR2ELibraryROOT"); }
            else
            {
                rootOBJ = new GameObject();
                rootOBJ.SetActive(false);
                rootOBJ.name = "SR2ELibraryROOT";
                Object.DontDestroyOnLoad(rootOBJ);
            }

            MelonCoroutines.Start(CheckForNewVersion());
            
            //This is because ML has no way (to my knowledge) to get the version
            string logFilePath = Application.dataPath+"/../MelonLoader/Latest.log";
            using(System.IO.FileStream logFileStream = new System.IO.FileStream(logFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
            {
                using(System.IO.StreamReader logFileReader = new System.IO.StreamReader(logFileStream))
                {
                    string text = logFileReader.ReadToEnd();
                    MLVERSION = text.Split("\n")[1].Split("v")[1].Split(" ")[0];
                }
            }
        }
        public static string MLVERSION = "unknown";
        public static string newVersion = null;
        IEnumerator CheckForNewVersion()
        {
            UnityWebRequest uwr = UnityWebRequest.Get("https://raw.githubusercontent.com/ThatFinnDev/SR2E/main/latestver.txt");
            yield return uwr.SendWebRequest();

            if (!uwr.isNetworkError)
                newVersion = uwr.downloadHandler.text.Replace(" ","").Replace("\n","");
            
        }

        
        //Logging code from KomiksPL
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
            catch (Exception e) { Console.WriteLine(e); }
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

                    SaveCountChanged = false;
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
                    PlatformEngagementPrompt prompt = Object.FindObjectOfType<PlatformEngagementPrompt>();
                    Object.FindObjectOfType<CompanyLogoScene>().StartLoadingIndicator();
                    prompt.EngagementPromptTextUI.SetActive(false);
                    prompt.OnInteract(new InputAction.CallbackContext());
                    break;
                case "GameCore":
                    killDamage = new Damage { Amount = 99999999, DamageSource = ScriptableObject.CreateInstance<DamageSourceDefinition>(), };
                    killDamage.DamageSource.hideFlags |= HideFlags.HideAndDontSave;
                    AutoSaveDirector autoSaveDirector = GameContext.Instance.AutoSaveDirector;
                    autoSaveDirector.saveSlotCount = 50;
                    
                    foreach (ParticleSystemRenderer particle in Resources.FindObjectsOfTypeAll<ParticleSystemRenderer>())
                    {
                        var pname = particle.gameObject.name.Replace(' ', '_');
                        if (!FXLibrary.ContainsKey(particle.gameObject))
                            FXLibrary.AddItems(particle.gameObject, particle, pname);
                        if (!FXLibraryReversable.ContainsKey(pname))
                            FXLibraryReversable.AddItems(pname, particle, particle.gameObject);
                    }

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
                foreach (var text in SR2EConsole.gameObject.getAllChildrenOfType<TMP_Text>())
                    text.font = SR2Font;
            else if(normalFont!=null)
                foreach (var text in SR2EConsole.gameObject.getAllChildrenOfType<TMP_Text>())
                    text.font = normalFont==null?SR2Font:normalFont;
            foreach (var text in SR2EConsole.gameObject.getObjRec<GameObject>("modMenu")
                         .getAllChildrenOfType<TMP_Text>())
                text.font = SR2Font;

        }

        internal static void OnSaveDirectorLoading(AutoSaveDirector autoSaveDirector)
        {
            }

        internal static void SaveDirectorLoaded()
        {
            LocalizedString label = AddTranslation(translation("buttons.mods.label"), "b.button_mods_sr2e", "UI");
            new CustomMainMenuButton(label, LoadSprite("modsMenuIcon"), 2, (System.Action)(() => { SR2EModMenu.Open(); }));
            new CustomPauseMenuButton(label, 3, (System.Action)(() => { SR2EModMenu.Open(); }));
            
            if (devMode) new CustomPauseMenuButton( AddTranslation(translation("buttons.debugplayer.label"), "b.debug_player_sr2e", "UI"), 3, (System.Action)(() => { SR2EDebugDirector.DebugStatsManager.TogglePlayerDebugUI();}));

        }
        public override void OnSceneWasInitialized(int buildindex, string sceneName) { if(sceneName=="MainMenuUI") mainMenuLoaded = true; }
        public override void OnSceneWasUnloaded(int buildIndex, string sceneName) { if(sceneName=="MainMenuUI") mainMenuLoaded = false; SR2ESaveManager.WarpManager.OnSceneLoaded(); }
        internal static bool SaveCountChanged = false;
       
        public override void OnUpdate()
        {
            if (mainMenuLoaded)
            {
                if (!SaveCountChanged)
                {
                    SaveGamesRootUI ui = GameObject.FindObjectOfType<SaveGamesRootUI>();
                    if (ui != null)
                    {
                        GameObject scrollView = GameObject.Find("ButtonsScrollView");
                        if (scrollView != null)
                        {
                            ScrollRect rect = scrollView.GetComponent<ScrollRect>();
                            rect.vertical = true;
                            Scrollbar scrollBar = GameObject.Instantiate(SR2EConsole.transform.getObjRec<Scrollbar>("saveFilesSlider"), rect.transform);
                            rect.verticalScrollbar = scrollBar;
                            rect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
                            scrollBar.GetComponent<RectTransform>().localPosition += new Vector3(Screen.width/250f, 0, 0);
                            SaveCountChanged = true;
                        }
                    }
                    
                }
            }

            if (!consoleFinishedCreating)
            {
                GameObject obj = GameObject.FindGameObjectWithTag("Respawn");
                if (obj != null)
                {
                    consoleFinishedCreating = true;
                    SR2EConsole.gameObject = obj;
                    obj.name = "SR2Console";
                    GameObject.DontDestroyOnLoad(obj);
                    SR2EConsole.transform = obj.transform;
                    SR2EConsole.Start();
                }
            }
            else
            {
                try { SR2EConsole.Update(); } catch (Exception e) { Console.WriteLine(e); }
                try { SR2ESaveManager.Update(); } catch (Exception e) { Console.WriteLine(e); }
                try { SR2EModMenu.Update(); } catch (Exception e) { Console.WriteLine(e); }
                if(devMode)
                    SR2EDebugDirector.DebugStatsManager.Update();
            }
        }
        
    }
}