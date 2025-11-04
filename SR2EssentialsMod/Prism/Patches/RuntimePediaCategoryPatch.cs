using Il2CppMonomiPark.SlimeRancher.Pedia;
using SR2E.Cotton;
using SR2E.Prism.Data;
using SR2E.Prism.Lib;
using SR2E.Storage;

namespace SR2E.Prism.Patches;

[PrismPatch()]
[HarmonyPatch(typeof(PediaCategory), nameof(PediaCategory.GetRuntimeCategory))]
internal class RuntimePediaCategoryPatch
{
    static Dictionary<string, PrismPediaCategoryType> categories = new()
    {
        {"Slimes", PrismPediaCategoryType.Slimes},
        {"Resources", PrismPediaCategoryType.Resources},
        {"Blueprints", PrismPediaCategoryType.Blueprints},
        {"World", PrismPediaCategoryType.World},
        {"Weather", PrismPediaCategoryType.Weather},
        {"Toys", PrismPediaCategoryType.Toys},
        {"Ranch", PrismPediaCategoryType.Ranch},
        {"Science", PrismPediaCategoryType.Science},
        {"Tutorials", PrismPediaCategoryType.Tutorial},
    };
    public static void Postfix(PediaCategory __instance, ref PediaRuntimeCategory __result)
    {
        
        if (categories.TryGetValue(__instance.name, out PrismPediaCategoryType category))
        {
            foreach (var pedia in PrismaLibPedia.pediaEntryLookup[category])
            {
                if (!__result._items.Contains(pedia))
                     __result._items.Add(pedia);
            }
        }
    }
}