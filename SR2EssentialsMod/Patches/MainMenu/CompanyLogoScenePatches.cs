using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;

namespace SR2E.Patches.MainMenu;

public static class CompanyLogoScenePatches
{
    [HarmonyPatch(typeof(CompanyLogoScene), nameof(CompanyLogoScene.StartLoadingIndicator))]
    public static class StartLoadingIndicatorPatch
    {
        public static bool alreadyStarted = false;
        public static bool Prefix(CompanyLogoScene __instance)
        {
            if (alreadyStarted) return false;
            alreadyStarted = true;
            return true;
        }
    }

    [HarmonyPatch(typeof(CompanyLogoScene), nameof(CompanyLogoScene.Start))]
    public static class StartPatch
    {
        public static void Postfix(CompanyLogoScene __instance)
        {
            StartLoadingIndicatorPatch.alreadyStarted = false;
        }
    }
}
