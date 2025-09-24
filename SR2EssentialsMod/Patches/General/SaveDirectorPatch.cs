using Il2CppMonomiPark.SlimeRancher;

namespace SR2E.Patches.General;

[HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.Awake))]
internal static class SaveDirectorPatch
{
    internal static void Prefix(AutoSaveDirector __instance)
    {
        SR2EEntryPoint.OnSaveDirectorLoading(__instance);
    }
    internal static void Postfix()
    {
        SR2EEntryPoint.SaveDirectorLoaded();
    }
}
