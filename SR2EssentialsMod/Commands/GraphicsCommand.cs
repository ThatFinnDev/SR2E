using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.Rendering;

namespace SR2E.Commands;

public class GraphicsCommand : SR2CCommand
{
    public override string ID => "graphics";
    public override string Usage => "graphics <mode>";
    public override string Description => "Makes your game look cool, or worse";
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
        GameObject obj = GameObject.Instantiate(SR2EUtils.Get<GameObject>("Range Lights"));
        obj.transform.GetChild(1).gameObject.SetActive(false);
        Object.DontDestroyOnLoad(obj);
        rangeLightInstance = obj;
    }

    internal static GameObject rangeLightInstance;
    internal static bool activated = false;
    public override bool Execute(string[] args)
    {
        if (args == null)
        { SR2Console.SendError($"Usage: {Usage}"); return false; }
        if (args.Length != 1)
        { SR2Console.SendMessage($"Usage: {Usage}"); return false; }


        if (rangeLightInstance == null)
        { SR2Console.SendError("An unknown error occured!"); return false; }


        rangeLightInstance.transform.GetChild(0).gameObject.SetActive(false);
        rangeLightInstance.transform.GetChild(1).gameObject.SetActive(false);
        rangeLightInstance.transform.GetChild(2).gameObject.SetActive(false);
        rangeLightInstance.GetComponent<SunAndMoonVector>().enabled = false;
        Il2CppArrayBase<ShaderGlobalVectorUpdater> vectorUpdaters = rangeLightInstance.GetComponents<ShaderGlobalVectorUpdater>();
        for (int i = 0; i < vectorUpdaters.Length; i++)
            vectorUpdaters[i].enabled = false;
        
        switch (args[0])
        {
            default:
                SR2Console.SendMessage("Successfully set graphics to normal!");
                return true;
            case "POTATO":
                rangeLightInstance.transform.GetChild(2).gameObject.SetActive(true);
                SR2Console.SendMessage("Successfully set graphics to potato!");
                return true;
            case "SHINY":
                rangeLightInstance.transform.GetChild(0).gameObject.SetActive(true);
                SR2Console.SendMessage("Successfully set graphics to shiny!");
                return true;
            case "FAKEDAY":
                rangeLightInstance.transform.GetChild(1).gameObject.SetActive(true);
                SR2Console.SendMessage("Successfully set graphics to fakeday!");
                return true;
            case "ORANGE":
                rangeLightInstance.transform.GetChild(1).gameObject.SetActive(true);
                rangeLightInstance.GetComponent<SunAndMoonVector>().enabled = true;
                for (int i = 0; i < vectorUpdaters.Length; i++)
                    vectorUpdaters[i].enabled = true;
                SR2Console.SendMessage("Successfully set graphics to orange!");
                return true;
        }
    }
}