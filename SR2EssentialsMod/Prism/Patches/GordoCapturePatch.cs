using System.Linq;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using SR2E.Storage;

namespace SR2E.Prism.Patches;
/*
[PrismPatch()]
[HarmonyPatch(typeof(SnareModel),nameof(SnareModel.GetGordoIdForBait))]
public class GordoCapturePatch
{
    public static void Postfix(SnareModel __instance, ref IdentifiableType __result)
    {
        var pair = CottonSlimes.gordoBaitDict.FirstOrDefault(x => x.Key == __instance.baitTypeId.name);
            
        if (pair.Value)
            __result = pair.Value;
    }
}*/