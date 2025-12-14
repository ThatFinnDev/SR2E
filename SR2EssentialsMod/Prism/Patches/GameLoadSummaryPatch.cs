using Il2CppMonomiPark.SlimeRancher.Persist;
using SR2E.Prism.Lib;
using SR2E.Storage;

namespace SR2E.Prism.Patches;

[PrismPatch()]
[HarmonyPatch(typeof(GameV09), nameof(GameV09.LoadSummaryData))]
internal static class GameLoadSummaryPatch
{
    static void Prefix()
    {
        try
        {
            foreach (var actor in PrismLibSaving.savedIdents)
            {
                PrismLibSaving.RefreshIfNotFound(autoSaveDirector._saveReferenceTranslation,actor.Value);
            }
        }
        catch {}
    }
}