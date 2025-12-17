using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using SR2E.Prism;

namespace SR2E.Patches.MainMenu;

internal static class CompanyLogoScenePatches
{
    internal static List<Sprite> customBouncySprites = new();
    [HarmonyPatch(typeof(CompanyLogoScene), nameof(CompanyLogoScene.StartLoadingIndicator))]
    internal static class StartLoadingIndicatorPatch
    {
        internal static bool alreadyStarted = false;
        internal static bool Prefix(CompanyLogoScene __instance)
        {
            __instance._skipWarningScreen = true;
            try
            {
                foreach (var sprite in customBouncySprites)
                    if(!__instance.bouncyIcons.Contains(sprite))
                        __instance.bouncyIcons = __instance.bouncyIcons.AddToNew(sprite);
                
            } catch { }
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
            __instance.chanceToBlink = 1f;
            __instance._skipWarningScreen = true;
            try
            {
                foreach (var sprite in customBouncySprites)
                    if(!__instance.bouncyIcons.Contains(sprite))
                        __instance.bouncyIcons = __instance.bouncyIcons.AddToNew(sprite);
                
            } catch { }
            __instance.StartLoadingIndicator();
        }
    }
}
