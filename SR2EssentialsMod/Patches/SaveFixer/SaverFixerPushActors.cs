using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Persist;

namespace SR2E.Patches.SaveFixer;

[HarmonyPatch(typeof(GameModelPushHelpers), nameof(GameModelPushHelpers.PushActors))]
internal static class SaverFixerPushActors
{
    internal static void Postfix(GameModel gameModel, ref List<ActorDataV02> actors, ILoadReferenceTranslation loadReferenceTranslation)
    {
        try {
            if (!SR2EEntryPoint.disableFixSaves)
            {
                //Remove invalid Actors
                var modifiedActors = new List<ActorDataV02>(actors);
                foreach (var actor in actors)
                    if(!loadReferenceTranslation.TryGetIdentifiableTypeFromReferenceId(actor.Identifier, out IdentifiableType type))
                        modifiedActors.Remove(actor);
                actors = modifiedActors;
            }
        }
        catch (Exception e) { MelonLogger.Error(e); }
    }
	
}