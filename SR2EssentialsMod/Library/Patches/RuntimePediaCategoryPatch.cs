using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.Pedia;

namespace CottonLibrary.Patches;

[HarmonyPatch(typeof(PediaCategory), nameof(PediaCategory.GetRuntimeCategory))]
public class RuntimePediaCategoryPatch
{
    private static Dictionary<string, Library.PediaCategoryType> categories = new()
    {
        {"Science", Library.PediaCategoryType.Science},
        {"Slimes", Library.PediaCategoryType.Slimes},
        {"World", Library.PediaCategoryType.World},
        {"Ranch", Library.PediaCategoryType.Ranch},
        {"Tutorials", Library.PediaCategoryType.Tutorial},
        {"Toys", Library.PediaCategoryType.Toys},
        {"Resources", Library.PediaCategoryType.Resources},
        {"Blueprints", Library.PediaCategoryType.Blueprints},
        {"Weather", Library.PediaCategoryType.Weather},
    };
    public static void Postfix(PediaCategory __instance, ref PediaRuntimeCategory __result)
    {
        if (categories.TryGetValue(__instance.name, out Library.PediaCategoryType category))
        {
            foreach (var pedia in Library.pediaEntries[category])
            {
                if (!__result._items.Contains(pedia))
                     __result._items.Add(pedia);
            }
        }
    }
}