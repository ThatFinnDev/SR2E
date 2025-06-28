namespace SR2E.Enums.Features;

public enum FeatureFlag
{
    None,
    //Dev
    /// <summary>
    /// Activates Devmode
    /// </summary>
    DevMode,
    /// <summary>
    /// Shows extra info in into MLLog
    /// </summary>
    DebugLogging,
    /// <summary>
    /// Show Unity related errors from Debug.Log, Debug.Warning and Debug.Error
    /// </summary>
    ShowUnityErrors,
    /// <summary>
    /// Show save game related errors and skips them
    /// </summary>
    IgnoreSaveErrors,
    /// <summary>
    /// Enable experimental features
    /// </summary>
    Experiments,
    /// <summary>
    /// Enable the settings injection feature
    /// </summary>
    ExperimentalSettingsInjection,
    /// <summary>
    /// Enable the save export feature
    /// </summary>
    ExperimentalSaveExport,
    /// <summary>
    /// Enable the enable dev key codes
    /// </summary>
    ExperimentalKeyCodes,
    
    //Commands+Dev
    /// <summary>
    /// Load dev-only commands
    /// </summary>
    CommandsLoadDevOnly, 
    /// <summary>
    /// Enable experimental commands
    /// </summary>
    CommandsLoadExperimental, 
    
    //Commands
    /// <summary>
    /// Load commands
    /// </summary>
    CommandsLoadCommands, //
    /// <summary>
    /// Load cheating commands
    /// </summary>
    CommandsLoadCheat, //
    /// <summary>
    /// Load binding commands
    /// </summary>
    CommandsLoadBinding, //
    /// <summary>
    /// Load warping commands
    /// </summary>
    CommandsLoadWarp, //
    /// <summary>
    /// Load common commands
    /// </summary>
    CommandsLoadCommon, //
    /// <summary>
    /// Load menu commands
    /// </summary>
    CommandsLoadMenu, //
    /// <summary>
    /// Load miscellaneous commands
    /// </summary>
    CommandsLoadMiscellaneous, //
    /// <summary>
    /// Load fun commands
    /// </summary>
    CommandsLoadFun, //

    //Cheats and Mods
    /// <summary>
    /// Disable everything related to cheats
    /// </summary>
    DisableCheats,
    /// <summary>
    /// Allow adding the cheat menu button to the pause menu
    /// </summary>
    AddCheatMenuButton, //
    /// <summary>
    /// Enable infhealth command and its cheat menu equivalent
    /// </summary>
    EnableInfHealth, //
    /// <summary>
    /// Enable infenergy command and its cheat menu equivalent
    /// </summary>
    EnableInfEnergy, //
    
    //Misc
    /// <summary>
    /// Allow adding the mod menu button to the pause menu and main menu
    /// </summary>
    AddModMenuButton, //
    /// <summary>
    /// Allow loading and integrating SR2E expansions
    /// </summary>
    AllowExpansions, //
    /// <summary>
    /// Modify the version text in the main menu
    /// </summary>
    EnableLocalizedVersionPatch, //
    /// <summary>
    /// Inject Translations into unity's localization system
    /// </summary>
    InjectTranslations, //
    /// <summary>
    /// Use the patch for il2cpp exceptions
    /// </summary>
    EnableIl2CppDetourExceptionReporting, //
    
    //Menus
    /// <summary>
    /// Enable the mod menu
    /// </summary>
    EnableModMenu, //
    /// <summary>
    /// Enable the cheat menu
    /// </summary>
    EnableCheatMenu, //
    /// <summary>
    /// Enable the console menu
    /// </summary>
    EnableConsole, //
    /// <summary>
    /// Enable the theme menu
    /// </summary>
    EnableThemeMenu, //
    /// <summary>
    /// Enable the repo menu
    /// </summary>
    EnableRepoMenu, //
    
    //UI
    /// <summary>
    /// Inject buttons in the main menu
    /// </summary>
    InjectMainMenuButtons, //
    /// <summary>
    /// Inject buttons in the ranch house ui
    /// </summary>
    InjectRanchUIButtons, //
    /// <summary>
    /// Inject buttons in the pause menu
    /// </summary>
    InjectPauseButtons, //

    //Updates and Patches
    /// <summary>
    /// Allow checking for new SR2E updates
    /// </summary>
    CheckForUpdates, //
    /// <summary>
    /// Allow auto updating if an update has been found
    /// </summary>
    AllowAutoUpdate, //

}