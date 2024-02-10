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