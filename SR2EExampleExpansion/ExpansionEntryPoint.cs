using Il2CppMonomiPark.SlimeRancher;
using SR2E.Expansion;

namespace SR2EExampleExpansion;


public class ExpansionEntryPoint : SR2EExpansionV3
{
    public override void OnInitializeMelon()
    {
        AddLanguages(EmbeddedResourceEUtil.LoadString("translations.csv"));
    }

    public override void AfterSaveDirectorLoaded(AutoSaveDirector saveDirector)
    {
    }

    public override void BeforeSaveDirectorLoaded(AutoSaveDirector saveDirector)
    {

    }

    public override void AfterGameContext(GameContext gameContext)
    {

    }

}

