/*using Il2CppMonomiPark.SlimeRancher.DataModel;
using SR2E.Buttons.OptionsUI;

namespace SR2E.Patches.Options;

[HarmonyPatch(typeof(PresetOptionsItemModel), nameof(PresetOptionsItemModel.RebuildOptions))]
internal static class PresetOptionsItemModelPatch
{
    internal static bool Prefix(PresetOptionsItemModel __instance)
    {
        if (__instance._presetOptionsItemDefinition!=null)
        {
            if (__instance._presetOptionsItemDefinition is CustomOptionsUIValuesDefinition) return false;
            if (__instance._presetOptionsItemDefinition._referenceId.StartsWith("setting.sr2eexclude")) return false;
        }
        return true;
    }

    
}*/