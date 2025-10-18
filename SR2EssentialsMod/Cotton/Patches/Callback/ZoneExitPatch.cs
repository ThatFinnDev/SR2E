﻿using Il2CppMonomiPark.SlimeRancher.World;
using SR2E.Storage;

namespace SR2E.Cotton.Patches.Callback;

[LibraryPatch()]
[HarmonyPatch(typeof(PlayerZoneTracker), nameof(PlayerZoneTracker.OnExited))]
static class ZoneExitPatch
{
    public static void Postfix(ZoneDefinition zone)
    {
        Callbacks.Invoke_onZoneExit(zone);
    }
}