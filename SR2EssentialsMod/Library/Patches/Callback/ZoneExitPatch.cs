
using CottonLibrary;
using Il2CppMonomiPark.SlimeRancher.World;


using HarmonyLib;
using Il2Cpp;
using SR2E.Storage;

namespace CottonLibrary.Patches.Callback;

[LibraryPatch()]
[HarmonyPatch(typeof(PlayerZoneTracker), nameof(PlayerZoneTracker.OnExited))]
static class ZoneExitPatch
{
    public static void Postfix(ZoneDefinition zone)
    {
        Callbacks.Invoke_onZoneExit(zone);
    }
}