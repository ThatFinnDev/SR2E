using Il2CppMonomiPark.SlimeRancher;

namespace SR2E.Expansion;

public abstract class SR2EExpansionV2 : SR2EExpansionV1
{
    public virtual void OnZoneCoreLoaded() { }
    /// <summary>
    /// Requires Library
    /// </summary>
    public virtual void SaveDirectorLoaded() { }
    
    /// <summary>
    /// This is the same thing as <c>SaveDirectorLoaded</c> except it's called after <c>SaveDirectorLoaded</c> has already called on each mod.
    /// Requires Library
    /// </summary>
    public virtual void LateSaveDirectorLoaded() { }  
    /// <summary>
    /// An even later stage of loading than <c>LateSaveDirectorLoaded</c>
    /// Requires Library
    /// </summary>
    public virtual void SaveDirectorLoading(AutoSaveDirector saveDirector) { }
    /// <summary>
    /// Requires Library
    /// </summary>
    public virtual void AutoLargosLoaded() { }

    /// <summary>
    /// Gets executed once SceeneContext loads. In Postfix of the Start method
    /// </summary>
    public virtual void OnSceneContext(SceneContext sceneContext) { }
}