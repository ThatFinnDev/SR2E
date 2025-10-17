using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMono.Security.X509;
using MelonLoader;
using SR2E.Storage;
using UnityEngine;

namespace CottonLibrary.Patches;

[LibraryPatch()]
[HarmonyPatch(typeof(DirectedActorSpawner))]
public class SpawnerPatch
{
    [HarmonyPatch(nameof(DirectedActorSpawner.Awake))]
    [HarmonyPostfix]
    static void PostAwake(DirectedActorSpawner __instance)
    {
        foreach (var action in Library.executeOnSpawnerAwake)
        {
            action(__instance);
        }
    }
    
    [HarmonyPatch(nameof(DirectedActorSpawner.MaybeReplaceId))]
    [HarmonyPrefix]
    static bool Replacement(DirectedActorSpawner __instance, ref IdentifiableType __result, IdentifiableType id)
    {
        if (!__instance) return false;
        if (__instance.WasCollected) return false;
        
        foreach (var replacement in spawnerReplacements)
        {
            try
            {
                if (IsInZone(replacement.zones))
                {
                    var chance = Randoms.SHARED.GetProbability(1f / replacement.chance);
                    if (chance)
                    {
                        __result = replacement.ident;
                        return false;
                    }
                }
            }
            catch { }
        }

        __result = id;
        
        return false;
    }
}