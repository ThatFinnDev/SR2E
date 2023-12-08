namespace SR2E;

internal class ChaosMode
{
    internal static void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        switch (sceneName)
        {
            case "MainMenuEnvironment":
                GameObject playerModel = GameObject.Find("BeatrixMainMenu");
                playerModel.transform.localScale = new Vector3(-13.3182f, 2.0545f, -0.9782f);
                playerModel.transform.localPosition = new Vector3(-394.3078f, 26.876f, -46.412f);
                break;
        }
    }
}