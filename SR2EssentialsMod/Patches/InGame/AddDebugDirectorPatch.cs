using Il2CppMonomiPark.SlimeRancher.Player;

namespace SR2E.Patches.InGame;

[HarmonyPatch(typeof(PlayerObjectDiscoveryHandler), nameof(PlayerObjectDiscoveryHandler.Start))]
internal class AddDebugDirectoyPatch
{
    internal static void Postfix(PlayerObjectDiscoveryHandler __instance)
    {
        if (__instance.gameObject.GetComponent<SR2EDebugUI>() == null)
            __instance.gameObject.AddComponent<SR2EDebugUI>();
    }
}
