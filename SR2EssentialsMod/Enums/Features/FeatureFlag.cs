using System;

namespace SR2E.Enums.Features;

public enum FeatureFlag
{
    //Off limits: 70
    
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
    /// Shows world populator errors
    /// </summary>
    ShowWorldPopulatorErrors=41,
    /// <summary>
    /// Tries to ignore world populator errors
    /// </summary>
    IgnoreWorldPopulatorErrors=42,
    /// <summary>
    /// Enable experimental features
    /// </summary>
    Experiments=50,
    /// <summary>
    /// Allow prism
    /// </summary>
    AllowPrism=60,
    /// <summary>
    /// Enable the enable dev key codes
    /// </summary>
    ExperimentalKeyCodes=80,
    /// <summary>
    /// Add test repo
    /// </summary>
    UseMockRepo=81,
    
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
    /// Allow loading and integrating SR2E expansions
    /// </summary>
    AllowExpansionsV1=241, //
    /// <summary>
    /// Allow loading and integrating SR2E expansions of V1
    /// </summary>
    AllowExpansionsV2=242, //
    /// <summary>
    /// Allow loading and integrating SR2E expansions of V2
    /// </summary>
    AllowExpansionsV3=243, //
    /// <summary>
    /// Modify the version text in the main menu of V3
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
    /// Inject test buttons in the main menu
    /// </summary>
    AddMockMainMenuButtons=331, //
    [Obsolete("Please use "+nameof(AddMockMainMenuButtons),true)]AddTestButtons=331, //
    /// <summary>
    /// Inject buttons in the ranch house ui
    /// </summary>
    InjectRanchUIButtons=340, //
    /// <summary>
    /// Inject buttons in the pause menu
    /// </summary>
    InjectPauseButtons=350, //
    /// <summary>
    /// Inject buttons in the options ui
    /// </summary>
    InjectOptionsButtons=355,
    /// <summary>
    /// Inject test buttons in the OptionsUI
    /// </summary>
    AddMockOptionsUIButtons=356, //

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
    /// <summary>
    /// Redirects the StorageProvider to a FileStorgeProvider
    /// As a result the save path will be changed
    /// It will be redirected to the SR2E folder
    /// </summary>
    RedirectSaveFiles=390, //
    /// <summary>
    /// Tries to restore Debug Abilities.<br/>
    /// This isn't a faithful recreation of the true SR2 Debug tools.<br/>
    /// However there are enough breadcrumbs in the code which.<br/>
    /// help to make sense of the missing code and try to implement it as best as possible.<br/>
    /// You need to enable the individual abilities
    /// </summary>
    RestoreDebugAbilities=400, //
    /// <summary>
    /// Tries to restore Debug Ability.<br/>
    /// This restore the FPS counter
    /// </summary>
    RestoreDebugFPSViewer=401, //
    /// <summary>
    /// Tries to restore Debug Ability.<br/>
    /// This restore the PlayerDebug ui
    /// </summary>
    RestoreDebugPlayerDebug=402, //
    /// <summary>
    /// Tries to restore Debug Ability.<br/>
    /// This restore the unity dev console
    /// </summary>
    RestoreDebugDevConsole=403, //
    /// <summary>
    /// Tries to restore Debug Ability.<br/>
    /// This restore the DebugUI
    /// </summary>
    RestoreDebugDebugUI=404, //
    /// <summary>
    /// This fixes loading SceneGroups with invalid scene references<br/>
    /// If it stumbles across an invalid scene, it will be skipped
    /// </summary>
    TryFixingInvalidSceneGroups=410, //
    
    

}