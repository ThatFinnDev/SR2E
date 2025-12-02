using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;

namespace SR2E.Patches.MainMenu;

[HarmonyPatch(typeof(NewGameRootUI), nameof(NewGameRootUI.Awake))]
internal static class NewGameRootUIPatch
{
    internal static void Prefix(NewGameRootUI __instance)
    {
        SR2EEntryPoint.baseUIAddSliders.Add(__instance);
    }
}