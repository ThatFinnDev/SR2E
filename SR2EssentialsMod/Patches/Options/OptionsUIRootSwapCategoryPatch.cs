using System;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Options;
using Il2CppMonomiPark.SlimeRancher.Persist;
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
        //It crashes if you don't use dynamic, don't ask why, probably Interop being Interop
        dynamic c = __instance.categories[categoryIndex];
        OptionsItemCategory category = null;
        if (c.TryCast<OptionsItemCategory>() != null)
            category = c.Cast<OptionsItemCategory>();
        if (category == null) return;
        foreach (var def in category.items.ToNetList())
        {
            if (def!=null)
                if (def is CustomOptionsValuesDefinition customDef && def.ReferenceId.StartsWith("setting.sr2eexclude"))
                {
                    IOptionsItemModel model = null;
                    if (customDef._optionsItemModels.Count > 0 && customDef._optionsItemModels[0] != null)
                    {
                        var modelTMP = customDef._optionsItemModels[0];
                        if(modelTMP!=null) model=modelTMP.TryCast<IOptionsItemModel>();
                    }
                    else
                    {
                        try
                        {
                            var modelTMP = customDef.CreateOptionItemModel();
                            if(modelTMP!=null) model=modelTMP.TryCast<IOptionsItemModel>();
                        }catch (Exception e) { MelonLogger.Error(e); }
                    };
                    if (!string.IsNullOrWhiteSpace(customDef.button.saveid))
                    {
                        var value = SR2EOptionsButtonManager.GetValuesButton(customDef.button.type,customDef.button.saveid, customDef.button.defaultValueIndex);
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