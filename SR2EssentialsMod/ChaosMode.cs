namespace SR2E;

internal class ChaosMode
{
    


    internal static void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        switch (sceneName)
        {
            case "GameCore":
                Color32 color = new Color32(242, 31, 124, 255);
                SlimeDefinition tarr = SR2EUtils.Get<SlimeDefinition>("Tarr");
                Material material = tarr.AppearancesDefault[0]._structures[0].DefaultMaterials[0];
                material.SetColor("_TopColor", color);
                material.SetColor("_MiddleColor", color);
                material.SetColor("_BottomColor", color);
                tarr.prefab.GetComponent<AttackPlayer>().DamagePerAttack = 1000;
                break;
            case "MainMenuEnvironment":
                GameObject playerModel = GameObject.Find("BeatrixMainMenu");
                playerModel.transform.localScale = new Vector3(-13.3182f, 2.0545f, -0.9782f);
                playerModel.transform.localPosition = new Vector3(-394.3078f, 26.876f, -46.412f);
                break;
        }
    }
}