using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.ScriptedValue;
using Il2CppMonomiPark.SlimeRancher.Options;
using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using UnityEngine.Localization;
using SR2E.Patches.MainMenu;
using System;
using System.Linq;
using Il2CppAssets.Script.Util.Extensions;
using Il2CppMono.Security.X509;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Platform;
using Il2CppMonomiPark.SlimeRancher.UI.Options;namespace SR2E.Buttons;[HarmonyPatch(typeof(OptionsViewModel),nameof(OptionsViewModel.GetOptionsItemsForCategory))]
public static class DetSettingsDefinitionsPatch
{
    public static void Prefix(OptionsViewModel __instance, OptionsConfiguration optionsConfiguration, OptionsItemCategory category )
    {
        foreach (var o in category.items)
        {
            if (CustomSettingsCreator.models.TryGetValue(o._referenceId, out OptionsItemModel model))
            {
             
                if (__instance.ProfileSpecificOptions != null)
                    __instance.ProfileSpecificOptions.optionsById.TryAdd(o._referenceId, model);
        
                if (__instance.GameSpecificOptions != null)
                    __instance.GameSpecificOptions.optionsById.TryAdd(o._referenceId, model);
            }
        }
    }
} 
public static class CustomSettingsCreator
{
    internal static Dictionary<string, OptionsItemModel> models = new Dictionary<string, OptionsItemModel>();
    
    public struct OptionValue
    {
        public string id;
        public LocalizedString name;
        public ScriptedInt valueInt = null;
        public ScriptedFloat valueFloat = null;
        public ScriptedBool valueBool = null;        public OptionValue(string id, LocalizedString name, ScriptedBool storedValue)
        {
            this.id = id;
            this.name = name;
            valueBool = storedValue;
        }        public OptionValue(string id, LocalizedString name, ScriptedFloat storedValue)
        {
            this.id = id;
            this.name = name;
            valueFloat = storedValue;        }        public OptionValue(string id, LocalizedString name, ScriptedInt storedValue)
        {
            this.id = id;
            this.name = name;
            valueInt = storedValue;        }
    }    public static ScriptedInt CreateScriptedInt(int value)
    {
        var scripted = ScriptableObject.CreateInstance<ScriptedInt>();
        scripted.Value = value;
        return scripted;
    }    public static ScriptedFloat CreateScriptedFloat(float value)
    {
        var scripted = ScriptableObject.CreateInstance<ScriptedFloat>();
        scripted.Value = value;
        return scripted;
    }    public static ScriptedBool CreateScruptedBool(bool value)
    {
        var scripted = ScriptableObject.CreateInstance<ScriptedBool>();
        scripted.Value = value;
        return scripted;
    }    public static Dictionary<string, List<ScriptedValuePresetOptionDefinition>> AllSettingsButtons =
        new Dictionary<string, List<ScriptedValuePresetOptionDefinition>>()
        {
            { "GameSettings", new List<ScriptedValuePresetOptionDefinition>() },
            { "Display", new List<ScriptedValuePresetOptionDefinition>() },
            { "Audio", new List<ScriptedValuePresetOptionDefinition>() },
            { "Bindings_Controller", new List<ScriptedValuePresetOptionDefinition>() },
            { "Bindings_Keyboard", new List<ScriptedValuePresetOptionDefinition>() },
            { "Input", new List<ScriptedValuePresetOptionDefinition>() },
            { "Gameplay_InGame", new List<ScriptedValuePresetOptionDefinition>() },
            { "Gameplay_MainMenu", new List<ScriptedValuePresetOptionDefinition>() },
            { "Graphics", new List<ScriptedValuePresetOptionDefinition>() },
        };    static List<string> allUsedIDs = new List<string>();    public enum BuiltinSettingsCategory
    {
        GameSettings,
        Display,
        Audio,
        Bindings_Controller,
        Input,
        Gameplay_MainMenu,
        Gameplay_InGame,
        Bindings_Keyboard,
        Graphics,        /// <summary>
        /// This requires you to add it to the category yourself. Use this for custom categories
        /// </summary>
        ManualOrCustom,
    }    public static void ApplyButtons(BuiltinSettingsCategory categoryEnum, OptionsItemCategory categoryObject)
    {
        foreach (var settingsButton in AllSettingsButtons[categoryEnum.ToString()])
        {
            categoryObject.items.Add(settingsButton);
        }
    }
    public static ScriptedValuePresetOptionDefinition Create(BuiltinSettingsCategory category, LocalizedString label, LocalizedString description, int insertIndex, string id, bool applyImmediately, bool confirm, params OptionValue[] values)
    {
        var button = Object.Instantiate(Resources.FindObjectsOfTypeAll<ScriptedValuePresetOptionDefinition>().First());        if (allUsedIDs.Contains(id))
        {
            throw new Exception("A custom settings button already has this id: " + id);
        }        button._wrapAround = true;
        button._referenceId = $"setting.{id}";
        button._label = label;
        button._detailsText = description;
        button._applyImmediately = applyImmediately;
        button._requireConfirmation = confirm;  
        Il2CppReferenceArray<ScriptedValuePresetOptionDefinition.ScriptedValuePreset> presets = 
            new Il2CppReferenceArray<ScriptedValuePresetOptionDefinition.ScriptedValuePreset>(values.Length);        int i = 0;
        foreach (var value in values)
        {
            var preset = new ScriptedValuePresetOptionDefinition.ScriptedValuePreset();            preset._presetLabel = value.name;
            preset._referenceId = value.id;            if (value.valueBool)
            {
                var scripedOption = new ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedBool, bool>();                var scriptedOptions =
                    new Il2CppReferenceArray<
                        ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedBool, bool>>(1);                scriptedOptions[0] = scripedOption;                scriptedOptions[0]._scriptedValue = value.valueBool;                preset._scriptedBoolSettings = scriptedOptions;
            }
            else if (value.valueInt)
            {                var scripedOption = new ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedInt, int>();                var scriptedOptions =
                    new Il2CppReferenceArray<
                        ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedInt, int>>(1)
                    {
                    };                scriptedOptions[0] = scripedOption;                scriptedOptions[0]._scriptedValue = value.valueInt;                preset._scriptedIntSettings = scriptedOptions;
            }
            else if (value.valueFloat)
            {
                var scripedOption =
                    new ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedFloat, float>();                var scriptedOptions =
                    new Il2CppReferenceArray<
                        ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedFloat, float>>(1);                scriptedOptions[0] = scripedOption;                scriptedOptions[0]._scriptedValue = value.valueFloat;                preset._scriptedFloatSettings = scriptedOptions;
            }
            else
                throw new InvalidCastException(
                    $"Value type for {id} isn't supported. valid types are: bool, float, and int.");            presets[i] = preset;
            i++;
        }        button.CreateOptionItemModel();        button._optionsPresets = presets;
        
        var presets2 = new Il2CppSystem.Collections.Generic.List<OptionPresetSet.OptionPreset>();
        
        foreach (var preset in presets)
            presets2.Add(new OptionPresetSet.OptionPreset(preset._referenceId, preset._presetLabel));
        var presetsSet = new OptionPresetSet(presets2.Cast<Il2CppSystem.Collections.Generic.IEnumerable<OptionPresetSet.OptionPreset>>());
        button._optionsItemModels._items[0].presets = presetsSet;
        allUsedIDs.Add(id);
        if (category != BuiltinSettingsCategory.ManualOrCustom)
            AllSettingsButtons[category.ToString()].Add(button);        models.Add(button._referenceId, button._optionsItemModels._items[0]);
        return button;
    }    public static OptionsItemCategory CreateCategory(LocalizedString label, Sprite icon, params ScriptedValuePresetOptionDefinition[] options)
    {
        var category = Object.Instantiate(Resources.FindObjectsOfTypeAll<OptionsItemCategory>().First());        category._title = label;
        category.name = label.GetLocalizedString().Replace(" ", "");
        category._icon = icon;
        category.items = new Il2CppSystem.Collections.Generic.List<OptionsItemDefinition>();
        foreach (var o in options)
        {
            category.items.Add(o);
        }        foreach (var provier in Resources.FindObjectsOfTypeAll<PlatformOptionsConfigurationProvider>())
        {
            provier.defaultAsset.items.Add(category);
        }
        
        return category;
    }
}