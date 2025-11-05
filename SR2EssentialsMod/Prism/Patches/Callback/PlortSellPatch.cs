using Il2CppMonomiPark.SlimeRancher.Economy;
using SR2E.Storage;

namespace SR2E.Prism.Patches.Callback;

[PrismPatch()]
[HarmonyPatch(typeof(PlortEconomyDirector), nameof(PlortEconomyDirector.RegisterSold))]
static class PlortSellPatch
{
    public static void Postfix(PlortEconomyDirector __instance, IdentifiableType id, int count)
    {
        Callbacks.Invoke_onPlortSold(count, id);
    }
}