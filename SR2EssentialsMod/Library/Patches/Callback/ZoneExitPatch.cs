
using Il2CppMonomiPark.SlimeRancher.World;


namespace SR2E.Library.Patches.Callback;

[HarmonyPatch(typeof(PlayerZoneTracker), nameof(PlayerZoneTracker.OnExited))]
static class ZoneExitPatch
{
    public static void Postfix(ZoneDefinition zone)
    {
        Callbacks.Invoke_onZoneExit(zone);
    }
}