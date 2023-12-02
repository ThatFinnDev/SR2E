using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;

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

[HarmonyPatch(typeof(SceneLoader), nameof(SceneLoader.LoadInitialSceneGroup))]
public static class SR2StartPatch
{
    public static void Prefix()
    {
        if (SR2EEntryPoint.skipEngagementPrompt)
        {
            MelonLogger.Msg("Skipping engagement prompt!");

            var sl = SystemContext.Instance.SceneLoader;

            sl.LoadMainMenuSceneGroup();
        }
    }
}