using System.Linq;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using SR2E.Prism.Lib;
using SR2E.Storage;

namespace SR2E.Prism.Patches;

[PrismPatch()]
[HarmonyPatch(typeof(SnareModel),nameof(SnareModel.GetGordoIdForBait))]
internal class GordoCapturePatch
{
    public static void Postfix(SnareModel __instance, ref IdentifiableType __result)
    {
        var pair = PrismLibGordo.gordoBaitDict.FirstOrDefault(x => x.Key == __instance.baitTypeId.ReferenceId);
            
        if (pair.Value!=null)
            __result = pair.Value;
    }
}