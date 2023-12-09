using Il2CppMonomiPark.SlimeRancher.World;
using static Il2Cpp.AmbianceDirector;

namespace SR2E.Library
{
    class CallbackPatches
    {
        [HarmonyPatch(typeof(EconomyDirector), nameof(EconomyDirector.RegisterSold))]
        static class PlortSellPatch
        {
            public static void Postfix(EconomyDirector __instance, IdentifiableType id, int count)
            {
                Callbacks.Invoke_onPlortSold(count, id);
            }
        }

        [HarmonyPatch(typeof(PlayerZoneTracker), nameof(PlayerZoneTracker.OnEntered))]
        static class ZoneEnterPatch
        {
            public static void Postfix(ZoneDefinition zone)
            {
                Callbacks.Invoke_onZoneEnter(zone);
            }
        }

        [HarmonyPatch(typeof(PlayerZoneTracker), nameof(PlayerZoneTracker.OnExited))]
        static class ZoneExitPatch
        {
            public static void Postfix(ZoneDefinition zone)
            {
                Callbacks.Invoke_onZoneExit(zone);
            }
        }
    }
}
