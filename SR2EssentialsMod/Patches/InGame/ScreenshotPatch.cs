using Il2CppMonomiPark.SlimeRancher.UI.Map;
using Il2CppMonomiPark.SlimeRancher.UI.Pause;
using SR2E.Menus;

namespace SR2E.Patches.InGame;

[HarmonyPatch(typeof(GameContext), nameof(GameContext.TakeScreenshot))]
internal static class ScreenshotPatch
{
    internal static System.Collections.IEnumerator WaitForUnpause()
    {
        while (Time.timeScale == 0)
        {
            yield return null;
        }
        sceneContext.PlayerState.VacuumItem.gameObject.SetActive(true);
    }
    
    internal static void Prefix(ScreenshotPauseItemModel __instance)
    {
        if (SR2ECheatMenu.betterScreenshot)
        {
            sceneContext.PlayerState.VacuumItem.gameObject.SetActive(false);
            MelonCoroutines.Start(WaitForUnpause());
        }
    }
}