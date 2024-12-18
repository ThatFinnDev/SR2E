using Il2CppMonomiPark.SlimeRancher.UI.Map;
using Il2CppMonomiPark.SlimeRancher.UI.Pause;

namespace SR2E.Patches.InGame;

[HarmonyPatch(typeof(GameContext), nameof(GameContext.TakeScreenshot))]
public static class ScreenshotPatch
{
    public static System.Collections.IEnumerator WaitForUnpause()
    {
        while (Time.timeScale == 0)
        {
            yield return null;
        }
        SceneContext.Instance.PlayerState.VacuumItem.gameObject.SetActive(true);
    }
    
    public static void Prefix(ScreenshotPauseItemModel __instance)
    {
        if (SR2ECheatMenu.betterScreenshot)
        {
            SceneContext.Instance.PlayerState.VacuumItem.gameObject.SetActive(false);
            MelonCoroutines.Start(WaitForUnpause());
        }
    }
}