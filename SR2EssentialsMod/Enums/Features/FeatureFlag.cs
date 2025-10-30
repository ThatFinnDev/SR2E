namespace SR2E.Enums.Features;

public enum FeatureFlag
{
    None=0,
    //Dev
    /// <summary>
    /// Activates Devmode
    /// </summary>
    DevMode=10,
    /// <summary>
    /// Shows extra info in into MLLog
    /// </summary>
    DebugLogging=20,
    /// <summary>
    /// Show Unity related errors from Debug.Log, Debug.Warning and Debug.Error
    /// </summary>
    ShowUnityErrors=30,
    /// <summary>
    /// Show save game related errors and skips them
    /// </summary>
    IgnoreSaveErrors=40,
    /// <summary>
    /// Enable experimental features
    /// </summary>
    Experiments=50,
    /// <summary>
    /// Allow experimental Library
    /// </summary>
    AllowExperimentalLibrary=60,
    /// <summary>
    /// Enable the enable dev key codes
    /// </summary>
    ExperimentalKeyCodes=80,
    
    //Commands+Dev
    /// <summary>
    /// Load dev-only commands
    /// </summary>
    CommandsLoadDevOnly=90, 
    /// <summary>
    /// Enable experimental commands
    /// </summary>
    CommandsLoadExperimental=100, 
    
    //Commands
    /// <summary>
    /// Load commands
    /// </summary>
    CommandsLoadCommands=110, //
    /// <summary>
    /// Load cheating commands
    /// </summary>
    CommandsLoadCheat=120, //
    /// <summary>
    /// Load binding commands
    /// </summary>
    CommandsLoadBinding=130, //
    /// <summary>
    /// Load warping commands
    /// </summary>
    CommandsLoadWarp=140, //
    /// <summary>
    /// Load common commands
    /// </summary>
    CommandsLoadCommon=150, //
    /// <summary>
    /// Load menu commands
    /// </summary>
    CommandsLoadMenu=160, //
    /// <summary>
    /// Load miscellaneous commands
    /// </summary>
    CommandsLoadMiscellaneous=170, //
    /// <summary>
    /// Load fun commands
    /// </summary>
    CommandsLoadFun=180, //

    //Cheats and Mods
    /// <summary>
    /// Disable everything related to cheats
    /// </summary>
    DisableCheats=190,
    /// <summary>
    /// Allow adding the cheat menu button to the pause menu
    /// </summary>
    AddCheatMenuButton=200, //
    /// <summary>
    /// Enable infhealth command and its cheat menu equivalent
    /// </summary>
    EnableInfHealth=210, //
    /// <summary>
    /// Enable infenergy command and its cheat menu equivalent
    /// </summary>
    EnableInfEnergy=220, //
    
    //Misc
    /// <summary>
    /// Allow adding the mod menu button to the pause menu and main menu
    /// </summary>
    AddModMenuButton=230, //
    /// <summary>
    /// Allow loading and integrating SR2E expansions
    /// </summary>
    AllowExpansions=240, //
    /// <summary>
    /// Modify the version text in the main menu
    /// </summary>
    EnableLocalizedVersionPatch=250, //
    /// <summary>
    /// Inject Translations into unity's localization system
    /// </summary>
    InjectTranslations=260, //
    /// <summary>
    /// Use the patch for il2cpp exceptions
    /// </summary>
    EnableIl2CppDetourExceptionReporting=270, //
    /// <summary>
    /// Enable the save export feature
    /// </summary>
    AllowSaveExport=70, //
    
    //Menus
    /// <summary>
    /// Enable the mod menu
    /// </summary>
    EnableModMenu=280, //
    /// <summary>
    /// Enable the cheat menu
    /// </summary>
    EnableCheatMenu=290, //
    /// <summary>
    /// Enable the console menu
    /// </summary>
    EnableConsole=300, //
    /// <summary>
    /// Enable the theme menu
    /// </summary>
    EnableThemeMenu=310, //
    /// <summary>
    /// Enable the repo menu
    /// </summary>
    EnableRepoMenu=320, //
    
    //UI
    /// <summary>
    /// Inject buttons in the main menu
    /// </summary>
    InjectMainMenuButtons=330, //
    /// <summary>
    /// Inject buttons in the ranch house ui
    /// </summary>
    InjectRanchUIButtons=340, //
    /// <summary>
    /// Inject buttons in the pause menu
    /// </summary>
    InjectPauseButtons=350, //

    //Updates and Patches
    /// <summary>
    /// Allow checking for new SR2E updates
    /// </summary>
    CheckForUpdates=360, //
    /// <summary>
    /// Allow auto updating if an update has been found
    /// </summary>
    AllowAutoUpdate=370, //
    /// <summary>
    /// Make SystemContext.isModded true
    /// </summary>
    ChangeSystemContextIsModded=380, //

}