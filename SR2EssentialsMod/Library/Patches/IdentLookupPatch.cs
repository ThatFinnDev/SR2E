using System.Reflection;
using HarmonyLib;
using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Persist;

namespace CottonLibrary.Patches;
/*
[HarmonyPatch(typeof(IdentifiableTypePersistenceIdReverseLookupTable), nameof(IdentifiableTypePersistenceIdReverseLookupTable.GetIdentifiableType))]
public class IdentLookupPatch
{
    public static void Prefix(IdentifiableTypePersistenceIdReverseLookupTable __instance, int persistenceId)
    {
        foreach (var ident in Library.savedIdents.Where(ident => !__instance._indexTable.Contains(ident.Key)))
            __instance._indexTable = __instance._indexTable.AddString(ident.Key);
    }
}
*/