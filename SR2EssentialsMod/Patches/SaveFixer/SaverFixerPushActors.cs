using System.Linq;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Persist;

namespace SR2E.Patches.SaveFixer;

[HarmonyPatch(typeof(GameModelPushHelpers), nameof(GameModelPushHelpers.PushActors))]
internal static class SaverFixerPushActors
{
    internal static void Postfix(GameModel gameModel, ref Il2CppSystem.Collections.Generic.List<ActorDataV02> actors, ILoadReferenceTranslation loadReferenceTranslation)
    {
        try {
            if (!SR2EEntryPoint.disableFixSaves)
            {
                //Remove invalid Actors
                var modifiedActors = actors;
                foreach (var actor in actors._items.ToList())
                    try
                    {
                        if(actor.Identifier!="SRAD"&&actor.Identifier!="SRPG") 
                            if(!loadReferenceTranslation.TryGetIdentifiableTypeFromReferenceId(actor.Identifier, out IdentifiableType type))
                                modifiedActors.Remove(actor);
                            
                    }
                    catch (Exception e)
                    {
                        if(actor!=null) MelonLogger.Error(e);
                        modifiedActors.Remove(actor);
                    }
                actors = modifiedActors;
            }
        }
        catch (Exception e) { MelonLogger.Error(e); }
    }
	
}