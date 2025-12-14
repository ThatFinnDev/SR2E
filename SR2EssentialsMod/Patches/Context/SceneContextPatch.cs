namespace SR2E.Patches.Context;

[HarmonyPatch(typeof(SceneContext), nameof(SceneContext.Start))]
internal class SceneContextPatch
{
    internal static void Postfix(SceneContext __instance)
    {
        SR2EEntryPoint.CheckForTime();
        foreach (var expansion in SR2EEntryPoint.expansionsV3)
            try { expansion.AfterSceneContext(__instance); } 
            catch (Exception e) { MelonLogger.Error(e); }
        foreach (var expansion in SR2EEntryPoint.expansionsV2)
            try { expansion.OnSceneContext(__instance); } 
            catch (Exception e) { MelonLogger.Error(e); }
    }
}