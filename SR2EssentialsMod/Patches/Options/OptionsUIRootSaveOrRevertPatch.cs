using System;
using Il2CppMonomiPark.SlimeRancher.Options;
using Il2CppMonomiPark.SlimeRancher.UI.Options;
using SR2E.Buttons.OptionsUI;
using SR2E.Storage;

namespace SR2E.Patches.Options;

[HarmonyPatch(typeof(OptionsUIRoot), nameof(OptionsUIRoot.SaveOrRevert))]
internal static class OptionsUIRootSaveOrRevertPatch
{
    public static void Prefix(OptionsUIRoot __instance)
    {
        if (__instance.optionsItemModels == null) __instance.optionsItemModels = new Il2CppSystem.Collections.Generic.List<IOptionsItemModel>();
    }
}