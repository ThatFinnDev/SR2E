using Il2CppMonomiPark.SlimeRancher.Options;
using Il2CppMonomiPark.SlimeRancher.UI.Options;
using SR2E.Buttons.OptionsUI;

namespace SR2E.Patches.Options;

[HarmonyPatch(typeof(ScriptedValuePresetOptionDefinition), nameof(ScriptedValuePresetOptionDefinition.CreateOptionItemModel))]
internal static class ScriptedValuePresetOptionDefinitionCreateOptionItemModelPatch
{
    static Exception Finalizer(ScriptedValuePresetOptionDefinition __instance, Exception __exception)
    {
        if (!InjectOptionsButtons.HasFlag()) return __exception;
        if (__instance is CustomOptionsValuesDefinition && __instance.ReferenceId.StartsWith("setting.sr2eexclude"))
            return null;
        return __exception;
    }
}