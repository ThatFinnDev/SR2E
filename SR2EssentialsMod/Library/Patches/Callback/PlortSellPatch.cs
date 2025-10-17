using HarmonyLib;
using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.Economy;
using SR2E.Storage;

namespace CottonLibrary.Patches.Callback;

[LibraryPatch()]
static class PlortSellPatch
{
    [HarmonyPostfix,HarmonyPatch(typeof(PlortEconomyDirector), nameof(PlortEconomyDirector.RegisterSold))]
    public static void Postfix(PlortEconomyDirector __instance, IdentifiableType id, int count)
    {
        Callbacks.Invoke_onPlortSold(count, id);
    }
}