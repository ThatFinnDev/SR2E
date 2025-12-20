using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions; 
using Il2CppInterop.Runtime.Injection;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppTMPro;
using UnityEngine.UI;
using Il2CppMonomiPark.ScriptedValue;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using MelonLoader.Utils;
using Newtonsoft.Json.Linq;
using SR2E.Expansion;
using SR2E.Components;
using SR2E.Managers;
using SR2E.Menus;
using SR2E.Patches.Context;
using SR2E.Patches.General;
using SR2E.Prism;
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
    public const string Name = "SR2E";
    public const string Description = "Essential stuff for Slime Rancher 2";
    public const string Author = "ThatFinn";
    public const string Contributors = "PinkTarr, shizophrenicgopher, Atmudia";
    public const string CodeVersion = "3.5.1";
    public const string DownloadLink = "https://sr2e.sr2.dev/";
    public const string SourceCode = "https://github.com/ThatFinnDev/SR2E";
    public const string Nexus = "https://www.nexusmods.com/slimerancher2/mods/60";

    /// <summary>
    /// Should be the same as CodeVersion unless this is non release build.<br />
    /// For alpha versions, add "-alpha.buildnumber" e.g 3.0.0-alpha.5<br />
    /// For beta versions, add "-beta.buildnumber" e.g 3.0.0-beta.12<br />
    /// For dev versions, use "-dev". Do not add a build number!<br />
    /// Add "+metadata" only in dev builds!
    /// </summary>
    public const string DisplayVersion = "3.5.1-dev";

    //allowmetadata, checkupdatelink,
    internal static readonly TripleDictionary<string, bool, string> PRE_INFO =
        new TripleDictionary<string, bool, string>()
        {
            { "release", (false, "https://api.sr2e.sr2.dev/branch/release") },
            { "alpha", (false, "https://api.sr2e.sr2.dev/branch/alpha") },
            { "beta", (false, "https://api.sr2e.sr2.dev/branch/beta") },
            { "dev", (true, "") },
        };
}

public class SR2EEntryPoint : MelonMod
{
    internal static List<SR2EExpansionV1> expansionsV1V2 = new();
    internal static List<SR2EExpansionV2> expansionsV2 = new();
    internal static List<SR2EExpansionV3> expansionsV3 = new();
    internal static TMP_FontAsset SR2Font;
    internal static TMP_FontAsset normalFont;
    internal static TMP_FontAsset regularFont;
    internal static TMP_FontAsset boldFont;
    internal static TMP_FontAsset notoSansFont;
    internal static string updateBranch = MiscEUtil.BRANCHES[Branch.Release];
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
    private static string SR2EFolderName = "SR2E";
    internal static string DataPath => Path.Combine(MelonEnvironment.UserDataDirectory, SR2EFolderName);
    internal static string TmpDataPath => Path.Combine(DataPath, ".tmp");
    internal static string FlagDataPath => Path.Combine(DataPath, "flags");
    internal static string CustomVolumeProfilesPath => Path.Combine(DataPath, "customVolumeProfiles");
    static bool IsLatestVersion => newVersion == BuildInfo.DisplayVersion;
    
    private static bool earlyRegistered = false;
    private static bool _usePrism = false;
    internal static bool usePrism => _usePrism;
    static MelonLogger.Instance unityLog = new MelonLogger.Instance("Unity");
    internal static string _mlVersion = "undefined";
    
    
    
    internal static string onSaveLoadCommand => prefs.GetEntry<string>("onSaveLoadCommand").Value; 
    internal static string onMainMenuLoadCommand => prefs.GetEntry<string>("onMainMenuLoadCommand").Value;
    internal static bool SR2ELogToMLLog => prefs.GetEntry<bool>("SR2ELogToMLLog").Value; 
    internal static bool mLLogToSR2ELog => prefs.GetEntry<bool>("mLLogToSR2ELog").Value; 
    internal static bool autoUpdate => prefs.GetEntry<bool>("autoUpdate").Value;
    internal static bool disableFixSaves => prefs.GetEntry<bool>("disableFixSaves").Value; 
    internal static float consoleMaxSpeed => prefs.GetEntry<float>("consoleMaxSpeed").Value; 
    internal static float noclipAdjustSpeed => prefs.GetEntry<float>("noclipAdjustSpeed").Value; 
    internal static float noclipSpeedMultiplier => prefs.GetEntry<float>("noclipSpeedMultiplier").Value; 
    internal static bool enableDebugDirector => prefs.GetEntry<bool>("enableDebugDirector").Value;
    
    static bool IsDisplayVersionValid()
    {
        if (!BuildInfo.DisplayVersion.Contains(BuildInfo.CodeVersion)) return false;
        /*Semver2 Regex*/ var semVerRegex = new Regex(@"^(?<major>0|[1-9]\d*)\.(?<minor>0|[1-9]\d*)\.(?<patch>0|[1-9]\d*)(?:-(?<prerelease>[0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?(?:\+(?<build>[0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?$");
        var match = semVerRegex.Match(BuildInfo.DisplayVersion);
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
            if (preRelease == pair.Key&&pair.Key!="release")
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

    static void RefreshPrefs()
    {
        var old_entries = new string[] {
            "noclipFlySpeed","noclipFlySprintSpeed","experimentalStuff","skipEngagementPrompt","devMode","showUnityErrors",
            "debugLogging","consoleUsesSR2Font","consoleUsesSR2Style","doesConsoleSync","mLLogToConsole","SR2ELogToMLLog","fixSaves",
            "quickStart","enableCheatMenuButton", "useExperimentalLibrary"
        };
        foreach (var entry in old_entries)
            prefs.DeleteEntry(entry);
        
        if(AllowAutoUpdate.HasFlag()) if (!prefs.HasEntry("autoUpdate")) prefs.CreateEntry("autoUpdate", (bool)false, "Update SR2E automatically");
        if(DevMode.HasFlag()) if(AllowPrism.HasFlag()) if (!prefs.HasEntry("forceUsePrism")) prefs.CreateEntry("forceUsePrism", (bool)false, "Force enable prism", "It's automatically enabled if expansions need it. This will just force it.",false);
        if (!prefs.HasEntry("disableFixSaves")) prefs.CreateEntry("disableFixSaves", (bool)false, "Disable save fixing", false).AddNullAction();
        if (!prefs.HasEntry("enableDebugDirector")) prefs.CreateEntry("enableDebugDirector", (bool)false, "Enable debug menu", false).AddAction(() => { SR2EDebugUI.isEnabled = enableDebugDirector; });
        if (!prefs.HasEntry("mLLogToSR2ELog")) prefs.CreateEntry("mLLogToSR2ELog", (bool)false, "Send MLLogs to console", false).AddNullAction();
        if (!prefs.HasEntry("SR2ELogToMLLog")) prefs.CreateEntry("SR2ELogToMLLog", (bool)false, "Send console messages to MLLogs", false).AddNullAction();
        if (!prefs.HasEntry("onSaveLoadCommand")) prefs.CreateEntry("onSaveLoadCommand", (string)"", "Command to execute, when save is loaded", false).AddNullAction();
        if (!prefs.HasEntry("onMainMenuLoadCommand")) prefs.CreateEntry("onMainMenuLoadCommand", (string)"", "Command to execute, when main menu is loaded", false).AddNullAction();
        if (!prefs.HasEntry("noclipSpeedMultiplier")) prefs.CreateEntry("noclipSpeedMultiplier", 2f, "NoClip sprint speed multiplier", false).AddNullAction();
        if (!prefs.HasEntry("noclipAdjustSpeed")) prefs.CreateEntry("noclipAdjustSpeed", (float)235f, "NoClip scroll speed", false).AddNullAction();
        if (!prefs.HasEntry("consoleMaxSpeed")) prefs.CreateEntry("consoleMaxSpeed", (float)0.75f, "Console scroll speed", false).AddNullAction();
        //if(DevMode.HasFlag()) if (!prefs.HasEntry("testLKey")) prefs.CreateEntry("testLKey", LKey.None, "Test LKey", false).AddNullAction();
    }



    
    public override void OnLateInitializeMelon()
    {
        if (Get<GameObject>("SR2EPrefabHolder")) prefabHolder = Get<GameObject>("SR2EPrefabHolder");
        else
        {
            prefabHolder = new GameObject();
            prefabHolder.SetActive(false);
            prefabHolder.name = "SR2EPrefabHolder";
            Object.DontDestroyOnLoad(prefabHolder);
        }

        if (LKeyInputAcquirer.Instance == null)
        {
            
            var ia = new GameObject();
            ia.AddComponent<LKeyInputAcquirer>();
            ia.AddComponent<KeyCodeInputAcquirer>();
            if (RestoreDebugAbilities.HasFlag()) ia.AddComponent<DevelopmentBuildText>();
            if (RestoreDebugDevConsole.HasFlag()) ia.AddComponent<DevConsoleFixer>();
            ia.name = "SR2EInputAcquirer";
            Object.DontDestroyOnLoad(ia);
        }

        if (CheckForUpdates.HasFlag()) MelonCoroutines.Start(GetBranchJson());

        if (usePrism)
        {
            //SaveComponents.RegisterComponent(typeof(ModdedV01));
        } 
        foreach (var expansion in expansionsV3) try { expansion.OnLateInitializeMelon(); } catch (Exception e) { MelonLogger.Error(e); }
    }
    IEnumerator GetBranchJson()
    {
        string checkLink = BuildInfo.PRE_INFO[updateBranch].Item2;
        if (string.IsNullOrEmpty(checkLink)) yield break;
        UnityWebRequest uwr = UnityWebRequest.Get(checkLink);
        yield return uwr.SendWebRequest();
        if (uwr.isNetworkError || uwr.isHttpError) yield break;
        string json = uwr.downloadHandler.text;
        try { JObject.Parse(json); }
        catch { MelonLogger.Msg("SR2E API either changed or is broken."); yield break; }
        branchJson = json;
        MelonCoroutines.Start(CheckForNewVersion());
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
            var latestVersion = jobject["versions_info"][latest];
            updateLink = latestVersion["download_url"].ToObject<string>();
        }
        catch { MelonLogger.Msg("SR2E API either changed or is broken."); yield break; }
        if (string.IsNullOrEmpty(updateLink)) yield break;
        UnityWebRequest uwr = UnityWebRequest.Get(updateLink);
        yield return uwr.SendWebRequest();
        if (!uwr.isNetworkError && !uwr.isHttpError)
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                MelonLogger.Msg("Downloading SR2E complete");
                string path = MelonAssembly.Assembly.Location;
                if (File.Exists(path))
                {
                    if(File.Exists(path + ".old")) File.Delete(path + ".old");
                    File.Move(path, path + ".old");
                }
                File.WriteAllBytes(Path.Combine(new FileInfo(path).Directory.FullName, "SR2E.dll"), uwr.downloadHandler.data);
                updatedSR2E = true;
                MelonLogger.Msg("Restart needed for applying SR2E update");
            }
    }



    //Logging code from Atmudia and adapted
    private static void AppLogUnity(string message, string trace, LogType type)
    {
        if (!ShowUnityErrors.HasFlag()) return;
        if (message.Equals(string.Empty)) return;
        string toDisplay = message;
        if(trace.StartsWith("TMPro.TextMeshProUGUI.Rebuild (UnityEngine.UI.CanvasUpdate update)")) return;
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
        StaticOnEarlyInitializeMelon();
        PatchIl2CppDetourMethodPatcher.InstallSecondPart(HarmonyInstance);
    }

    static void StaticOnEarlyInitializeMelon()
    {
        if (earlyRegistered) return;
        earlyRegistered = true;
        string[] launchArgs = Environment.GetCommandLineArgs();
        var usedArgs = new List<String>();
        foreach (string arg in launchArgs)
        {
            if (arg.StartsWith("-sr2e.") && arg.Contains("="))
            {
                var split = arg.Split("=");
                if (split.Length != 2) continue;
                if (usedArgs.Contains(split[0])) continue;
                usedArgs.Add(split[0]);
                switch (split[0])
                {
                    case "-sr2e.id":
                        int id = 0;
                        try { id = int.Parse(split[1]); }catch { }
                        if (id != 0) SR2EFolderName += id;
                        break;
                }
            }
        }
        if(!Directory.Exists(DataPath)) Directory.CreateDirectory(DataPath);
        if(!Directory.Exists(TmpDataPath)) Directory.CreateDirectory(TmpDataPath);
        if(!Directory.Exists(FlagDataPath)) Directory.CreateDirectory(FlagDataPath);
        if(!Directory.Exists(CustomVolumeProfilesPath)) Directory.CreateDirectory(CustomVolumeProfilesPath);
        InitFlagManager();
    }

    void InjectIl2CppComponents()
    {
        var types = AccessTools.GetTypesFromAssembly(MelonAssembly.Assembly);
        var devPatches = DevMode.HasFlag();
        foreach (var type in types)
        {
            if (type == null) continue;
            try
            {
                if (type.GetCustomAttribute<InjectClass>() == null) continue;
                if(!ClassInjector.IsTypeRegisteredInIl2Cpp(type))
                    ClassInjector.RegisterTypeInIl2Cpp(type, new RegisterTypeOptions() { LogSuccess = false });
            }
            catch (Exception e)
            {
                MelonLogger.Error(e);
                MelonLogger.Error($"Failed to inject {type.FullName}: {e.Message}");
            }
        }
    }
    void PatchGame()
    {
        if(!_usePrism) try { _usePrism= prefs.GetEntry<bool>("forceUsePrism").Value; }catch { }
        if (!AllowPrism.HasFlag()) _usePrism = false;
        var types = AccessTools.GetTypesFromAssembly(MelonAssembly.Assembly);
        var devPatches = DevMode.HasFlag();
        foreach (var type in types)
        {
            if (type == null) continue;
            try
            {
                // Skip entire class if marked as a library patch and library disabled
                if (!_usePrism && type.GetCustomAttribute<PrismPatch>() != null) continue;
                // Skip entire class if marked as a dev patch and devmode disabled
                if(!devPatches && type.GetCustomAttribute<DevPatch>() != null) continue;
                var classPatches = HarmonyMethodExtensions.GetFromType(type);
                if (classPatches.Count > 0)
                {
                    var processor = HarmonyInstance.CreateClassProcessor(type);
                    processor.Patch();
                    //MelonLogger.Msg($"Patched class: {type.FullName}");
                }
            }
            catch (Exception e)
            {
                MelonLogger.Error(e);
                MelonLogger.Error($"Failed to patch {type.FullName}: {e.Message}");
            }
        }
    }

    internal static List<(string, string, string, string, Assembly,string)> brokenExpansions = new ();
    // Dont change name, it is called via reflection in order to hide it. 
    private static void AddBrokenExpansion(string name, string author, string version, string downloadlink, Assembly assembly,string errorMessage)
    {
        brokenExpansions.Add((name, author, version, downloadlink, assembly,errorMessage));
    }
    /// <summary>
    /// Loads an expansion, don't call manually, please use the standard .cs file
    /// </summary>
    /// <param name="expansionV3"></param>
    /// <returns></returns>
    public static bool LoadExpansion(SR2EExpansionV3 expansionV3)
    {
        StaticOnEarlyInitializeMelon();
        if (AllowExpansionsV3.HasFlag())
        {
            bool shouldUnregister = false;
            var attributes = expansionV3.MelonBase.MelonAssembly.Assembly.GetCustomAttributes<AssemblyMetadataAttribute>();
            var metadata = new Dictionary<string, string>();
            foreach (var _attribute in attributes) metadata.Add(_attribute.Key, _attribute.Value);
            if (!metadata.ContainsKey(SR2EExpansionAttributes.IsExpansion) || metadata[SR2EExpansionAttributes.IsExpansion].ToLower() != "true") shouldUnregister = true;
            else
            {
                var attributeUsePrism = !metadata.ContainsKey(SR2EExpansionAttributes.UsePrism) ? false : metadata[SR2EExpansionAttributes.UsePrism].ToLower() == "true";
                if (attributeUsePrism && !AllowPrism.HasFlag()) shouldUnregister = true;
                else
                {
                    if (attributeUsePrism) _usePrism = true;
                }
            }
            
            if (!shouldUnregister)
            {
                expansionsV3.Add(expansionV3);
                if(hasInitialized) expansionV3.OnInitializeMelon();
                return true;
            }
        }
        expansionV3.MelonBase.Unregister();
        return false;
    }

    private static bool hasInitialized = false;
    public override void OnInitializeMelon()
    {
        hasInitialized = true;
        prefs = MelonPreferences.CreateCategory("SR2E", "SR2E");
        string path = MelonAssembly.Assembly.Location + ".old";
        if (File.Exists(path)) File.Delete(path);
        RefreshPrefs();
        foreach (MelonBase melonBase in new List<MelonBase>(MelonBase.RegisteredMelons))
        {
            if (melonBase is SR2EExpansionV2)
                if (AllowExpansionsV2.HasFlag())
                {
                    var attribute = melonBase.MelonAssembly.Assembly.GetCustomAttribute<SR2EExpansionAttribute>();
                    if (attribute == null) melonBase.Unregister();
                    else
                    {
                        if(attribute.usePrism&&!AllowPrism.HasFlag()) melonBase.Unregister();
                        else
                        {
                            if (attribute.usePrism)
                                _usePrism = true;
                            expansionsV2.Add(melonBase as SR2EExpansionV2);
                        }
                    }
                }
                else melonBase.Unregister();
            
            if (melonBase is SR2EExpansionV1)
                if (AllowExpansionsV1.HasFlag()) expansionsV1V2.Add(melonBase as SR2EExpansionV1);
                else melonBase.Unregister();
        }

        InjectIl2CppComponents();
        PatchGame();

        Application.add_logMessageReceived(new Action<string, string, LogType>(AppLogUnity));
        try { AddLanguages(EmbeddedResourceEUtil.LoadString("translations.csv")); } catch (Exception e) { MelonLogger.Error(e); }

        foreach (var expansion in expansionsV1V2) try { expansion.OnNormalInitializeMelon(); } catch (Exception e) { MelonLogger.Error(e); }
        
        foreach (var expansion in expansionsV3) try { expansion.OnInitializeMelon(); } catch (Exception e) { MelonLogger.Error(e); }
    }
    public override void OnApplicationQuit()
    {
        try { if (systemContext.SceneLoader.IsCurrentSceneGroupGameplay()) autoSaveDirector.SaveGame(); }catch { }
        foreach (var expansion in expansionsV3) try { expansion.OnApplicationQuit(); } catch (Exception e) { MelonLogger.Error(e); }
        
    }

    internal static void CheckFallBackFont()
    {
        try
        {
            if (notoSansFont == null)
            {
                var settings = Get<TMP_Settings>("TMP Settings");
                if (settings == null) return;
                string tempPath = Path.Combine(TmpDataPath, "tmpFallbackFont.ttf");
                File.WriteAllBytes(tempPath, EmbeddedResourceEUtil.LoadResource("Assets.NotoSans.ttf"));
                Font tempFont = new Font(tempPath);
                notoSansFont = TMP_FontAsset.CreateFontAsset(tempFont);
                //settings.m_fallbackFontAssets.Add(fallBackFont);, creates issues for some reason :(
                settings.m_warningsDisabled = true;
            }
            foreach (var fontAsset in GetAll<TMP_FontAsset>())
            {
                if (fontAsset == notoSansFont) continue;
                if(!fontAsset.fallbackFontAssetTable.Contains(notoSansFont))
                    fontAsset.fallbackFontAssetTable.Add(notoSansFont);
            }
        }
        catch { }
    }
    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        CheckFallBackFont();
        if (DebugLogging.HasFlag()) MelonLogger.Msg("OnLoaded Scene: " + sceneName);

        try { SR2EWarpManager.OnSceneLoaded(); } catch (Exception e) { MelonLogger.Error(e); }
        if(sceneName=="StandaloneStart"||sceneName=="CompanyLogo"||sceneName=="LoadScene")
            try {
                if(MenuEUtil.isAnyMenuOpen) MenuEUtil.CloseOpenMenu(); 
                if (MenuEUtil.isAnyPopUpOpen); MenuEUtil.CloseOpenPopUps();
            } catch { }
        
        switch (sceneName)
        {
            case "MainMenuUI":
                SR2EVolumeProfileManager.OnMainMenuUILoad();
                //For some reason there are 2 configurations? And due to Il2CPP, just patching the Getter via Harmony isn't sufficient
                foreach (var configuration in GetAll<AutoSaveDirectorConfiguration>())
                    configuration._saveSlotCount = SAVESLOT_COUNT.Get();
                
                NativeEUtil.CustomTimeScale = 1f;
                Time.timeScale = 1;
                try
                {
                    var b = Get<ButtonBehaviorViewHolder>("SaveGameSlotButton");
                    ExecuteInTicks(() =>
                    {
                        var l = b.gameObject.GetObjectRecursively<LayoutElement>("Icon");
                        l.minWidth = l.preferredWidth;
                    },3);
                }
                catch (Exception e)
                {
                    MelonLogger.Error(e);
                }
                /*try
                {
                    var scroll = SR2EStuff.GetObjectRecursively<Scrollbar>("saveFilesSliderRec");
                    var styler = scroll.AddComponent<ScrollbarStyler>();
                    foreach (var sstyler in GetAll<ScrollbarStyler>())
                    {
                        if (sstyler._style == null) continue;
                        styler._style = sstyler._style;
                        scroll.colors = sstyler.GetComponent<Scrollbar>().colors;
                    }
                }
                catch (Exception e)
                {
                    MelonLogger.Error(e);
                    MelonLogger.Error("There was a problem applying styles to the save slider!");
                }*/
                break;
            case "ZoneCore": foreach (var expansion in expansionsV2) try { expansion.OnZoneCoreLoaded(); } catch (Exception e) { MelonLogger.Error(e); } break;
        }

        
        if (usePrism)  try { PrismShortcuts.OnSceneWasLoaded(buildIndex,sceneName); } catch (Exception e) { MelonLogger.Error(e); }
        
        switch (sceneName)
        {
            case "StandaloneEngagementPrompt": foreach (var expansion in expansionsV1V2) try { expansion.OnStandaloneEngagementPromptLoad(); }catch (Exception e) { MelonLogger.Error(e); } break;
            case "PlayerCore": foreach (var expansion in expansionsV1V2) try { expansion.OnPlayerCoreLoad(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "UICore": foreach (var expansion in expansionsV1V2) try { expansion.OnUICoreLoad(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "MainMenuUI": foreach (var expansion in expansionsV1V2) try { expansion.OnMainMenuUILoad(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "LoadScene": foreach (var expansion in expansionsV1V2) try { expansion.OnLoadSceneLoad(); } catch (Exception e) { MelonLogger.Error(e); } break;
        }
        switch (sceneName)
        {
            case "StandaloneEngagementPrompt": foreach (var expansion in expansionsV3) try { expansion.OnStandaloneEngagementPromptLoad(); }catch (Exception e) { MelonLogger.Error(e); } break;
            case "PlayerCore": foreach (var expansion in expansionsV3) try { expansion.OnPlayerCoreLoad(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "UICore": foreach (var expansion in expansionsV3) try { expansion.OnUICoreLoad(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "MainMenuUI": foreach (var expansion in expansionsV3) try { expansion.OnMainMenuUILoad(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "LoadScene": foreach (var expansion in expansionsV3) try { expansion.OnLoadSceneLoad(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "ZoneCore": foreach (var expansion in expansionsV3) try { expansion.OnZoneCoreLoaded(); } catch (Exception e) { MelonLogger.Error(e); } break;
        }
        foreach (var expansion in expansionsV3) try { expansion.OnSceneWasLoaded(buildIndex, sceneName); } catch (Exception e) { MelonLogger.Error(e); }

        SR2ECommandManager.OnSceneWasLoaded(buildIndex, sceneName);
    }

    internal static void CheckForTime()
    {
        if (!inGame) return;
        try
        {
            if(Time.timeScale!=0&&Time.timeScale != NativeEUtil.CustomTimeScale) 
                Time.timeScale = NativeEUtil.CustomTimeScale; 
        } catch {}
        ExecuteInSeconds(() => { CheckForTime();}, 1);
    }
    internal static void SendFontError(string name)
    {
        MelonLogger.Error($"The font '{name}' couldn't be loaded!");
    }
    internal static void SetupFonts()
    {
        if (SR2Font == null) SR2Font = FontEUtil.FontFromGame("Runsell Type - HemispheresCaps2");
        if (regularFont == null) regularFont = FontEUtil.FontFromGame("Lexend-Regular (Latin)"); 
        if (boldFont == null) boldFont = FontEUtil.FontFromGame("Lexend-Bold (Latin)"); 
        if (normalFont == null) normalFont = FontEUtil.FontFromOS("Tahoma"); 
        foreach (var expansion in expansionsV1V2) try { expansion.OnSR2FontLoad(); }catch (Exception e) { MelonLogger.Error(e); }
        foreach (var pair in menus) pair.Key.ReloadFont();
    }


    

    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        if (DebugLogging.HasFlag()) MelonLogger.Msg("WasInitialized Scene: " + sceneName);
        if (usePrism)  try { PrismShortcuts.OnSceneWasInitialized(buildIndex,sceneName); } catch (Exception e) { MelonLogger.Error(e); }
        if (sceneName == "MainMenuUI") mainMenuLoaded = true;
        
        switch (sceneName)
        {
            case "StandaloneEngagementPrompt": foreach (var expansion in expansionsV1V2) try { expansion.OnStandaloneEngagementPromptInitialize(); }catch (Exception e) { MelonLogger.Error(e); } break;
            case "PlayerCore": foreach (var expansion in expansionsV1V2) try { expansion.OnPlayerCoreInitialize(); }catch (Exception e) { MelonLogger.Error(e); } break;
            case "UICore": foreach (var expansion in expansionsV1V2) try { expansion.OnUICoreInitialize(); }catch (Exception e) { MelonLogger.Error(e); } break;
            case "MainMenuUI": foreach (var expansion in expansionsV1V2) try { expansion.OnMainMenuUIInitialize(); }catch (Exception e) { MelonLogger.Error(e); } break;
            case "LoadScene": foreach (var expansion in expansionsV1V2) try { expansion.OnLoadSceneInitialize(); }catch (Exception e) { MelonLogger.Error(e); } break;
            case "ZoneCore": foreach (var expansion in expansionsV1V2) try { expansion.OnZoneCoreInitialized(); }catch (Exception e) { MelonLogger.Error(e); } break;
        }
        switch (sceneName)
        {
            case "StandaloneEngagementPrompt": foreach (var expansion in expansionsV3) try { expansion.OnStandaloneEngagementPromptInitialize(); }catch (Exception e) { MelonLogger.Error(e); } break;
            case "PlayerCore": foreach (var expansion in expansionsV3) try { expansion.OnPlayerCoreInitialize(); }catch (Exception e) { MelonLogger.Error(e); } break;
            case "UICore": foreach (var expansion in expansionsV3) try { expansion.OnUICoreInitialize(); }catch (Exception e) { MelonLogger.Error(e); } break;
            case "MainMenuUI": foreach (var expansion in expansionsV3) try { expansion.OnMainMenuUIInitialize(); }catch (Exception e) { MelonLogger.Error(e); } break;
            case "LoadScene": foreach (var expansion in expansionsV3) try { expansion.OnLoadSceneInitialize(); }catch (Exception e) { MelonLogger.Error(e); } break;
            case "ZoneCore": foreach (var expansion in expansionsV3) try { expansion.OnZoneCoreInitialized(); }catch (Exception e) { MelonLogger.Error(e); } break;
        }
        
        foreach (var expansion in expansionsV3) try { expansion.OnSceneWasInitialized(buildIndex, sceneName); } catch (Exception e) { MelonLogger.Error(e); }
        SR2ECommandManager.OnSceneWasInitialized(buildIndex, sceneName);
    }

    public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
    {
        if (DebugLogging.HasFlag()) MelonLogger.Msg("OnUnloaded Scene: " + sceneName);
        if (sceneName == "MainMenuUI") mainMenuLoaded = false;
        
        switch (sceneName)
        {               
            case "StandaloneEngagementPrompt": foreach (var expansion in expansionsV1V2) try { expansion.OnStandaloneEngagementPromptUnload(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "PlayerCore": foreach (var expansion in expansionsV1V2) try { expansion.OnPlayerCoreUnload(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "UICore": foreach (var expansion in expansionsV1V2) try { expansion.OnUICoreUnload(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "MainMenuUI": foreach (var expansion in expansionsV1V2) try { expansion.OnMainMenuUIUnload(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "LoadScene": foreach (var expansion in expansionsV1V2) try { expansion.OnLoadSceneUnload(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "ZoneCore": foreach (var expansion in expansionsV1V2) try { expansion.OnZoneCoreUnloaded(); } catch (Exception e) { MelonLogger.Error(e); } break;
        }
        switch (sceneName)
        {               
            case "StandaloneEngagementPrompt": foreach (var expansion in expansionsV3) try { expansion.OnStandaloneEngagementPromptUnload(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "PlayerCore": foreach (var expansion in expansionsV3) try { expansion.OnPlayerCoreUnload(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "UICore": foreach (var expansion in expansionsV3) try { expansion.OnUICoreUnload(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "MainMenuUI": foreach (var expansion in expansionsV3) try { expansion.OnMainMenuUIUnload(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "LoadScene": foreach (var expansion in expansionsV3) try { expansion.OnLoadSceneUnload(); } catch (Exception e) { MelonLogger.Error(e); } break;
            case "ZoneCore": foreach (var expansion in expansionsV3) try { expansion.OnZoneCoreUnloaded(); } catch (Exception e) { MelonLogger.Error(e); } break;
        }
        SR2ECommandManager.OnSceneWasUnloaded(buildIndex, sceneName);
        foreach (var expansion in expansionsV3) try { expansion.OnSceneWasUnloaded(buildIndex, sceneName); } catch (Exception e) { MelonLogger.Error(e); }

    }


    public override void OnUpdate()
    {
        try {
            foreach (BaseUI ui in new List<BaseUI>(baseUIAddSliders))
            {
                if (ui)
                {
                    GameObject scrollView = ui.gameObject.GetObjectRecursively<GameObject>("ButtonsScrollView");
                    if (scrollView != null)
                    {
                        ScrollRect rect = scrollView.GetComponent<ScrollRect>();
                        if (rect.verticalScrollbar == null)
                        {
                            rect.vertical = true;
                            Scrollbar scrollBar = GameObject.Instantiate(SR2EStuff.GetObjectRecursively<Scrollbar>("saveFilesSliderRec"),
                                rect.transform);
                            rect.verticalScrollbar = scrollBar;
                            rect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
                            scrollBar.GetComponent<RectTransform>().localPosition += new Vector3(Screen.width / 250f, 0, 0);
                        }
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
                                    child.AddComponent(pair.Value);
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
                    SR2ERepoManager.Start();
                    SR2EStuff = obj;
                    obj.name = "SR2EStuff";
                    obj.tag = "";
                    obj.SetActive(false);
                    GameObject.DontDestroyOnLoad(obj);
                    foreach (var type in MelonAssembly.Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(SR2EMenu)) && !t.IsAbstract))
                    {
                        try
                        {
                            var identifier = type.GetMenuIdentifierByType();
                            if (!string.IsNullOrWhiteSpace(identifier.saveKey))
                            {
                                var asset = SystemContextPatch.bundle.LoadAsset(SystemContextPatch.getMenuPath(identifier));
                                var Object = GameObject.Instantiate(asset, obj.transform);
                                menusToInit.Add(Object.name, type);
                                if(!ClassInjector.IsTypeRegisteredInIl2Cpp(type))
                                    ClassInjector.RegisterTypeInIl2Cpp(type, new RegisterTypeOptions() { LogSuccess = false });
                                
                            }
                            else MelonLogger.Error($"The menu under the name {type.Name} couldn't be loaded! It's MenuIdentifier is broken!");

                        }catch (Exception e) { MelonLogger.Error(e); }
                    }
                }
            }
            else
            {
                
                try { if (SR2EConsole.openKey.OnKeyDown()||SR2EConsole.openKey2.OnKeyDown()) MenuEUtil.GetMenu<SR2EConsole>().Toggle(); } catch (Exception e) { MelonLogger.Error(e); }
                try { SR2ECommandManager.Update(); } catch (Exception e) { MelonLogger.Error(e); }
                try { SR2EBindingManger.Update(); } catch (Exception e) { MelonLogger.Error(e); }
                if (DevMode.HasFlag()) SR2EDebugUI.DebugStatsManager.Update();
                foreach (var pair in menus) try { pair.Key.AlwaysUpdate(); } catch (Exception e) { MelonLogger.Error(e); }
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
        } catch (Exception e) { MelonLogger.Error(e); }
        foreach (var expansion in expansionsV3) try { expansion.OnUpdate(); } catch (Exception e) { MelonLogger.Error(e); }
    }


    //Forwarder for V3 Expansions
    public override void OnPreSupportModule()
    { foreach (var expansion in expansionsV3) try { expansion.OnPreSupportModule(); } catch (Exception e) { MelonLogger.Error(e); } }
    public override void OnFixedUpdate()
    { foreach (var expansion in expansionsV3) try { expansion.OnFixedUpdate(); } catch (Exception e) { MelonLogger.Error(e); } }
    public override void OnGUI()
    { foreach (var expansion in expansionsV3) try { expansion.OnGUI(); } catch (Exception e) { MelonLogger.Error(e); } }
    public override void OnLateUpdate()
    { foreach (var expansion in expansionsV3) try { expansion.OnLateUpdate(); } catch (Exception e) { MelonLogger.Error(e); } }
    public override void OnPreferencesLoaded()
    { foreach (var expansion in expansionsV3) try { expansion.OnPreferencesLoaded(); } catch (Exception e) { MelonLogger.Error(e); } }
    public override void OnPreferencesLoaded(string filepath)
    { foreach (var expansion in expansionsV3) try { expansion.OnPreferencesLoaded(filepath); } catch (Exception e) { MelonLogger.Error(e); } }
    public override void OnPreferencesSaved()
    { foreach (var expansion in expansionsV3) try { expansion.OnPreferencesSaved(); } catch (Exception e) { MelonLogger.Error(e); } }
    public override void OnPreferencesSaved(string filepath)
    { foreach (var expansion in expansionsV3) try { expansion.OnPreferencesSaved(filepath); } catch (Exception e) { MelonLogger.Error(e); } }
}
