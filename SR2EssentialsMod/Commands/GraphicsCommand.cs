using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.Rendering;

namespace SR2E.Commands;

internal class GraphicsCommand : SR2ECommand
{
    public override string ID => "graphics";
    public override string Usage => "graphics <mode>";
    public override CommandType type => CommandType.Fun;
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    { 
        if (argIndex == 0)
            return new List<string> { "NORMAL", "POTATO", "SHINY", "FAKEDAY", "ORANGE" };
        return null;
    }

    public override void OnMainMenuUILoad()
    {
        if(rangeLightInstance!=null)
        {
            rangeLightInstance.transform.GetChild(0).gameObject.SetActive(false);
            rangeLightInstance.transform.GetChild(1).gameObject.SetActive(false);
            rangeLightInstance.transform.GetChild(2).gameObject.SetActive(false);
            rangeLightInstance.GetComponent<SunAndMoonVector>().enabled = false;
            return;
        }
        GameObject obj = GameObject.Instantiate(Get<GameObject>("Range Lights"));
        obj.transform.GetChild(1).gameObject.SetActive(false);
        Object.DontDestroyOnLoad(obj);
        rangeLightInstance = obj;
    }

    internal static GameObject rangeLightInstance;
    internal static bool activated = false;
    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,1)) return SendUsage();


        if (rangeLightInstance == null)
        { SendError(translation("cmd.graphics.norangelights")); return false; }


        rangeLightInstance.transform.GetChild(0).gameObject.SetActive(false);
        rangeLightInstance.transform.GetChild(1).gameObject.SetActive(false);
        rangeLightInstance.transform.GetChild(2).gameObject.SetActive(false);
        rangeLightInstance.GetComponent<SunAndMoonVector>().enabled = false;
        Il2CppArrayBase<ShaderGlobalVectorUpdater> vectorUpdaters = rangeLightInstance.GetComponents<ShaderGlobalVectorUpdater>();
        for (int i = 0; i < vectorUpdaters.Length; i++)
            vectorUpdaters[i].enabled = false;
        
        switch (args[0].ToUpper())
        {
            default:
                SendMessage(translation("cmd.graphics.success","normal"));
                return true;
            case "POTATO":
                rangeLightInstance.transform.GetChild(2).gameObject.SetActive(true);
                SendMessage(translation("cmd.graphics.success","potato"));
                return true;
            case "SHINY":
                rangeLightInstance.transform.GetChild(0).gameObject.SetActive(true);
                SendMessage(translation("cmd.graphics.success","shiny"));
                return true;
            case "FAKEDAY":
                rangeLightInstance.transform.GetChild(1).gameObject.SetActive(true);
                SendMessage(translation("cmd.graphics.success","fakeday"));
                return true;
            case "ORANGE":
                rangeLightInstance.transform.GetChild(1).gameObject.SetActive(true);
                rangeLightInstance.GetComponent<SunAndMoonVector>().enabled = true;
                for (int i = 0; i < vectorUpdaters.Length; i++)
                    vectorUpdaters[i].enabled = true;
                SendMessage(translation("cmd.graphics.success","orange"));
                return true;
        }
    }
}