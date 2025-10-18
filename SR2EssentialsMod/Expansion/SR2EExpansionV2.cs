using Il2CppMonomiPark.SlimeRancher;

namespace SR2E.Expansion;
/// <summary>
/// <see cref="CottonMod"/> but with a pre-made instance variable.
/// </summary>
/// <typeparam name="M">This is your entry point class. For example, you can do <code>public class ModEntry : CottonModInstance&#60;ModEntry&#62; {}</code></typeparam>
/*public class SR2EExpansionV2Instance<M> : SR2EExpansionV2 where M : SR2EExpansionV2Instance<M>
{
    public static SR2EExpansionV2Instance<M> Instance { get; private set; }
    
    /// <summary>
    /// Make sure this code runs or else the class is useless!
    /// </summary>
    public SR2EExpansionV2Instance() => Instance = this;
} */

public abstract class SR2EExpansionV2 : SR2EExpansionV1
{
    //public void UseCotton() {}
    public virtual void OnPlayerSceneLoaded() { }
    public virtual void OnSystemSceneLoaded() { }
    public virtual void OnGameCoreLoaded() { }
    public virtual void OnZoneCoreLoaded() { }
    public virtual void SaveDirectorLoaded() { }
    
    /// <summary>
    /// This is the same thing as <c>SaveDirectorLoaded</c> except it's called after <c>SaveDirectorLoaded</c> has already called on each mod.
    /// </summary>
    public virtual void LateSaveDirectorLoaded() { }  
    /// <summary>
    /// An even later stage of loading than <c>LateSaveDirectorLoaded</c> 
    /// </summary>
    public virtual void AutoLargosLoaded() { }

    public virtual void SaveDirectorLoading(AutoSaveDirector saveDirector) { }
}