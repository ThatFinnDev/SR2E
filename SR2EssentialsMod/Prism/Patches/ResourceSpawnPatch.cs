using SR2E.Cotton;
using SR2E.Storage;

namespace SR2E.Prism.Patches;

[PrismPatch()]
[HarmonyPatch(typeof(SpawnResource), nameof(SpawnResource.Awake))]
internal class ResourceSpawnPatch
{
    static void Prefix(SpawnResource __instance)
    {
        foreach (var action in CottonLibrary.Spawning.onResourceGrowerAwake)
            action(__instance);
    }
}