using Il2CppMonomiPark.SlimeRancher.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Library.Patches.Callback;

[HarmonyPatch(typeof(PlayerZoneTracker), nameof(PlayerZoneTracker.OnEntered))]
static class ZoneEnterPatch
{
    public static void Postfix(ZoneDefinition zone)
    {
        Callbacks.Invoke_onZoneEnter(zone);
    }
}