namespace SR2E.Patches.General;

[HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.Awake))]
public static class SaveDirectorPatch
{
    public static void Prefix(AutoSaveDirector __instance)
    {
        SR2EEntryPoint.OnSaveDirectorLoading(__instance);
    }
    public static void Postfix()
    {
        SR2EEntryPoint.SaveDirectorLoaded();
    }
}
