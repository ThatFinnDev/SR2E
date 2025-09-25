using Il2CppMonomiPark.SlimeRancher;

namespace SR2E.Patches.General;

[HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.Awake))]
internal static class SaveDirectorPatch
{
    internal static void Prefix(AutoSaveDirector __instance)
    {
        __instance._configuration._saveSlotCount = SAVESLOT_COUNT.Get();
        SR2EEntryPoint.OnSaveDirectorLoading(__instance);
    }
    internal static void Postfix(AutoSaveDirector __instance)
    {
        __instance._configuration._saveSlotCount = SAVESLOT_COUNT.Get();
        SR2EEntryPoint.SaveDirectorLoaded();
    }
}
