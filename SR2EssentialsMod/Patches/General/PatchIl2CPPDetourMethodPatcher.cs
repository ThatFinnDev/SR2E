namespace SR2E.Patches.General;

[HarmonyPatch("Il2CppInterop.HarmonySupport.Il2CppDetourMethodPatcher", "ReportException")]
internal static class PatchIl2CppDetourMethodPatcher
{
    internal static bool Prefix(System.Exception ex)
    {
        if (!EnableIl2CppDetourExceptionReporting.HasFlag()) return true;
        MelonLogger.Error("During invoking native->managed trampoline", ex);
        return false;                               
    }
}