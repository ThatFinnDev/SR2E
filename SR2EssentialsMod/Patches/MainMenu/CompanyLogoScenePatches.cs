using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;

namespace SR2E.Patches.MainMenu;

internal static class CompanyLogoScenePatches
{
    [HarmonyPatch(typeof(CompanyLogoScene), nameof(CompanyLogoScene.StartLoadingIndicator))]
    internal static class StartLoadingIndicatorPatch
    {
        internal static bool alreadyStarted = false;
        internal static bool Prefix(CompanyLogoScene __instance)
        {
            if (alreadyStarted) return false;
            alreadyStarted = true;
            return true;
        }
    }

    [HarmonyPatch(typeof(CompanyLogoScene), nameof(CompanyLogoScene.Start))]
    internal static class StartPatch
    {
        internal static void Postfix(CompanyLogoScene __instance)
        {
            StartLoadingIndicatorPatch.alreadyStarted = false;
        }
    }
}
