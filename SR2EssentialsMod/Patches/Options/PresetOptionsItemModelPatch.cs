using Il2CppMonomiPark.SlimeRancher.DataModel;
using SR2E.Buttons.OptionsUI;

namespace SR2E.Patches.Options;

[HarmonyPatch(typeof(PresetOptionsItemModel), nameof(PresetOptionsItemModel.RebuildOptions))]
internal static class PresetOptionsItemModelPatch
{
    [HarmonyFinalizer]
    static Exception Finalizer(PresetOptionsItemModel __instance, Exception __exception)
    {
        if (!InjectOptionsButtons.HasFlag()) return __exception;
        if (__instance._presetOptionsItemDefinition!=null)
        {
            if (__instance._presetOptionsItemDefinition is CustomOptionsValuesDefinition||
                __instance._presetOptionsItemDefinition._referenceId.StartsWith("setting.sr2eexclude"))
                return null;
        }
        return __exception;
    }

    
}