using System.Linq;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.Pedia;
using SR2E.Prism.Lib;
using SR2E.Storage;

namespace SR2E.Prism.Patches;

[PrismPatch()]
[HarmonyPatch(typeof(PediaEntry),nameof(PediaEntry.Highlights))]
internal class PediaHightlightPatch
{
    public static void Postfix(PediaEntry __instance, ref Il2CppReferenceArray<PediaEntryHighlight> __result)
    {
        
        if (__instance == null) return;
        if (!PrismLibPedia._additionalFactsMap.ContainsKey(__instance)) return;
        var modifiedResult = __result.ToList();
        foreach (var additionalFact in PrismLibPedia._additionalFactsMap[__instance])
        {
            var native = additionalFact.ConvertToNativeType();
            if (native.Label == null) native.Label = PrismShortcuts.emptyTranslation;
            if (native.Description == null) native.Description = PrismShortcuts.emptyTranslation;
            if (native.Icon == null) native.Icon = PrismShortcuts.unavailableIcon;
            modifiedResult.Add(native);
        }
        __result=modifiedResult.ToArray();
    }
}