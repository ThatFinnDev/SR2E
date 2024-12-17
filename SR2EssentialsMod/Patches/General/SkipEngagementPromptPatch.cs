namespace SR2E.Patches.General
{
    [HarmonyPatch(typeof(SystemContext), nameof(SystemContext.Start))]
    public static class SkipStartupPatch
    {
        public static void Prefix(SystemContext __instance)
        {
            __instance.SceneLoader._initialSceneGroup = __instance.SceneLoader._mainMenuSceneGroup;
        }
    }
}
