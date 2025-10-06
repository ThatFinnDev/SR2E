using System.Linq;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Persist;

namespace SR2E.Patches.SaveFixer;

[HarmonyPatch(typeof(GameModelPushHelpers), nameof(GameModelPushHelpers.PushPlacedGadgets))]
internal static class SaverFixerPushGadgets
{
    internal static void Postfix(GameModel gameModel, ref Il2CppSystem.Collections.Generic.List<PlacedGadgetV05> placedGadgets, ILoadReferenceTranslation loadReferenceTranslation)
    {
        try {
            if (!SR2EEntryPoint.disableFixSaves)
            {
                //Remove invalid Actors
                var modifiedGadgets = placedGadgets;
                foreach (var gadget in placedGadgets._items.ToList())
                    try
                    {
                        if(gadget.Identifier!="SRAD"&&gadget.Identifier!="SRPG") 
                            if(!loadReferenceTranslation.TryGetIdentifiableTypeFromReferenceId(gadget.Identifier, out IdentifiableType type))
                                modifiedGadgets.Remove(gadget);
                            
                    }
                    catch (Exception e)
                    {
                        if(gadget!=null) MelonLogger.Error(e);
                        modifiedGadgets.Remove(gadget);
                    }
                        
                placedGadgets = modifiedGadgets;
            }
        }
        catch (Exception e) { MelonLogger.Error(e); }
    }
	
}