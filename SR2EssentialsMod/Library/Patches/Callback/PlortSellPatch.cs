using HarmonyLib;
using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.Economy;

namespace CottonLibrary.Patches.Callback;

[HarmonyPatch(typeof(PlortEconomyDirector), nameof(PlortEconomyDirector.RegisterSold))]
static class PlortSellPatch
{
    public static void Postfix(PlortEconomyDirector __instance, IdentifiableType id, int count)
    {
        Callbacks.Invoke_onPlortSold(count, id);
    }
}