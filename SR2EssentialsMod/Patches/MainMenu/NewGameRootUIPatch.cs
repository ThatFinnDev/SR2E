using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;

namespace SR2E.Patches.MainMenu;

[HarmonyPatch(typeof(NewGameOptionsUIRoot), nameof(NewGameOptionsUIRoot.Awake))]
internal static class NewGameRootUIPatch
{
    internal static void Prefix(NewGameOptionsUIRoot __instance)
    {
        SR2EEntryPoint.baseUIAddSliders.Add(__instance);
    }
}