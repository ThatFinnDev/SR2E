using System.Linq;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Persist;

namespace SR2E.Patches.Saving.Fixer;

[HarmonyPriority(-99999999)]
[HarmonyPatch(typeof(GameModelPushHelpers), nameof(GameModelPushHelpers.PushGadget))]
internal static class SaveFixerPushGadget
{
    static bool needsRemoving(int integer,ILoadReferenceTranslation r)
    {
        try { if (r.GetIdentifiableType(integer) == null) return true; }
        catch (Exception e) { return true; }
        return false;
    }
    internal static bool Prefix(GameModel gameModel, ref PlacedGadgetV06 gadget, ILoadReferenceTranslation loadReferenceTranslation)
    {
        if (!SR2EEntryPoint.disableFixSaves)
            try
            { 
                if(needsRemoving(gadget.TypeId,loadReferenceTranslation)) return false;
            }
            catch (Exception e)
            {
                if(gadget!=null) MelonLogger.Error(e);
                return false;
            }
        return true;
    }
	
}