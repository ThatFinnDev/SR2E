using Il2CppMonomiPark.SlimeRancher.Economy;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.UI.Plot;
using SR2E.Storage;

namespace SR2E.Prism.Patches.Landplot;

[PrismPatch()]
[HarmonyPatch(typeof(LandPlotUIRoot),nameof(LandPlotUIRoot.BuyPlot))]
internal static class LandPlotUIRootClosePatch
{
    // Required for non ranch plots
    public static void Postfix(LandPlotUIRoot __instance,PurchaseCost cost,GameObject plotPrefab)
    {
        if (plotPrefab.name=="patchEmpty")
            ExecuteInTicks(() =>
            {
                foreach (var c in GetAllInScene<Canvas>("DimBackground(Clone)")) 
                    GameObject.Destroy(c.gameObject);
                try { __instance.Close(); }catch { }
            },2);
    }
}