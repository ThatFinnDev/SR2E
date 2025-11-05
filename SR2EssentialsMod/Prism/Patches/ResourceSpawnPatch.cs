using SR2E.Storage;

namespace SR2E.Prism.Patches;
/*
[PrismPatch()]
[HarmonyPatch(typeof(SpawnResource), nameof(SpawnResource.Awake))]
internal class ResourceSpawnPatch
{
    static void Prefix(SpawnResource __instance)
    {
        foreach (var action in PediaLibSpawning.onResourceGrowerAwake)
            try
            {
                action.Invoke(__instance);
            }
            catch (Exception e) { MelonLogger.Error(e); }
    }
}*/