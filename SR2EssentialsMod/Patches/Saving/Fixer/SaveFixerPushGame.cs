using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Persist;

namespace SR2E.Patches.Saving.Fixer;

[HarmonyPriority(-99999999)]
[HarmonyPatch(typeof(GameModelPushHelpers), nameof(GameModelPushHelpers.PushGame))]
internal static class SaveFixerPushGame
{
    static bool needsRemoving(int integer,ILoadReferenceTranslation r)
    {
        try { if (r.GetIdentifiableType(integer) == null) return true; }
        catch (Exception e) { return true; }
        return false;
    }
    internal static void Prefix(ActorIdProvider actorIdProvider, ISaveReferenceTranslation saveReferenceTranslation, GameV09 gameState, GameModel gameModel)
    {
        if (!SR2EEntryPoint.disableFixSaves)
            try {
                var loadTranslation = saveReferenceTranslation.toNonIVariant().CreateLoadReferenceTranslation(gameState);
                foreach (var id in gameState.Drone.Cloud.IDs.ToArray())
                {
                    try {
                        if (needsRemoving(id,loadTranslation))
                            gameState.Drone.Cloud.IDs.Remove(id);
                    }
                    catch (Exception e) { MelonLogger.Error(e); }
                }
            }
            catch (Exception e) { MelonLogger.Error(e); }
    }

}