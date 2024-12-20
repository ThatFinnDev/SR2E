namespace SR2E.Patches.General;

[HarmonyPatch("Il2CppInterop.HarmonySupport.Il2CppDetourMethodPatcher", "ReportException")]
public static class PatchIl2CppDetourMethodPatcher
{
    public static bool Prefix(System.Exception ex)
    {
        MelonLogger.Error("During invoking native->managed trampoline", ex);
        return false;                               
    }
}