using Il2CppMonomiPark.SlimeRancher;
using SR2E.Saving;
using SR2E.Storage;

namespace SR2E.Expansion;

public abstract class SR2EExpansionV3
{
    protected SR2EExpansionV3() {}
    // Dont change name, it is called via reflection in order to hide it. 
    public MelonBase MelonBase=>_melonBase;
    private MelonBase _melonBase;
    
    #region MelonLoader Native Events
    /// <summary>
    /// Runs before Support Module Initialization and after Assembly Generation for Il2Cpp Games.<br />
    /// Forwarded from ML
    /// </summary>
    public virtual void OnPreSupportModule() { }


    /// <summary>
    /// Runs after the Melon has registered. This callback waits until MelonLoader has fully initialized (<see cref="MelonEvents.OnApplicationStart"/>).<br />
    /// Forwarded from ML
    /// </summary>
    public virtual void OnInitializeMelon() { }

    /// <summary>
    /// Runs after <see cref="OnInitializeMelon"/>. This callback waits until Unity has invoked the first 'Start' messages (<see cref="MelonEvents.OnApplicationLateStart"/>).<br />
    /// Forwarded from ML
    /// </summary>
    public virtual void OnLateInitializeMelon() { }

    /// <summary>
    /// Runs when the Melon is unregistered. Also runs before the Application is closed (<see cref="MelonEvents.OnApplicationDefiniteQuit"/>).<br />
    /// Forwarded from ML
    /// </summary>
    public virtual void OnDeinitializeMelon() { }
    
    /// <summary>
    /// Runs once per frame.<br />
    /// Forwarded from ML
    /// </summary>
    public virtual void OnUpdate() { }

    /// <summary>
    /// Can run multiple times per frame. Mostly used for Physics.<br />
    /// Forwarded from ML
    /// </summary>
    public virtual void OnFixedUpdate() { }

    /// <summary>
    /// Runs once per frame, after <see cref="OnUpdate"/>.<br />
    /// Forwarded from ML
    /// </summary>
    public virtual void OnLateUpdate() { }

    /// <summary>
    /// Can run multiple times per frame. Mostly used for Unity's IMGUI.<br />
    /// Forwarded from ML
    /// </summary>
    public virtual void OnGUI() { }

    /// <summary>
    /// Runs on a quit request. It is possible to abort the request in this callback.<br />
    /// Forwarded from ML
    /// </summary>
    public virtual void OnApplicationQuit() { }

    /// <summary>
    /// Runs when Melon Preferences get saved.<br />
    /// Forwarded from ML
    /// </summary>
    public virtual void OnPreferencesSaved() { }

    /// <summary>
    /// Runs when Melon Preferences get saved. Gets passed the Preferences's File Path.<br />
    /// Forwarded from ML
    /// </summary>
    public virtual void OnPreferencesSaved(string filepath) { }

    /// <summary>
    /// Runs when Melon Preferences get loaded.<br />
    /// Forwarded from ML
    /// </summary>
    public virtual void OnPreferencesLoaded() { }

    /// <summary>
    /// Runs when Melon Preferences get loaded. Gets passed the Preferences's File Path.<br />
    /// Forwarded from ML
    /// </summary>
    public virtual void OnPreferencesLoaded(string filepath) { }
    
    /// <summary>
    /// Runs when a new Scene is loaded.<br />
    /// Forwarded from ML
    /// </summary>
    public virtual void OnSceneWasLoaded(int buildIndex, string sceneName) { }

    /// <summary>
    /// Runs once a Scene is initialized.<br />
    /// Forwarded from ML
    /// </summary>
    public virtual void OnSceneWasInitialized(int buildIndex, string sceneName) { }

    /// <summary>
    /// Runs once a Scene unloads.<br />
    /// Forwarded from ML
    /// </summary>
    public virtual void OnSceneWasUnloaded(int buildIndex, string sceneName) { }
    #endregion
    
    
    
    
    #region SR2E Scene Events
    
    /// <summary>
    /// Gets executed every time the scene "StandaloneEngagementPrompt" gets loaded.
    /// </summary>
    public virtual void OnStandaloneEngagementPromptLoad() { }
    /// <summary>
    /// Gets executed every time the scene "StandaloneEngagementPrompt" gets unloaded.
    /// </summary>
    public virtual void OnStandaloneEngagementPromptUnload() { }
    /// <summary>
    /// Gets executed every time the scene "StandaloneEngagementPrompt" gets initialized.
    /// </summary>
    public virtual void OnStandaloneEngagementPromptInitialize() { }
    
    /// <summary>
    /// Gets executed every time the scene "PlayerCore" gets loaded.
    /// </summary>
    public virtual void OnPlayerCoreLoad() { }
    /// <summary>
    /// Gets executed every time the scene "PlayerCore" gets unloaded.
    /// </summary>
    public virtual void OnPlayerCoreUnload() { }
    /// <summary>
    /// Gets executed every time the scene "PlayerCore" gets initialized.
    /// </summary>
    public virtual void OnPlayerCoreInitialize() { }
    
    /// <summary>
    /// Gets executed every time the scene "UICore" gets loaded.
    /// </summary>
    public virtual void OnUICoreLoad() { }
    /// <summary>
    /// Gets executed every time the scene "UICore" gets unloaded.
    /// </summary>
    public virtual void OnUICoreUnload() { }
    /// <summary>
    /// Gets executed every time the scene "UICore" gets initialized.
    /// </summary>
    public virtual void OnUICoreInitialize() { }
    
    /// <summary>
    /// Gets executed every time the scene "MainMenuUI" gets loaded.
    /// </summary>
    public virtual void OnMainMenuUILoad() { }
    /// <summary>
    /// Gets executed every time the scene "MainMenuUI" gets unloaded.
    /// </summary>
    public virtual void OnMainMenuUIUnload() { }
    /// <summary>
    /// Gets executed every time the scene "MainMenuUI" gets initialized.
    /// </summary>
    public virtual void OnMainMenuUIInitialize() { }
    
    /// <summary>
    /// Gets executed every time the scene "LoadScene" gets loaded.
    /// </summary>
    public virtual void OnLoadSceneLoad() { }
    /// <summary>
    /// Gets executed every time the scene "LoadScene" gets unloaded.
    /// </summary>
    public virtual void OnLoadSceneUnload() { }
    /// <summary>
    /// Gets executed every time the scene "LoadScene" gets initialized.
    /// </summary>
    public virtual void OnLoadSceneInitialize() { }
    
    /// <summary>
    /// Gets executed every time the scene "ZoneCore" gets loaded.
    /// </summary>
    public virtual void OnZoneCoreLoaded() { }
    /// <summary>
    ///   Gets executed every time the scene "ZoneCore" gets initialized.
    /// </summary>
    public virtual void OnZoneCoreInitialized() { }
    /// <summary>
    /// Gets executed every time the scene "ZoneCore" gets unloaded.
    /// </summary>
    public virtual void OnZoneCoreUnloaded() { }
    #endregion
    
    
    
    
    #region SR2E Context & Directors Events
    /// <summary>
    /// Gets executed once SystemContext loads. In Postfix of the Start method
    /// </summary>
    public virtual void AfterSystemContext(SystemContext systemContext) { }
    
    /// <summary>
    /// Gets executed once GameContext loads. In Postfix of the Start method
    /// </summary>
    public virtual void AfterGameContext(GameContext gameContext) { }
    
    /// <summary>
    /// Gets executed once SceneContext loads. In Postfix of the Start method
    /// </summary>
    public virtual void AfterSceneContext(SceneContext sceneContext) { }
    
    /// <summary>
    /// Get's called before the AutoSaveDirector has been loaded
    /// </summary>
    public virtual void BeforeSaveDirectorLoaded(AutoSaveDirector saveDirector) { }
    
    /// <summary>
    /// Get's called after the AutoSaveDirector has been loaded<br />
    /// You should use this method to add your translations to SR2<br />
    /// and to add your own buttons.
    /// </summary>
    public virtual void AfterSaveDirectorLoaded(AutoSaveDirector saveDirector) { }
    #endregion
    
    
    
    #region SR2E Save Files
    
    /// <summary>
    /// Loads the custom SaveRoot from a save file<br />
    /// Check the type before proceeding<br />
    /// This gets executed as soon as the save file has been loaded<br />
    /// Even before the game has spawned all it's actors etc.
    /// </summary>
    public virtual void OnEarlyCustomSaveDataReceived(RootSave saveRoot, LoadingGameSessionData loadingGameSessionData) { }

    /// <summary>
    /// Same as OnEarlyCustomSaveDataReceived but only gets called if this mod has no custom save data<br />
    /// This gets executed as soon as the save file has been loaded<br />
    /// Even before the game has spawned all it's actors etc.
    /// </summary>
    public virtual void OnEarlyNoCustomSaveDataReceived(LoadingGameSessionData loadingGameSessionData) { }
    
    
    
    /// <summary>
    /// Loads the custom SaveRoot from a save file<br />
    /// Check the type before proceeding<br />
    /// </summary>
    public virtual void OnCustomSaveDataReceived(RootSave saveRoot, LoadingGameSessionData loadingGameSessionData) { }

    /// <summary>
    /// Same as OnCustomSaveDataReceived but only gets called if this mod has no custom save data
    /// </summary>
    public virtual void OnNoCustomSaveDataReceived(LoadingGameSessionData loadingGameSessionData) { }
    
    
    /// <summary>
    /// Gets executed everytime a save file is being saved<br/>
    /// Return null if you don't use custom save data
    /// </summary>
    /// <returns>RootSave</returns>
    public virtual RootSave OnSaveCustomSaveData(SavingGameSessionData savingGameSessionData) { return null; }
    #endregion
    
    #region SR2E
    
    /// <summary>
    /// Gets executed once all commands get registered.<br />
    /// You should use this method to register your own commands,<br />
    /// with the SR2ECommand.CommandType.DontLoad flag,<br />
    /// by using SR2EConsole.RegisterCommand(new YourCommand()).
    /// </summary>
    public virtual void LoadCommands() { }
    
    #endregion
    
    
    
    
    #region Prism Events
    /// <summary>
    /// In this function you should add all of your base slimes, veggies, toys etc.<br />
    /// Requires Prism
    /// </summary>
    public virtual void OnPrismCreateAdditions() { }
    
    /// <summary>
    /// Use this if you want to do stuff with every e.g slime, veggie etc.<br />
    /// DO NOT add objects here, do that in <c>OnPrismCreateAdditions</c><br />
    /// This gets called after every mod ran OnPrismCreateAdditions()<br />
    /// Requires Prism
    /// </summary>
    public virtual void AfterPrismCreateAdditions() { }
    
    /// <summary>
    /// This gets called after all largos have been created<br />
    /// Requires Prism
    /// </summary>
    public virtual void AfterPrismLargosCreated() { }
    #endregion
}