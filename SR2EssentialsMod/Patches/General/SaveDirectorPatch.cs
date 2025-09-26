using Il2CppMonomiPark.SlimeRancher;
using Il2CppSystem.Reflection;

namespace SR2E.Patches.General;

[HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.Awake))]
internal static class SaveDirectorPatch
{
    internal static void Prefix(AutoSaveDirector __instance)
    {
        SR2EEntryPoint.OnSaveDirectorLoading(__instance);
    }
    internal static void Postfix(AutoSaveDirector __instance)
    {
        SR2EEntryPoint.SaveDirectorLoaded();
    }
}
