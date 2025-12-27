using System.IO;
using Il2CppMonomiPark.SlimeRancher;
using SR2E.Expansion;
using SR2E.Saving;
using SR2E.Storage;
using SR2E.Utils;

namespace SR2EExampleExpansion;


public class ExpansionEntryPoint : SR2EExpansionV3
{
    public override void OnInitializeMelon()
    {
        AddLanguages(EmbeddedResourceEUtil.LoadString("translations.csv"));
    }


    public override void OnEarlyCustomSaveDataReceived(RootSave saveRoot, LoadingGameSessionData loadingGameSessionData)
    {
        // Optional check if the save data is the correct one.
        if (!(saveRoot is ExampleSaveData)) return;
        var data = (ExampleSaveData)saveRoot;
    }

    public override RootSave OnSaveCustomSaveData(SavingGameSessionData savingGameSessionData)
    {
        // You can create a new TestSaveRoot or you can also have a static TestSaveRoot that you use constantly
        // and return it here
        var data = new ExampleSaveData();

        data.ilList = new Il2CppSystem.Collections.Generic.List<string>();
        data.ilList.Add("IL2CPP_Item");

        data.ilDict = new Il2CppSystem.Collections.Generic.Dictionary<int, Vector3>();
        data.ilDict.Add(1, Vector3.up);
        return data;
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

