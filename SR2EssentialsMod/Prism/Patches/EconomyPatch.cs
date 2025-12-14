using Il2CppMonomiPark.SlimeRancher.Economy;
using SR2E.Prism.Lib;
using SR2E.Storage;

namespace SR2E.Prism.Patches;

[PrismPatch()]
[HarmonyPatch(typeof(PlortEconomyDirector),nameof(PlortEconomyDirector.InitModel))]
internal static class EconomyPatch
{
    public static void Prefix(PlortEconomyDirector __instance)
    {
        PrismLibMarket.TryRefreshMarketData(__instance._settings);
    }
}