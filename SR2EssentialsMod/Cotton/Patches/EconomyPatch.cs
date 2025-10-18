using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.Economy;
using SR2E.Storage;

namespace SR2E.Cotton.Patches;

[LibraryPatch()]
[HarmonyPatch(typeof(PlortEconomyDirector), "InitModel")]
public static class EconomyPatch
{
    public static void Prefix(PlortEconomyDirector __instance)
    {
        List<PlortValueConfiguration> entries = new List<PlortValueConfiguration>();
        foreach (var defaultEntry in __instance._settings.PlortsTable.Plorts)
        {
            entries.Add(defaultEntry);
        }
        foreach (var entry in CottonLibrary.marketData)
        {
            var defualtValues = new PlortValueConfiguration()
            {
                FullSaturation = entry.Value.saturation,
                Type = entry.Key,
                InitialValue = entry.Value.value
            };
            
            entries.Add(defualtValues);
        }
        __instance._settings.PlortsTable.Plorts = new Il2CppReferenceArray<PlortValueConfiguration>(entries.ToArray());
    }
}