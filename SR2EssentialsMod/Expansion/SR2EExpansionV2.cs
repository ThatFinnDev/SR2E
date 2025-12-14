using System;
using Il2CppMonomiPark.SlimeRancher;

namespace SR2E.Expansion;

[Obsolete("OBSOLETE!: Please use SR2EExpansionV3.")]
public abstract class SR2EExpansionV2 : SR2EExpansionV1
{
    public virtual void OnZoneCoreLoaded() { }
    /// <summary>
    /// In this function you should add all of your base slimes, veggies, toys etc.
    /// Requires Prism
    /// </summary>
    public virtual void OnPrismCreateAdditions() { }
    
    /// <summary>
    /// Use this if you want to do stuff with every e.g slime, veggie etc.
    /// DO NOT add objects here, do that in <c>OnPrismCreateAdditions</c>
    /// This gets called after every mod ran OnPrismCreateAdditions()
    /// Requires Prism
    /// </summary>
    public virtual void AfterPrismCreateAdditions() { }
    
    
    
    /// <summary>
    /// Get's called before the AutoSaveDirector has been loaded
    /// </summary>
    public virtual void BeforeSaveDirectorLoaded(AutoSaveDirector saveDirector) { }
    /// <summary>
    /// Get's called after the AutoSaveDirector has been loaded
    /// You should use this method to add your translations to SR2
    /// and to add your own buttons.
    /// </summary>
    public virtual void AfterSaveDirectorLoaded(AutoSaveDirector saveDirector) { }
    /// <summary>
    /// This gets called after all largos have been created
    /// Requires Prism
    /// </summary>
    public virtual void AfterPrismLargosCreated() { }

    /// <summary>
    /// Gets executed once SceneContext loads. In Postfix of the Start method
    /// </summary>
    public virtual void OnSceneContext(SceneContext sceneContext) { }
    
    
    
    
    [Obsolete("OBSOLETE!: Use BeforeSaveDirectorLoaded instead", true)]
    public override void OnSaveDirectorLoading(AutoSaveDirector autoSaveDirector) {}

    [Obsolete("OBSOLETE!: Use AfterSaveDirectorLoaded instead", true)]
    public override void SaveDirectorLoaded(AutoSaveDirector autoSaveDirector) {}
}