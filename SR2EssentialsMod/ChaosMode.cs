namespace SR2E;

internal class ChaosMode
{
    internal static void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        switch (sceneName)
        {
            case "GameCore":
                break;
            case "PlayerCore":
                break;
            case "UICore":
                break;
            case "MainMenuEnvironment":
                GameObject playerModel = GameObject.Find("LeBe");
                break;
        }
    }
}