using Il2CppMonomiPark.SlimeRancher.Persist;
using SR2E.Cotton;
using SR2E.Storage;

namespace SR2E.Prism.Patches;

[PrismPatch()]
[HarmonyPatch(typeof(GameV08), nameof(GameV08.LoadSummaryData))]
public static class GameLoadSummaryPatch
{
    static void Prefix()
    {
        try
        {
            foreach (var actor in CottonLibrary.savedIdents)
            {
                CottonLibrary.Saving.RefreshIfNotFound(gameContext.AutoSaveDirector._saveReferenceTranslation,actor.Value);
            }
        }
        catch {}
    }
}