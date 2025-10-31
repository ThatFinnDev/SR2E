using Il2CppMonomiPark.SlimeRancher.Economy;
using SR2E.Cotton;
using SR2E.Storage;

namespace SR2E.Prism.Patches;

[PrismPatch()]
[HarmonyPatch(typeof(PlortEconomyDirector),nameof(PlortEconomyDirector.InitModel))]
public static class EconomyPatch
{
    public static void Prefix(PlortEconomyDirector __instance)
    {
        CottonLibrary.Market.TryRefreshMarketData(__instance._settings);
    }
}