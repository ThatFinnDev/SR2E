using System;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Persist;

namespace SR2E.Patches.SaveFixer;

[HarmonyPatch(typeof(GameModelPushHelpers), nameof(GameModelPushHelpers.PushRanch))]
internal static class SaverFixerPushRanch
{
    internal static void Postfix(GameModel gameModel, RanchV02 ranch, ILoadReferenceTranslation loadReferenceTranslation)
    {
        try
        {
            if (!SR2EEntryPoint.disableFixSaves) foreach(var plot in ranch.Plots)
            {
                //Remove invalid plot
                if (!Enum.IsDefined<LandPlot.Id>(plot.TypeId))
                    ranch.Plots.Remove(plot);
                else
                    foreach (LandPlot.Upgrade upgrade in plot.Upgrades)
                        if (!Enum.IsDefined<LandPlot.Upgrade>(upgrade))
                            //Remove invalid upgrade
                            plot.Upgrades.Remove(upgrade);
            }
        }
        catch (Exception e) { MelonLogger.Error(e); }
    }

}