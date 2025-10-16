using Il2CppMonomiPark.SlimeRancher.SceneManagement;

namespace SR2E.Patches.General;

[HarmonyPatch(typeof(SceneLoader), nameof(SceneLoader.LoadSceneGroup))]
public class SceneLoaderLoadSceneGroupPatch
{
    internal static SceneLoadingParameters _loadingParameters = null;
    internal static void Prefix(SceneLoader __instance, SceneGroup sceneGroup, SceneLoadingParameters parameters)
    {
        if (sceneGroup.IsGameplay)
            if (parameters != null)
                _loadingParameters = parameters;
        
    }
}