using SR2E.Prism.Lib;
using SR2E.Storage;

namespace SR2E.Prism.Patches;

[PrismPatch()]
[HarmonyPatch(typeof(DirectedActorSpawner))]
internal class SpawnerPatch
{
    [HarmonyPatch(nameof(DirectedActorSpawner.Awake))]
    [HarmonyPostfix]
    static void PostAwake(DirectedActorSpawner __instance)
    {
        foreach (var action in PrismLibSpawning.executeOnSpawnerAwake)
            try
            {
                action.Invoke(__instance);
            }
            catch (Exception e) { MelonLogger.Error(e); }
    }
    
}