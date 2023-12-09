using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppMonomiPark.SlimeRancher.World.Teleportation;

namespace SR2E;

[HarmonyPatch(typeof(WeaponVacuum), nameof(WeaponVacuum.Expel), typeof(GameObject), typeof(bool), typeof(float), typeof(SlimeAppearance.AppearanceSaveSet))]
internal class WeaponVacuumExpelPatch
{
    public static void Prefix(WeaponVacuum __instance, ref bool ignoreEmotions)
    {
        if (__instance._player.Ammo.GetSelectedEmotions() == null)
        {
            ignoreEmotions = true;
        }
    }
}
[HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.MAX_AUTOSAVES),  MethodType.Getter)]
internal class AutoSaveCountPatch
{
    public static bool Prefix(int __result)
    {
        
    }
}
[HarmonyPatch(typeof(TeleporterNode), nameof(TeleporterNode.OnTriggerEnter))]
internal class TeleporterUsePatch
{
    public static void Prefix(Collider collider)
    {
        if (collider.gameObject == SceneContext.Instance.player)
            GameContext.Instance.AutoSaveDirector.SaveGame();
    }
}


[HarmonyPatch(typeof(SceneLoader), nameof(SceneLoader.LoadInitialSceneGroup))]
public static class SR2StartPatch
{
    public static bool Prefix()
    {
        if (SR2EEntryPoint.skipEngagementPrompt)
        {
            MelonLogger.Msg("Skipping engagement prompt!");

            var sl = SystemContext.Instance.SceneLoader;

            sl.LoadMainMenuSceneGroup();
            return false;
        }
        return true;
    }
}
[HarmonyPatch(typeof(SaveGamesRootUI), nameof(SaveGamesRootUI.Awake))]
public static class SaveGameRootUIPatch
{
    public static void Prefix()
    {
        SR2EEntryPoint.SaveCountChanged = false;
    }
}