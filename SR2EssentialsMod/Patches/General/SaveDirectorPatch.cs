namespace SR2E.Patches.General;

<<<<<<< HEAD
[HarmonyPatch(typeof(AutoSaveDirector), "Awake")]
public static class SaveDirectorPatch
{
    public static void Prefix(AutoSaveDirector __instance)
    {
        SR2EEntryPoint.OnSaveDirectorLoading(__instance);
    }
    public static void Postfix()
=======
[HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.Awake))]
internal static class SaveDirectorPatch
{
    internal static void Prefix(AutoSaveDirector __instance)
    {
        SR2EEntryPoint.OnSaveDirectorLoading(__instance);
    }
    internal static void Postfix()
>>>>>>> experimental
    {
        SR2EEntryPoint.SaveDirectorLoaded();
    }
}
