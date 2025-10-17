using CottonLibrary;
using HarmonyLib;
using Il2Cpp;


[HarmonyPatch(typeof(SpawnResource), nameof(SpawnResource.Awake))]
public class ResourceSpawnPatch
{
    static void Prefix(SpawnResource __instance)
    {
        foreach (var action in Library.onResourceGrowerAwake)
            action(__instance);
    }
}