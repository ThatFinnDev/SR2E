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
using Il2CppMonomiPark.SlimeRancher.UI.Options;namespace SR2E.Buttons;

public static class CustomSettingsCreator
{
    public delegate void OnSettingEdited(ScriptedValuePresetOptionDefinition option, int valueIndex, bool savedChange = false);
    
    internal static void ApplyModel()
    {
        foreach (var setting in settingModels)
        {
            GameContext.Instance.OptionsDirector._optionsModel.optionsById.Add(setting.Key, setting.Value);
        }

        var newGameUI = Resources.FindObjectsOfTypeAll<NewGameRootUI>().First();
        foreach (var setting in gameModels)
        {
            newGameUI._optionsItemDefinitionsProvider.GetAssetForCurrentPlatform()._gameBasedOptionItems.Add(setting.Value.optionsItemDefinition);
        }
    }

    static Dictionary<string, OptionsItemModel> settingModels = new Dictionary<string, OptionsItemModel>();
    static Dictionary<string, OptionsItemModel> gameModels = new Dictionary<string, OptionsItemModel>();

    public struct OptionValue
    {
        public readonly string id;
        public readonly LocalizedString name;
        public ScriptedInt valueInt = null;
        public ScriptedFloat valueFloat = null;
        public ScriptedBool valueBool = null;

        public int actualInt = int.MinValue;
        public float actualFloat = float.MinValue;
        public bool actualBool = false;
        
        public OptionValue(string id, LocalizedString name, ScriptedBool storedValue, bool value)
        {
            this.id = id;
            this.name = name;
            valueBool = storedValue;
            actualBool = value;
        }

        public OptionValue(string id, LocalizedString name, ScriptedFloat storedValue, float value)
        {
            this.id = id;
            this.name = name;
            valueFloat = storedValue;
            actualFloat = value;
        }

        public OptionValue(string id, LocalizedString name, ScriptedInt storedValue, int value)
        {
            this.id = id;
            this.name = name;
            valueInt = storedValue;
            actualInt = value;
        }
    }

    public static ScriptedInt CreateScriptedInt(int value)
    {
        var scripted = ScriptableObject.CreateInstance<ScriptedInt>();
        scripted.Value = value;
        return scripted;
    }

    public static ScriptedFloat CreateScriptedFloat(float value)
    {
        var scripted = ScriptableObject.CreateInstance<ScriptedFloat>();
        scripted.Value = value;
        return scripted;
    }

    public static ScriptedBool CreateScruptedBool(bool value)
    {
        var scripted = ScriptableObject.CreateInstance<ScriptedBool>();
        scripted.Value = value;
        return scripted;
    }

    public static Dictionary<string, List<ScriptedValuePresetOptionDefinition>> AllSettingsButtons =
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
        };

    static List<string> allUsedIDs = new List<string>();

    public enum BuiltinSettingsCategory
    {
        GameSettings,
        Display,
        Audio,
        Bindings_Controller,
        Input,
        Gameplay_MainMenu,
        Gameplay_InGame,
        Bindings_Keyboard,
        Graphics,

        /// <summary>
        /// This requires you to add it to the category yourself. Use this for custom categories
        /// </summary>
        ManualOrCustom,
    }

    public static void ApplyButtons(BuiltinSettingsCategory categoryEnum, OptionsItemCategory categoryObject)
    {
        foreach (var settingsButton in AllSettingsButtons[categoryEnum.ToString()])
        {
            categoryObject.items.Add(settingsButton);
        }
    }

    public static ScriptedValuePresetOptionDefinition Create(BuiltinSettingsCategory category, 
        LocalizedString label, 
        LocalizedString description,
        string id, 
        bool applyImmediately, 
        bool confirm, 
        OnSettingEdited modifyCallback,
        params OptionValue[] values)
    {
        var button = Object.Instantiate(Resources.FindObjectsOfTypeAll<ScriptedValuePresetOptionDefinition>().First());
        
        if (allUsedIDs.Contains(id))
            throw new Exception("A custom settings button already has this id: " + id);

        button._wrapAround = true;
        button._referenceId = $"setting.{id}";
        button._label = label;
        button._detailsText = description;
        button._applyImmediately = applyImmediately;
        button._requireConfirmation = confirm;
        Il2CppReferenceArray<ScriptedValuePresetOptionDefinition.ScriptedValuePreset> presets = new Il2CppReferenceArray<ScriptedValuePresetOptionDefinition.ScriptedValuePreset>(values.Length);
        int i = 0;
        foreach (var value in values)
        {
            var preset = new ScriptedValuePresetOptionDefinition.ScriptedValuePreset();
            preset._presetLabel = value.name;
            preset._referenceId = value.id;
            if (value.valueBool)
            {
                var scripedOption = new ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedBool, bool>();
                var scriptedOptions = new Il2CppReferenceArray<ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedBool, bool>>(1);
                scriptedOptions[0] = scripedOption;
                scriptedOptions[0]._scriptedValue = value.valueBool;
                scriptedOptions[0]._value = value.actualBool;
                preset._scriptedBoolSettings = scriptedOptions;
            }
            else if (value.valueInt)
            {
                var scripedOption = new ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedInt, int>();
                var scriptedOptions = new Il2CppReferenceArray<ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedInt, int>>(1);
                
                scriptedOptions[0] = scripedOption;
                scriptedOptions[0]._scriptedValue = value.valueInt;
                scriptedOptions[0]._value = value.actualInt;

                preset._scriptedIntSettings = scriptedOptions;
            }
            else if (value.valueFloat)
            {

                var scripedOption = new ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedFloat, float>();

                var scriptedOptions = new Il2CppReferenceArray<ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedFloat, float>>(1);

                scriptedOptions[0] = scripedOption;

                scriptedOptions[0]._scriptedValue = value.valueFloat;   
                scriptedOptions[0]._value = value.actualFloat;


                preset._scriptedFloatSettings = scriptedOptions;
            }
            else throw new InvalidCastException($"Value type for {id} isn't supported. valid types are: bool, float, and int.");

            presets[i] = preset;

            i++;
        }

        button.CreateOptionItemModel();
        button._optionsPresets = presets;

        var presets2 = new Il2CppSystem.Collections.Generic.List<OptionPresetSet.OptionPreset>();

        foreach (var preset in presets)
            presets2.Add(new OptionPresetSet.OptionPreset(preset._referenceId, preset._presetLabel));
        
        var presetsSet = new OptionPresetSet(presets2.Cast<Il2CppSystem.Collections.Generic.IEnumerable<OptionPresetSet.OptionPreset>>());
        
        button._optionsItemModels._items[0].presets = presetsSet;
        
        button._optionsItemModels._items[0].OnSelectionChanged += new System.Action<int>(i =>
        {
            modifyCallback.Invoke(button, i, true);
        });
        button._optionsItemModels._items[0].OnTempSelectionChanged += new System.Action<int>(i =>
        {
            modifyCallback.Invoke(button, i);
        });
        
        allUsedIDs.Add(id);
        if (category != BuiltinSettingsCategory.ManualOrCustom)
            AllSettingsButtons[category.ToString()].Add(button);
        
        button._isProfileSetting = category != BuiltinSettingsCategory.GameSettings;

        button._optionsItemModels._items[0].id = $"moddedSetting.{id}";
        
        if (button._isProfileSetting)
            settingModels.Add(button._referenceId, button._optionsItemModels._items[0]);
        else 
            gameModels.Add(button._referenceId, button._optionsItemModels._items[0]);
        
        return button;
    }

    public static OptionsItemCategory CreateCategory(LocalizedString label, Sprite icon,
        params ScriptedValuePresetOptionDefinition[] options)
    {
        var category = Object.Instantiate(Resources.FindObjectsOfTypeAll<OptionsItemCategory>().First());
        category._title = label;
        category.name = label.GetLocalizedString().Replace(" ", "");
        category._icon = icon;
        category.items = new Il2CppSystem.Collections.Generic.List<OptionsItemDefinition>();
        foreach (var o in options)
        {
            category.items.Add(o);
        }

        foreach (var provier in Resources.FindObjectsOfTypeAll<PlatformOptionsConfigurationProvider>())
        {
            provier.defaultAsset.items.Add(category);
        }

        return category;
    }
}