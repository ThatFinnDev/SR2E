using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;

namespace SR2E.Patches.General;

[HarmonyPatch(typeof(CompanyLogoScene), nameof(CompanyLogoScene.StartLoadingIndicator))]
public static class CompanyLogoScenePatch
{
    public static bool alreadyStarted = false;
    public static bool Prefix(PlatformEngagementPrompt __instance)
    {
        if (alreadyStarted) return false;
        alreadyStarted = true;
        return true;
    }
}
