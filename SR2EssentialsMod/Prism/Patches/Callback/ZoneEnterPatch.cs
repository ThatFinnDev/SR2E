using Il2CppMonomiPark.SlimeRancher.World;
using SR2E.Storage;

namespace SR2E.Prism.Patches.Callback;

[PrismPatch()]
[HarmonyPatch(typeof(PlayerZoneTracker), nameof(PlayerZoneTracker.OnEntered))]
static class ZoneEnterPatch
{
    public static void Postfix(ZoneDefinition zone)
    {
        Callbacks.Invoke_onZoneEnter(zone);
    }
}