using Il2CppMonomiPark.SlimeRancher.SceneManagement;

namespace SR2E.Patches.General;

[HarmonyPatch(typeof(LocationBookmarksUtil), nameof(LocationBookmarksUtil.GoToLocationPlayer))]
internal class TeleportPlayerPatch
{
    internal static bool isTeleportingPlayer = false;
    internal static void Prefix()
    {
        isTeleportingPlayer = true;
    } 
}
[HarmonyPatch(typeof(SceneLoader), nameof(SceneLoader.LoadSceneGroup))]
internal class TeleportPlayerPatch2
{
    public static void Prefix(SceneLoader __instance,ref SceneGroup sceneGroup, SceneLoadingParameters parameters)
    {
        if (TeleportPlayerPatch.isTeleportingPlayer)
        {
            TeleportPlayerPatch.isTeleportingPlayer = false;
            if (parameters != null)
            {
                parameters.ReloadAllCoreScenes = false;
            }
        }
    } 
}