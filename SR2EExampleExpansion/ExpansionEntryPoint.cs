using Il2CppMonomiPark.SlimeRancher;
using SR2E.Expansion;
using SR2E.Utils;

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


    public override void LoadCommands()
    {
        // Used to register commands manually
        // SR2EConsole.RegisterCommand(new Command());
    }

    public override void AfterGameContext(GameContext gameContext)
    {

    }

    public override void AfterSystemContext(SystemContext systemContext)
    {

    }

    public override void AfterSceneContext(SceneContext sceneContext)
    {
    }
}

