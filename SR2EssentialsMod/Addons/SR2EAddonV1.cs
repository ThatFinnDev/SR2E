namespace SR2E.Addons;

public abstract class SR2EAddonV1 : MelonMod
{
    public virtual void OnSaveDirectorLoading(AutoSaveDirector autoSaveDirector) {}

    public virtual void SaveDirectorLoaded() {}
    public virtual void LoadCommands() { }
}