using Cotton;
using SR2E.Storage;

namespace SR2E.Cotton.Patches;

[LibraryPatch()]
[HarmonyPatch(typeof(SpawnResource), nameof(SpawnResource.Awake))]
public class ResourceSpawnPatch
{
    static void Prefix(SpawnResource __instance)
    {
        foreach (var action in CottonLibrary.Spawning.onResourceGrowerAwake)
            action(__instance);
    }
}