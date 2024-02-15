/*namespace SR2E.Patches;

[HarmonyPatch(typeof(WaitForChargeup), nameof(WaitForChargeup.Update))]
public static class WaitForChargeupPatch
{
    public static bool Prefix(WaitForChargeup __instance)
    {
        if (__instance._model == null)
            return false;
        return true;
    }
}*/

using System;
using System.Linq;
using System.Reflection;

[HarmonyPatch]
internal static class Il2cppDetourMethodPatcherReportExceptionPatch
{
    public static MethodInfo TargetMethod()
    {
        return AccessTools.Method(AccessTools.AllAssemblies().FirstOrDefault((Assembly x) => x.GetName().Name.Equals("Il2CppInterop.HarmonySupport")).GetTypes().FirstOrDefault((Type x) => x.Name == "Il2CppDetourMethodPatcher"), "ReportException", null, null);
    }
    public static bool Prefix(Exception ex)
    {
        MelonLogger.Error("During invoking native->managed trampoline", ex);
        return false;
    }
}