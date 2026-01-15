using SR2E.Prism.Lib;
using SR2E.Storage;

namespace SR2E.Prism.Patches.Landplot;

[PrismPatch()]
[HarmonyPatch(typeof(LandPlot),nameof(LandPlot.Start))]
internal static class LandPlotStartPatch
{
    [HarmonyFinalizer]
    static Exception Finalizer(LandPlot __instance,Exception __exception)
    {
        if (__exception == null) return null;
        try { if (PrismLibLandPlots.landPlotLocations.Contains(__instance.transform.parent.GetComponent<LandPlotLocation>())) return null; } catch { }
        return __exception;
    }
}