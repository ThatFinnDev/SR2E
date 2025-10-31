using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.Economy;
using SR2E.Storage;

namespace SR2E.Cotton.Patches;

[LibraryPatch()]
[HarmonyPatch(typeof(PlortEconomyDirector),"InitModel")]
public static class EconomyPatch
{
    public static void Prefix(PlortEconomyDirector __instance)
    {
        CottonLibrary.Market.TryRefreshMarketData(__instance._settings);
    }
}