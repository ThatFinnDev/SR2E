using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Persist;

namespace SR2E.Patches.SaveFixer;

[HarmonyPatch(typeof(GameModelPushHelpers), nameof(GameModelPushHelpers.PushPlacedGadgets))]
internal static class SaverFixerPushGadgets
{
    internal static void Postfix(GameModel gameModel, ref List<PlacedGadgetV05> placedGadgets, ILoadReferenceTranslation loadReferenceTranslation)
    {
        try {
            if (!SR2EEntryPoint.disableFixSaves)
            {
                //Remove invalid Actors
                var modifiedGadgets = new List<PlacedGadgetV05>(placedGadgets);
                foreach (var gadget in placedGadgets)
                    if(!loadReferenceTranslation.TryGetIdentifiableTypeFromReferenceId(gadget.Identifier, out IdentifiableType type))
                        modifiedGadgets.Remove(gadget);
                placedGadgets = modifiedGadgets;
            }
        }
        catch (Exception e) { MelonLogger.Error(e); }
    }
	
}