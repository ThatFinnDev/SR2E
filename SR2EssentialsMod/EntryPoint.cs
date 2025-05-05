using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Il2CppInterop.Runtime.Injection;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppTMPro;
using UnityEngine.UI;
using Il2CppKinematicCharacterController;
using Il2CppMonomiPark.ScriptedValue;
using Il2CppMonomiPark.SlimeRancher.Options;
using Newtonsoft.Json.Linq;
using SR2E.Expansion;
using SR2E.Buttons;
using SR2E.Components;
using SR2E.Managers;
using SR2E.Menus;
using SR2E.Patches.Context;
using SR2E.Storage;
using UnityEngine.Networking;

namespace SR2E;

public enum Branch
{
    Release,
    Beta,
    Alpha,
    Developer
}

// SR2E Build information. Please do not edit anything other than version numbers.
public static class BuildInfo
{
    public const string NAME = "SR2E";
    public const string DESCRIPTION = "Essential stuff for Slime Rancher 2";
    public const string AUTHOR = "ThatFinn";
    public const string CO_AUTHORS = "PinkTarr";
    public const string CODE_VERSION = "3.1.2";
    public const string DOWNLOAD_LINK = "https://sr2e.thatfinn.dev/";

    /// <summary>
    /// Should be the same as CodeVersion unless this is non release build.<br />
    /// For alpha versions, add "-alpha.buildnumber" e.g 3.0.0-alpha.5<br />
    /// For beta versions, add "-beta.buildnumber" e.g 3.0.0-beta.12<br />
    /// For dev versions, use "-dev". Do not add a build number!<br />
    /// Add "+metadata" only in dev builds!
    /// </summary>
    public const string DISPLAY_VERSION = "3.1.2";



    //allowmetadata, checkupdatelink,
    internal static readonly TripleDictionary<string, bool, string> PRE_INFO =
        new TripleDictionary<string, bool, string>()
        {
            { "alpha", (false, "https://api.sr2e.thatfinn.dev/downloads/sr2e/alpha.json") },
            { "beta", (false, "https://api.sr2e.thatfinn.dev/downloads/sr2e/beta.json") },
            { "dev", (true, "") },
        };
}

public class SR2EEntryPoint : MelonMod
{
    internal static ScriptedBool cheatsEnabledOnSave;
    internal static List<SR2EExpansionV1> expansions = new List<SR2EExpansionV1>();
    internal static TMP_FontAsset SR2Font;
    internal static TMP_FontAsset normalFont;
    internal static TMP_FontAsset regularFont;
    internal static TMP_FontAsset boldFont;
    internal static string updateBranch = BRANCHES[Branch.Release];
    internal static bool menusFinished = false;
    internal static bool mainMenuLoaded = false;
    internal static GameObject SR2EStuff;
    internal static bool updatedSR2E = false;
    internal static string newVersion = null;
    internal static ScriptedBool saveSkipIntro;
    internal static bool addedButtons = false;
    internal static List<BaseUI> baseUIAddSliders = new List<BaseUI>();
    internal static Dictionary<SR2EMenu, Dictionary<string, object>> menus = new Dictionary<SR2EMenu, Dictionary<string, object>>();
    static Dictionary<string, Type> menusToInit = new Dictionary<string, Type>();
    static MelonPreferences_Category prefs;
    static string branchJson = "";
    bool alreadyLoadedSettings = false;
    static bool IsLatestVersion => newVersion == BuildInfo.DISPLAY_VERSION;
    
    internal static string onSaveLoadCommand => prefs.GetEntry<string>("onSaveLoadCommand").Value; 
    internal static string onMainMenuLoadCommand => prefs.GetEntry<string>("onMainMenuLoadCommand").Value;
    internal static bool SR2ELogToMLLog => prefs.GetEntry<bool>("SR2ELogToMLLog").Value; 
    internal static bool mLLogToSR2ELog => prefs.GetEntry<bool>("mLLogToSR2ELog").Value; 
    internal static bool autoUpdate => prefs.GetEntry<bool>("autoUpdate").Value; 
    internal static bool quickStart => prefs.GetEntry<bool>("quickStart").Value; 
    internal static bool fixSaves => prefs.GetEntry<bool>("fixSaves").Value; 
    internal static float consoleMaxSpeed => prefs.GetEntry<float>("consoleMaxSpeed").Value; 
    internal static float noclipAdjustSpeed => prefs.GetEntry<float>("noclipAdjustSpeed").Value; 
    internal static float noclipSpeedMultiplier => prefs.GetEntry<float>("noclipSpeedMultiplier").Value; 
    internal static bool enableDebugDirector => prefs.GetEntry<bool>("enableDebugDirector").Value; 
    internal static bool enableCheatMenuButton => prefs.GetEntry<bool>("enableCheatMenuButton").Value; 
    
    static bool IsDisplayVersionValid() 
    {
        /*Semver2 Regex*/ var semVerRegex = new Regex(@"^(?<major>0|[1-9]\d*)\.(?<minor>0|[1-9]\d*)\.(?<patch>0|[1-9]\d*)(?:-(?<prerelease>[0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?(?:\+(?<build>[0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?$");
        var match = semVerRegex.Match(BuildInfo.DISPLAY_VERSION);
        /*Not Semver2*/ if (!match.Success) return false;
        string metadata = match.Groups["build"].Value;
        bool hasMetadata = !string.IsNullOrEmpty(metadata);
        string preReleaseAndBuild = match.Groups["prerelease"].Value;
        if (string.IsNullOrEmpty(preReleaseAndBuild)) return !hasMetadata; /*release and no meta*/
        /*No release -> continue*/ int dotIndex = preReleaseAndBuild.IndexOf('.');
        if (!(dotIndex != -1 && preReleaseAndBuild.LastIndexOf('.') == dotIndex && dotIndex != 0 && dotIndex != preReleaseAndBuild.Length - 1))
            if (preReleaseAndBuild != "dev") return false; /*Has no dot to indicate buildnumber*/
        string preRelease = preReleaseAndBuild != "dev" ? preReleaseAndBuild.Substring(0, dotIndex) : "dev";
        /*Check pre and meta*/ bool valid = false;
        foreach (var pair in BuildInfo.PRE_INFO)
            if (preRelease == pair.Key)
            {
                if (!pair.Value.Item1 && hasMetadata) return false; //Has meta even though it's not allowed
                valid = true;
                updateBranch = pair.Key;
                break;
            }
        if (!valid) return false;
        if (preRelease == "dev") return true;
        /*Check buildnumber*/ string buildnumber = preReleaseAndBuild.Substring(dotIndex + 1);
        if (int.TryParse(buildnumber, out int value)) return true;
        return false; /*buildnumber is no int*/
    }

    internal static void RefreshPrefs()
    {
        prefs.DeleteEntry("noclipFlySpeed");
        prefs.DeleteEntry("noclipFlySprintSpeed");
        prefs.DeleteEntry("experimentalStuff");
        prefs.DeleteEntry("skipEngagementPrompt");
        prefs.DeleteEntry("devMode");
        prefs.DeleteEntry("showUnityErrors");
        prefs.DeleteEntry("debugLogging");
        prefs.DeleteEntry("consoleUsesSR2Font");;
        prefs.DeleteEntry("consoleUsesSR2Style");;
        prefs.DeleteEntry("doesConsoleSync");
        prefs.DeleteEntry("mLLogToConsole");
        prefs.DeleteEntry("SR2ELogToMLLog");
        
        if(AllowAutoUpdate.HasFlag()) if (!prefs.HasEntry("autoUpdate")) prefs.CreateEntry("autoUpdate", (bool)false, "Update SR2E automatically");
        if (!prefs.HasEntry("fixSaves")) prefs.CreateEntry("fixSaves", (bool)false, "Fix broken saves (experimental)", false).AddNullAction();
        //if (!prefs.HasEntry("consoleUsesSR2Font")) prefs.CreateEntry("consoleUsesSR2Font", (bool)false, "Console uses SR2 font", false).AddAction((System.Action)(() => { SetupFonts(); }));
        if (!prefs.HasEntry("quickStart")) prefs.CreateEntry("quickStart", (bool)false, "Quickstart (may break other mods)");
        if (!prefs.HasEntry("enableDebugDirector")) prefs.CreateEntry("enableDebugDirector", (bool)false, "Enable debug menu", false).AddAction((System.Action)(() => 
            { SR2EDebugDirector.isEnabled = enableDebugDirector; }));
        if (!prefs.HasEntry("enableCheatMenuButton")) prefs.CreateEntry("enableCheatMenuButton", (bool)false, "Enable cheat menu button in pause menu", false).AddAction((System.Action)(() => 
            {
                if (!enableCheatMenuButton) cheatMenuButton.Remove();
                if (enableCheatMenuButton) cheatMenuButton.AddAgain();
            }));
        if (!prefs.HasEntry("mLLogToSR2ELog")) prefs.CreateEntry("mLLogToSR2ELog", (bool)false, "Send MLLogs to console", false).AddNullAction();
        if (!prefs.HasEntry("SR2ELogToMLLog")) prefs.CreateEntry("SR2ELogToMLLog", (bool)false, "Send console messages to MLLogs", false).AddNullAction();
        if (!prefs.HasEntry("onSaveLoadCommand")) prefs.CreateEntry("onSaveLoadCommand", (string)"", "Execute command when save is loaded", false).AddNullAction();
        if (!prefs.HasEntry("onMainMenuLoadCommand")) prefs.CreateEntry("onMainMenuLoadCommand", (string)"", "Execute command when main menu is loaded", false).AddNullAction();
        if (!prefs.HasEntry("noclipSpeedMultiplier")) prefs.CreateEntry("noclipSpeedMultiplier", 2f, "NoClip sprint speed multiplier", false).AddNullAction();
        if (!prefs.HasEntry("noclipAdjustSpeed")) prefs.CreateEntry("noclipAdjustSpeed", (float)235f, "NoClip scroll speed", false).AddNullAction();
        if (!prefs.HasEntry("consoleMaxSpeed")) prefs.CreateEntry("consoleMaxSpeed", (float)0.75f, "Controls how fast you scroll in the Console", false).AddNullAction();
    }
    public override void OnLateInitializeMelon()
    {
        if (Get<GameObject>("SR2EPrefabHolder")) rootOBJ = Get<GameObject>("SR2EPrefabHolder");
        else
        {
            rootOBJ = new GameObject();
            rootOBJ.SetActive(false);
            rootOBJ.name = "SR2EPrefabHolder";
            Object.DontDestroyOnLoad(rootOBJ);
        }
        MelonCoroutines.Start(GetBranchJson());
    }
    IEnumerator GetBranchJson()
    {
        string checkLink = "https://api.sr2e.thatfinn.dev/downloads/sr2e/release.json";
        if (updateBranch != "release") checkLink = BuildInfo.PRE_INFO[updateBranch].Item2;
        if (string.IsNullOrEmpty(checkLink)) yield break;
        UnityWebRequest uwr = UnityWebRequest.Get(checkLink);
        yield return uwr.SendWebRequest();
        if (uwr.isNetworkError || uwr.isHttpError) yield break;
        string json = uwr.downloadHandler.text;
        try { JObject.Parse(json); }
        catch { MelonLogger.Msg("SR2E API either changed or is broken."); yield break; }
        branchJson = json;
        if (CheckForUpdates.HasFlag()) MelonCoroutines.Start(CheckForNewVersion());
    }

    IEnumerator CheckForNewVersion()
    {
        if (string.IsNullOrWhiteSpace(branchJson)) yield break;
        try
        {
            var jobject = JObject.Parse(branchJson);
            string latest = jobject["latest"].ToObject<string>();
            newVersion = latest;
            if (!IsLatestVersion) if (AllowAutoUpdate.HasFlag()) if (autoUpdate)
                MelonCoroutines.Start(UpdateVersion());
        }
        catch { MelonLogger.Msg("SR2E API either changed or is broken."); }
    }
    IEnumerator UpdateVersion()
    {
        if (string.IsNullOrWhiteSpace(branchJson)) yield break;
        string updateLink = "";
        try
        {
            var jobject = JObject.Parse(branchJson);
            string latest = jobject["latest"].ToObject<string>();
            var versions = jobject["versions"].ToObject<Dictionary<string, string>>();
            updateLink = versions[latest];
        }
        catch { MelonLogger.Msg("SR2E API either changed or is broken."); yield break; }
        if (string.IsNullOrEmpty(updateLink)) yield break;
        UnityWebRequest uwr = UnityWebRequest.Get(updateLink);
        yield return uwr.SendWebRequest();
        if (!uwr.isNetworkError && !uwr.isHttpError)
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                if (DebugLogging.HasFlag()) MelonLogger.Msg("Downloading SR2E complete");
                string path = MelonAssembly.Assembly.Location;
                if (File.Exists(path)) File.Move(path, path + ".old");
                File.WriteAllBytes(Path.Combine(new FileInfo("path").Directory.FullName, "SR2E.dll"), uwr.downloadHandler.data);
                updatedSR2E = true;
                if (DebugLogging.HasFlag()) MelonLogger.Msg("Restart needed for applying SR2E update");
            }
    }



    //Logging code from Atmudia and adapted
    private static void AppLogUnity(string message, string trace, LogType type)
    {
        if (!ShowUnityErrors.HasFlag()) return;
        if (message.Equals(string.Empty)) return;
        string toDisplay = message;
        if (!trace.Equals(string.Empty)) toDisplay += "\n" + trace;
        toDisplay = Regex.Replace(toDisplay, @"\[INFO]\s|\[ERROR]\s|\[WARNING]\s", "");
        switch (type)
        {
            case LogType.Assert: unityLog.Error(toDisplay); break;
            case LogType.Exception: unityLog.Error(toDisplay + trace); break;
            case LogType.Log: unityLog.Msg(toDisplay); break;
            case LogType.Error: unityLog.Error(toDisplay + trace); break;
            case LogType.Warning: unityLog.Warning(toDisplay); break;
        }
    }
    public override void OnEarlyInitializeMelon()
    {
        if (!IsDisplayVersionValid()) { MelonLogger.Msg("Version Code is broken!"); Unregister(); return; }
        InitFlagManager();
    }

    static MelonLogger.Instance unityLog = new MelonLogger.Instance("Unity");
    public override void OnInitializeMelon()
    {
        prefs = MelonPreferences.CreateCategory("SR2E", "SR2E");
        string path = MelonAssembly.Assembly.Location + ".old";
        if (File.Exists(path)) File.Delete(path);
        RefreshPrefs();

        Application.add_logMessageReceived(new Action<string, string, LogType>(AppLogUnity));
        try { AddLanguages(LoadTextFile("SR2E.translations.csv")); } catch (Exception e) { MelonLogger.Error(e); }
        foreach (MelonBase melonBase in new List<MelonBase>(MelonBase.RegisteredMelons))
        {
            if (melonBase is SR2EExpansionV1)
                if (AllowExpansions.HasFlag()) expansions.Add(melonBase as SR2EExpansionV1);
                else melonBase.Unregister();
        }
        foreach (var expansion in expansions) try { expansion.OnNormalInitializeMelon(); } catch (Exception e) { MelonLogger.Error(e); }
    }
    public override void OnApplicationQuit()
    {
        try { if (SystemContext.Instance.SceneLoader.IsCurrentSceneGroupGameplay()) GameContext.Instance.AutoSaveDirector.SaveGame(); }catch { }
    }
    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (DebugLogging.HasFlag()) MelonLogger.Msg("OnLoaded Scene: " + sceneName);
        switch (sceneName)
        {
            case "MainMenuUI":
                CustomTimeScale = 1f;
                if (ExperimentalSettingsInjection.HasFlag())
                {
                    bool tempLoad = alreadyLoadedSettings;
                    if (tempLoad)
                    {
                        CustomSettingsCreator.ClearUsedIDs();
                        CustomSettingsCreator.AllSettingsButtons =
                            new Dictionary<string, List<ScriptedValuePresetOptionDefinition>>()
                            {
                                { "GameSettings", new List<ScriptedValuePresetOptionDefinition>() },
                                { "Display", new List<ScriptedValuePresetOptionDefinition>() },
                                { "Audio", new List<ScriptedValuePresetOptionDefinition>() },
                                { "Bindings_Controller", new List<ScriptedValuePresetOptionDefinition>() },
                                { "Bindings_Keyboard", new List<ScriptedValuePresetOptionDefinition>() },
                                { "Input", new List<ScriptedValuePresetOptionDefinition>() },
                                { "Gameplay_InGame", new List<ScriptedValuePresetOptionDefinition>() },
                                { "Gameplay_MainMenu", new List<ScriptedValuePresetOptionDefinition>() },
                                { "Graphics", new List<ScriptedValuePresetOptionDefinition>() }
                            };

                    }
                    alreadyLoadedSettings = true;
                    RegisterOptionMenuButtons += (_, _) =>
                    {
                        var testVal = CustomSettingsCreator.CreateScriptedInt(0);

                        List<ScriptedValuePresetOptionDefinition> options =
                            new List<ScriptedValuePresetOptionDefinition>();
                        options.Add(CustomSettingsCreator.Create(
                            CustomSettingsCreator.BuiltinSettingsCategory.ManualOrCustom,
                            AddTranslationFromSR2E("setting.gamesettingtest", "b.testsetting", "UI"),
                            AddTranslationFromSR2E("setting.gamesettingtest.desc", "l.testsettingdescription",
                                "UI"), "testButton1", 1,true, true, false,
                            (def, idx, _) => { MelonLogger.Msg($"Test button edited! New value index: {idx}."); },
                            new CustomSettingsCreator.OptionValue("val1",
                                AddTranslationFromSR2E("setting.gamesettingtest.value1", "l.testsettingvalue1",
                                    "UI"), testVal, 0),
                            new CustomSettingsCreator.OptionValue("val2",
                                AddTranslationFromSR2E("setting.gamesettingtest.value2", "l.testsettingvalue2",
                                    "UI"), testVal, 1),
                            new CustomSettingsCreator.OptionValue("val3",
                                AddTranslationFromSR2E("setting.gamesettingtest.value3", "l.testsettingvalue3",
                                    "UI"), testVal, 2)
                        ));
                        CustomSettingsCreator.CreateCategory(
                            AddTranslationFromSR2E("setting.categoryname", "l.sr2ecategory", "UI"),
                            SR2EUtils.ConvertToSprite(SR2EUtils.LoadImage("category")),
                            options.ToArray());

                        cheatsEnabledOnSave = CustomSettingsCreator.CreateScruptedBool(true);
                        saveSkipIntro = CustomSettingsCreator.CreateScruptedBool(false);

                        CustomSettingsCreator.Create(
                            CustomSettingsCreator.BuiltinSettingsCategory.GameSettings,
                            AddTranslationFromSR2E("setting.allowcheats", "b.cheatingsetting", "UI"),
                            AddTranslationFromSR2E("setting.allowcheats.desc",
                                "l.cheatingsettingdescription",
                                "UI"),"allowCheating",0,true,true,false,
                            (def, idx, _) => { },
                            new CustomSettingsCreator.OptionValue("off",
                                AddTranslationFromSR2E( "setting.allowcheats.off","l.settingvalueno","UI"),
                                cheatsEnabledOnSave, false),
                            new CustomSettingsCreator.OptionValue("on",
                                AddTranslationFromSR2E("setting.allowcheats.on", "l.settingvalueyes", "UI"),
                                cheatsEnabledOnSave, true)
                        );

                        CustomSettingsCreator.Create(
                            CustomSettingsCreator.BuiltinSettingsCategory.GameSettings,
                            AddTranslationFromSR2E("setting.skipintro", "b.skipintrosetting", "UI"),
                            AddTranslationFromSR2E("setting.skipintro.desc", "l.skipintrosettingdescription",
                                "UI"), "skipIntro",1,true,true,false,
                            (_, _, _) => { },
                            new CustomSettingsCreator.OptionValue("off",
                                sr2etosrlanguage["setting.allowcheats.off"],
                                saveSkipIntro, false),
                            new CustomSettingsCreator.OptionValue("on",
                                sr2etosrlanguage["setting.allowcheats.on"],
                                saveSkipIntro, true)
                        );
                    };

                    RegisterOptionMenuButtons?.Invoke(this, EventArgs.Empty);

                    var optionCategories = Resources.FindObjectsOfTypeAll<OptionsItemCategory>();
                    foreach (var category in optionCategories)
                    {
                        switch (category.name)
                        {
                            case "GameSettings": CustomSettingsCreator.ApplyButtons(CustomSettingsCreator.BuiltinSettingsCategory.GameSettings, category); break;
                            case "Display": CustomSettingsCreator.ApplyButtons(CustomSettingsCreator.BuiltinSettingsCategory.Display, category); break;
                            case "Audio": CustomSettingsCreator.ApplyButtons(CustomSettingsCreator.BuiltinSettingsCategory.Audio, category); break;
                            case "BindingsGamepad": CustomSettingsCreator.ApplyButtons(CustomSettingsCreator.BuiltinSettingsCategory.Bindings_Controller, category); break;
                            case "Input": CustomSettingsCreator.ApplyButtons(CustomSettingsCreator.BuiltinSettingsCategory.Input, category); break;
                            case "Gameplay_MainMenu": CustomSettingsCreator.ApplyButtons(CustomSettingsCreator.BuiltinSettingsCategory.Gameplay_MainMenu, category); break;
                            case "BindingsKbm": CustomSettingsCreator.ApplyButtons(CustomSettingsCreator.BuiltinSettingsCategory.Bindings_Keyboard, category); break;
                            case "Video": CustomSettingsCreator.ApplyButtons(CustomSettingsCreator.BuiltinSettingsCategory.Graphics, category); break;
                            default:
                                // There are 2 other categories, but they are console only. 
                                // Also, the Gameplay_InGame is loaded somewhere after GameCore.
                                break;
                        }
                        //MelonLogger.BigError("SR2E TODO", "PLEASE IMPLEMENT THE GAMEPLAY_INGAME SETTINGS CATEGORY");
                    }
                    CustomSettingsCreator.ApplyModel();
                }
                break;
            case "StandaloneEngagementPrompt":
                Object.FindObjectOfType<CompanyLogoScene>().StartLoadingIndicator();
                break;
            case "PlayerCore":
                NoClipComponent.playerSettings = Get<KCCSettings>("");
                NoClipComponent.player = SceneContext.Instance.player.transform;
                NoClipComponent.playerController = NoClipComponent.player.GetComponent<SRCharacterController>();
                NoClipComponent.playerMotor = NoClipComponent.player.GetComponent<KinematicCharacterMotor>();
                player = Get<GameObject>("PlayerControllerKCC");
                break;
            case "UICore":
                CheckForTime();
                break;
        }

        switch (sceneName)
        {
            case "StandaloneEngagementPrompt": foreach (var expansion in expansions) try { expansion.OnStandaloneEngagementPromptLoad(); }catch (Exception e) { MelonLogger.Error(e); } break;
            case "PlayerCore": foreach (var expansion in expansions) try { expansion.OnPlayerCoreLoad(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "UICore": foreach (var expansion in expansions) try { expansion.OnUICoreLoad(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "MainMenuUI": foreach (var expansion in expansions) try { expansion.OnMainMenuUILoad(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "LoadScene": foreach (var expansion in expansions) try { expansion.OnLoadSceneLoad(); } catch (Exception e) { MelonLogger.Error(e); } break;
        }

        SR2ECommandManager.OnSceneWasLoaded(buildIndex, sceneName);
    }

    void CheckForTime()
    {
        if (!inGame) return;
        if (Time.timeScale == 1f) Time.timeScale = CustomTimeScale;
        if (SceneContext.Instance.TimeDirector._timeFactor == 1f) SceneContext.Instance.TimeDirector._timeFactor = CustomTimeScale;
        ExecuteInSeconds((Action)(() => { CheckForTime();}), 1);
    }
    public static event EventHandler RegisterOptionMenuButtons;
    static bool useSR2Font = true;

    internal static void SendFontError(string name)
    {
        MelonLogger.Error($"The font '{name}' couldn't be loaded!");
        MelonLogger.Error("This happens on some platforms and I (the dev) haven't found a fix yet!");
    }
    internal static void SetupFonts()
    {
        if (SR2Font == null) SR2Font = FontFromGame("Runsell Type - HemispheresCaps2");
        if (regularFont == null) regularFont = FontFromGame("Lexend-Regular (Latin)"); 
        if (boldFont == null) boldFont = FontFromGame("Lexend-Bold (Latin)"); 
        if (normalFont == null) normalFont = FontFromOS("Tahoma"); 
        foreach (var expansion in expansions) try { expansion.OnSR2FontLoad(); }catch (Exception e) { MelonLogger.Error(e); }
        foreach (var pair in menus) pair.Key.ReloadFont();
    }

    internal static void OnSaveDirectorLoading(AutoSaveDirector autoSaveDirector)
    {
        foreach (var expansion in expansions) try { expansion.OnSaveDirectorLoading(autoSaveDirector); }catch (Exception e) { MelonLogger.Error(e); }
    }

    internal static CustomPauseMenuButton cheatMenuButton;
    internal static bool isSaveDirectorLoaded = false;

    internal static void SaveDirectorLoaded()
    {
        if (isSaveDirectorLoaded) return;
        isSaveDirectorLoaded = true;
        foreach (var expansion in expansions) try { expansion.SaveDirectorLoaded(GameContext.Instance.AutoSaveDirector); } catch (Exception e) { MelonLogger.Error(e); }
    }

    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        if (DebugLogging.HasFlag()) MelonLogger.Msg("WasInitialized Scene: " + sceneName);
        if (sceneName == "MainMenuUI") mainMenuLoaded = true;
        switch (sceneName)
        {                
            case "StandaloneEngagementPrompt": foreach (var expansion in expansions) try { expansion.OnStandaloneEngagementPromptInitialize(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "PlayerCore": foreach (var expansion in expansions) try { expansion.OnPlayerCoreInitialize(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "UICore": foreach (var expansion in expansions) try { expansion.OnUICoreInitialize(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "MainMenuUI": foreach (var expansion in expansions) try { expansion.OnMainMenuUIInitialize(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "LoadScene": foreach (var expansion in expansions) try { expansion.OnLoadSceneInitialize(); } catch (Exception e) { MelonLogger.Error(e); } break;
        }
        SR2ECommandManager.OnSceneWasInitialized(buildIndex, sceneName);
    }

    public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
    {
        if (DebugLogging.HasFlag()) MelonLogger.Msg("OnUnloaded Scene: " + sceneName);
        if (sceneName == "MainMenuUI") mainMenuLoaded = false;
        try { SR2EWarpManager.OnSceneUnloaded(); }
        catch (Exception e) { MelonLogger.Error(e); }
        switch (sceneName)
        {               
            case "StandaloneEngagementPrompt": foreach (var expansion in expansions) try { expansion.OnStandaloneEngagementPromptUnload(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "PlayerCore": foreach (var expansion in expansions) try { expansion.OnPlayerCoreUnload(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "UICore": foreach (var expansion in expansions) try { expansion.OnUICoreUnload(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "MainMenuUI": foreach (var expansion in expansions) try { expansion.OnMainMenuUIUnload(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "LoadScene": foreach (var expansion in expansions) try { expansion.OnLoadSceneUnload(); } catch (Exception e) { MelonLogger.Error(e); } break;
        }
        SR2ECommandManager.OnSceneWasUnloaded(buildIndex, sceneName);
    }


    public override void OnUpdate()
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
                    Scrollbar scrollBar = GameObject.Instantiate(SR2EStuff.getObjRec<Scrollbar>("saveFilesSliderRec"),
                        rect.transform);
                    rect.verticalScrollbar = scrollBar;
                    rect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
                    scrollBar.GetComponent<RectTransform>().localPosition += new Vector3(Screen.width / 250f, 0, 0);
                }
            }
            baseUIAddSliders.Remove(ui);
        }

        if (!menusFinished)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Respawn");
            if (SR2EStuff != null)
            {
                SR2EStuff.SetActive(true);
                foreach (var pair in new Dictionary<string, Type>(menusToInit))
                    for (int i = 0; i < SR2EStuff.transform.childCount; i++)
                    {
                        Transform child = SR2EStuff.transform.GetChild(i);
                        if (child.name == pair.Key)
                        {
                            try
                            {
                                var methodInfo = pair.Value.GetMethod(nameof(SR2EMenu.PreAwake), BindingFlags.Static | BindingFlags.Public);
                                var result = methodInfo.Invoke(null, new[] { child.gameObject });
                                child.gameObject.SetActive(true);
                                menusToInit.Remove(pair.Key);
                            }catch (Exception e) { MelonLogger.Error(e); }
                        }
                    }
                menusFinished = true;
            }
            else if (obj != null)
            {
                SR2ELogManager.Start();
                SR2ESaveManager.Start();
                SR2ECommandManager.Start();
                SR2EStuff = obj;
                obj.name = "SR2EStuff";
                obj.tag = "";
                obj.SetActive(false);
                GameObject.DontDestroyOnLoad(obj);
                foreach (Type type in MelonAssembly.Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(SR2EMenu)) && !t.IsAbstract))
                {
                    try
                    {
                        var methodInfo = type.GetMethod(nameof(SR2EMenu.GetMenuIdentifier), BindingFlags.Static | BindingFlags.Public);
                        var result = methodInfo.Invoke(null, null);
                        if (result is MenuIdentifier identifier)
                        {
                            var asset = SystemContextPatch.bundle.LoadAsset(SystemContextPatch.getMenuPath(identifier));
                            var Object = GameObject.Instantiate(asset, obj.transform);
                            menusToInit.Add(Object.name, type);
                            ClassInjector.RegisterTypeInIl2Cpp(type, new RegisterTypeOptions() { LogSuccess = false });
                        }
                        else MelonLogger.Error($"The menu under the name {type.Name} couldn't be loaded! It's MenuIdentifier is broken!");

                    }catch (Exception e) { MelonLogger.Error(e); }
                }
            }
        }
        else
        {
            try { if (SR2EInputManager.OnKeyPressed(GM<SR2EConsole>().openKey)) GM<SR2EConsole>().Toggle(); } catch (Exception e) { MelonLogger.Error(e); }
            try { SR2ECommandManager.Update(); } catch (Exception e) { MelonLogger.Error(e); }
            try { SR2EBindingManger.Update(); } catch (Exception e) { MelonLogger.Error(e); }
            if (DevMode.HasFlag()) SR2EDebugDirector.DebugStatsManager.Update();
        }

        if(actionCounter.Count>0) foreach (var pair in new Dictionary<Action, int>(actionCounter))
        {
            if (pair.Value < 1)
            {
                try { pair.Key.Invoke(); } catch (Exception e) { MelonLogger.Error(e); }
                actionCounter.Remove(pair.Key);
            }
            else actionCounter[pair.Key]--;
        }
    }
}
