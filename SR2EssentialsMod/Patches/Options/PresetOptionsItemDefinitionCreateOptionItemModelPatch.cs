using Il2CppMonomiPark.SlimeRancher.Options;
using Il2CppMonomiPark.SlimeRancher.UI.Options;
using SR2E.Buttons.OptionsUI;

namespace SR2E.Patches.Options;

[HarmonyPatch(typeof(PresetOptionsItemDefinition), nameof(PresetOptionsItemDefinition.CreateOptionItemModel))]
internal static class PresetOptionsItemDefinitionCreateOptionItemModelPatch
{
    [HarmonyFinalizer]
    static Exception Finalizer(PresetOptionsItemDefinition __instance, Exception __exception)
    {
        if (!InjectOptionsButtons.HasFlag()) return __exception;
        if (__instance is CustomOptionsValuesDefinition && __instance.ReferenceId.StartsWith("setting.sr2eexclude"))
            return null;
        return __exception;
    }
}