using System;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppTMPro;

namespace SR2E.Expansion;

[Obsolete("OBSOLETE!: Please use SR2EExpansionV2.")]
public abstract class SR2EExpansionV1 : MelonMod
{
    public static TMP_FontAsset sr2Font => SR2E.SR2EEntryPoint.SR2Font;
    /// <summary>
    /// Replacement for OnInitializeMelon.
    /// </summary>
    public virtual void OnNormalInitializeMelon() { }
    /// <summary>
    /// This method is sealed! Use OnNormalInitializeMelon instead!
    /// </summary>
    public sealed override void OnInitializeMelon() { }
    /// <summary>
    /// Gets executed once SR2's own font has been loaded.
    /// The TMP_FontAsset is called sr2Font.
    /// </summary>
    public virtual void OnSR2FontLoad() { }
    /// <summary>
    /// Gets executed once the AutoSaveDirector is loading.
    /// </summary>
    public virtual void OnSaveDirectorLoading(AutoSaveDirector autoSaveDirector) {}

    /// <summary>
    /// Gets executed once the AutoSaveDirector has finished loading.
    /// You should use this method to add your translations to SR2
    /// and to add your own buttons.
    /// </summary>
    public virtual void SaveDirectorLoaded(AutoSaveDirector autoSaveDirector) {}
    /// <summary>
    /// Gets executed once all commands get registered.
    /// You should use this method to register your own commands,
    /// with the SR2ECommand.CommandType.DontLoad flag,
    /// by using SR2EConsole.RegisterCommand(new YourCommand()).
    /// </summary>
    public virtual void LoadCommands() { }
    
    /// <summary>
    /// Gets executed once SystemContext loads. In Postfix of the Start method
    /// </summary>
    public virtual void OnSystemContext(SystemContext systemContext) { }
    
    /// <summary>
    /// Gets executed every time the scene "StandaloneEngagementPrompt" gets loaded.
    /// </summary>
    public virtual void OnStandaloneEngagementPromptLoad() { }
    /// <summary>
    /// Gets executed every time the scene "StandaloneEngagementPrompt" gets unloaded.
    /// </summary>
    public virtual void OnStandaloneEngagementPromptUnload() { }
    /// <summary>
    /// Gets executed every time the scene "ZoneCore" gets unloaded.
    /// </summary>
    public virtual void OnZoneCoreUnloaded() { }
    
    
    /// <summary>
    /// Gets executed every time the scene "StandaloneEngagementPrompt" gets initialized.
    /// </summary>
    public virtual void OnStandaloneEngagementPromptInitialize() { }
    
    /// <summary>
    /// Gets executed once GameContext loads. In Postfix of the Start method
    /// </summary>
    public virtual void OnGameContext(GameContext gameContext) { }
    
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
    /// Gets executed every time the scene "MainMenuUI" gets initialized.
    /// </summary>
    public virtual void OnMainMenuUILoad() { }
    /// <summary>
    /// Gets executed every time the scene "MainMenuUI" gets loaded.
    /// </summary>
    public virtual void OnMainMenuUIUnload() { }
    /// <summary>
    /// Gets executed every time the scene "MainMenuUI" gets unloaded.
    /// </summary>
    public virtual void OnMainMenuUIInitialize() { }
    
    /// <summary>
    /// Gets executed every time the scene "LoadScene" gets loaded.
    /// </summary>
    public virtual void OnLoadSceneLoad() { }
    /// <summary>
    /// Gets executed every time the scene "LoadScene" gets unloaded .
    /// </summary>
    public virtual void OnLoadSceneUnload() { }
    /// <summary>
    /// Gets executed every time the scene "LoadScene" gets initialized.
    /// </summary>
    public virtual void OnLoadSceneInitialize() { }
    
    /// <summary>
    ///   Gets executed every time the scene "ZoneCore" gets initialized.
    /// </summary>
    public virtual void OnZoneCoreInitialized() { }
}