using System;
using Il2CppMonomiPark.SlimeRancher.Options;
using Il2CppMonomiPark.SlimeRancher.UI.Options;
using SR2E.Buttons.OptionsUI;
using SR2E.Storage;

namespace SR2E.Patches.Options;

[HarmonyPatch(typeof(OptionsUIRoot), nameof(OptionsUIRoot.SwapCategory))]
internal static class OptionsUIRootSwapCategoryPatch
{
    public static void Prefix(OptionsUIRoot __instance, int categoryIndex)
    {
        if (__instance.optionsItemModels == null) __instance.optionsItemModels = new Il2CppSystem.Collections.Generic.List<IOptionsItemModel>();
        if (!InjectOptionsButtons.HasFlag()) return;
        var c = __instance.categories[categoryIndex];
        OptionsItemCategory category = c.TryCast<OptionsItemCategory>();
        if (category == null) return;
        foreach (var def in category.items.ToNetList())
        {
            if (def!=null)
                if (def is CustomOptionsValuesDefinition customDef && def.ReferenceId.StartsWith("setting.sr2eexclude"))
                {
                    IOptionsItemModel model = null;
                    if (customDef._optionsItemModels.Count > 0 && customDef._optionsItemModels[0] != null)
                        model = customDef._optionsItemModels[0].TryCast<IOptionsItemModel>();
                    else
                    {
                        try
                        {
                            model = customDef.CreateOptionItemModel().TryCast<IOptionsItemModel>();
                        }catch (Exception e) { MelonLogger.Error(e); }
                    };
                    if (!string.IsNullOrWhiteSpace(customDef.button.saveid))
                    {
                        var value = SR2EOptionsButtonManager.GetValuesButton(customDef.button.saveid, customDef.button.defaultValueIndex);
                        customDef.SetTempPresetIndex(value);
                        if (model != null)
                            model.ApplyTemporaryValue();
                    }
                    if(model!=null)
                        if (!__instance.optionsItemModels.Contains(model))
                        {
                            __instance.optionsItemModels.Insert(Math.Clamp(customDef.button.insertIndex,0,__instance.optionsItemModels.Count),model);
                        }
                }
        }
    }
}