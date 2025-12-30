using System;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Persist;
using SR2E.Commands;

namespace SR2E.Patches.Saving.Fixer;

[HarmonyPriority(-99999999)]
[HarmonyPatch(typeof(GameModelPushHelpers), nameof(GameModelPushHelpers.PushRanch))]
internal static class SaveFixerPushRanch
{
    internal static void Prefix(GameModel gameModel, RanchV02 ranch, ILoadReferenceTranslation loadReferenceTranslation)
    {
        try
        {
            RanchCommand.LoadAutoComplete(ranch);
            if (!SR2EEntryPoint.disableFixSaves)
            {
                foreach (var plot in ranch.Plots.ToArray())
                {
                    //Remove invalid plot
                    if (plot == null)
                        ranch.Plots.Remove(plot);
                    if (!Enum.IsDefined<LandPlot.Id>(plot.TypeId))
                        ranch.Plots.Remove(plot);
                    else
                        foreach (LandPlot.Upgrade upgrade in plot.Upgrades)
                            if (!Enum.IsDefined<LandPlot.Upgrade>(upgrade))
                                //Remove invalid upgrade
                                plot.Upgrades.Remove(upgrade);
                }
            }
        }
        catch (Exception e) { MelonLogger.Error(e); }
    }

}