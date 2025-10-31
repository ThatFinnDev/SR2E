using Il2CppMonomiPark.SlimeRancher.Pedia;
using SR2E.Cotton;
using SR2E.Storage;

namespace SR2E.Prism.Patches;

[PrismPatch()]
[HarmonyPatch(typeof(PediaCategory), nameof(PediaCategory.GetRuntimeCategory))]
internal class RuntimePediaCategoryPatch
{
    static Dictionary<string, CottonLibrary.Pedia.PediaCategoryType> categories = new()
    {
        {"Science", CottonLibrary.Pedia.PediaCategoryType.Science},
        {"Slimes", CottonLibrary.Pedia.PediaCategoryType.Slimes},
        {"World", CottonLibrary.Pedia.PediaCategoryType.World},
        {"Ranch", CottonLibrary.Pedia.PediaCategoryType.Ranch},
        {"Tutorials", CottonLibrary.Pedia.PediaCategoryType.Tutorial},
        {"Toys", CottonLibrary.Pedia.PediaCategoryType.Toys},
        {"Resources", CottonLibrary.Pedia.PediaCategoryType.Resources},
        {"Blueprints", CottonLibrary.Pedia.PediaCategoryType.Blueprints},
        {"Weather", CottonLibrary.Pedia.PediaCategoryType.Weather},
    };
    public static void Postfix(PediaCategory __instance, ref PediaRuntimeCategory __result)
    {
        
        if (categories.TryGetValue(__instance.name, out CottonLibrary.Pedia.PediaCategoryType category))
        {
            foreach (var pedia in CottonLibrary.Pedia.pediaEntries[category])
            {
                if (!__result._items.Contains(pedia))
                     __result._items.Add(pedia);
            }
        }
    }
}