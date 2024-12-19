using Il2CppTMPro;

namespace SR2E.Expansion;

public abstract class SR2EExpansionV1 : MelonMod
{
    public static TMP_FontAsset sr2Font => SR2E.SR2EEntryPoint.SR2Font;

    public virtual void OnSR2FontLoad() { }
    public virtual void OnSaveDirectorLoading(AutoSaveDirector autoSaveDirector) {}

    public virtual void SaveDirectorLoaded() {}
    public virtual void LoadCommands() { }
    
    public virtual void OnSystemCoreLoad() { }
    public virtual void OnSystemCoreUnload() { }
    public virtual void OnSystemCoreInitialize() { }
    
    public virtual void OnStandaloneEngagementPromptLoad() { }
    public virtual void OnStandaloneEngagementPromptUnload() { }

    public virtual void OnStandaloneEngagementPromptInitialize() { }
    
    public virtual void OnGameCoreLoad() { }
    public virtual void OnGameCoreUnload() { }
    public virtual void OnGameCoreInitialize() { }
    
    public virtual void OnPlayerCoreLoad() { }
    public virtual void OnPlayerCoreUnload() { }
    public virtual void OnPlayerCoreInitialize() { }
    
    public virtual void OnUICoreLoad() { }
    public virtual void OnUICoreUnload() { }
    public virtual void OnUICoreInitialize() { }
    
    public virtual void OnMainMenuUILoad() { }
    public virtual void OnMainMenuUIUnload() { }
    public virtual void OnMainMenuUIInitialize() { }
    
    public virtual void OnLoadSceneLoad() { }
    public virtual void OnLoadSceneUnload() { }
    public virtual void OnLoadSceneInitialize() { }
    

}