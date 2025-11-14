/*using Il2CppInterop.Runtime.Injection;
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
using Il2CppMonomiPark.SlimeRancher.UI.Options;
using Il2CppMonomiPark.SlimeRancher.Util;
using QualityLevel = Il2CppMonomiPark.ScriptedValue.QualityLevel;

namespace SR2E.Buttons;

public static class CustomSettingsCreator
{
    

    /// <summary>
    /// Delegate that is called when a setting is either loaded into frame or edited
    /// </summary>
    public delegate void OnSettingEdited(ScriptedValuePresetOptionDefinition option, int valueIndex, bool savedChange = false);

    // Using this to reset the options model each main menu load.
    static Il2CppSystem.Collections.Generic.IEnumerable<OptionsItemDefinition> optionsItemDefinitions;
    
    internal static void ApplyModel()
    {
        if (optionsItemDefinitions == null)
            optionsItemDefinitions = gameContext.OptionsDirector._optionsItemDefinitionsProvider.defaultAsset.ProfileBasedDefinitions;

        gameContext.OptionsDirector._optionsModel = new OptionsModel(optionsItemDefinitions);
        
        foreach (var setting in settingModels)
        {
            gameContext.OptionsDirector._optionsModel.optionsById.Add(setting.Key, setting.Value);
        }

        var newGameUI = Resources.FindObjectsOfTypeAll<NewGameRootUI>().First();
        foreach (var setting in gameModels)
        {
            newGameUI._optionsItemDefinitionsProvider.GetAssetForCurrentPlatform()._gameBasedOptionItems.Add(setting.Value.optionsItemDefinition);
        }
    }

    static Dictionary<string, OptionsItemModel> settingModels = new Dictionary<string, OptionsItemModel>();
    static Dictionary<string, OptionsItemModel> gameModels = new Dictionary<string, OptionsItemModel>();

    /// <summary>
    /// Struct for an option's preset value.
    /// </summary>
    public struct OptionValue
    {
        /// <summary>
        /// Preset value ID
        /// </summary>
        public readonly string id;
        
        /// <summary>
        /// Localized name for the value
        /// </summary>
        public readonly LocalizedString name;
        
        /// <summary>
        /// Scripted integer value (if any)
        /// </summary>
        public ScriptedInt valueInt = null;
        
        
        /// <summary>
        /// Scripted float value (if any)
        /// </summary>
        public ScriptedFloat valueFloat = null;
        
        
        /// <summary>
        /// Scripted boolean value (if any)
        /// </summary>
        public ScriptedBool valueBool = null;

        
        /// <summary>
        /// Actual integer value (if any)
        /// </summary>
        public int actualInt = int.MinValue;
        
        /// <summary>
        /// Actual float value (if any)
        /// </summary>
        public float actualFloat = float.MinValue;
        
        /// <summary>
        /// Actual boolean value (if any)
        /// </summary>
        public bool actualBool = false;
        
        /// <summary>
        /// Constructor for a boolean option value
        /// </summary>
        /// <param name="id">The value's ID</param>
        /// <param name="name">The value's localized name</param>
        /// <param name="storedValue">The variable to edit</param>
        /// <param name="value">The actual value for this option</param>
        public OptionValue(string id, LocalizedString name, ScriptedBool storedValue, bool value)
        {
            this.id = id;
            this.name = name;
            valueBool = storedValue;
            actualBool = value;
        }

        /// <summary>
        /// Constructor for a floating point option value
        /// </summary>
        /// <param name="id">The value's ID</param>
        /// <param name="name">The value's localized name</param>
        /// <param name="storedValue">The variable to edit</param>
        /// <param name="value">The actual value for this option</param>
        public OptionValue(string id, LocalizedString name, ScriptedFloat storedValue, float value)
        {
            this.id = id;
            this.name = name;
            valueFloat = storedValue;
            actualFloat = value;
        }

        /// <summary>
        /// Constructor for a integer option value
        /// </summary>
        /// <param name="id">The value's ID</param>
        /// <param name="name">The value's localized name</param>
        /// <param name="storedValue">The variable to edit</param>
        /// <param name="value">The actual value for this option</param>
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

    /// <summary>
    /// Only run before injecting buttons!!!
    /// </summary>
    internal static void ClearUsedIDs()
    {
        allUsedIDs.Clear();
    }

    /// <summary>
    /// Create a custom settings option that gets injected into the game.
    /// </summary>
    /// <param name="category">The category to inject the setting into. There are 2 special values. The first is GameSettings, which gets injected into the new save ui, the other is ManualOrCustom which doesnt inject into anything</param>
    /// <param name="label">The name of the option that gets shown</param>
    /// <param name="description">The description shown while the setting is selected</param>
    /// <param name="id">The reference ID of the setting</param>
    /// <param name="defaultOptionIndex"> The default option index when the user hasnt chosen an option yet</param>
    /// <param name="applyImmediately">Whether or not to apply as soon as the value is changed</param> // please fact check this
    /// <param name="confirm">Whether or not to use a confirmation prompt for this option</param> // and this too
    /// <param name="modifyCallback">The action you want to run when the value is changed or the category containing the option is opened</param>
    /// <param name="values">params Array of SR2E option value structs</param>
    /// <returns>A custom settings option</returns>
    public static ScriptedValuePresetOptionDefinition Create(
        BuiltinSettingsCategory category, 
        LocalizedString label, 
        LocalizedString description,
        string id, 
        int defaultOptionIndex, 
        bool applyImmediately, 
        bool wrapAround,
        bool confirm, 
        OnSettingEdited modifyCallback,
        params OptionValue[] values)
    {
        var button = Object.Instantiate(Resources.FindObjectsOfTypeAll<ScriptedValuePresetOptionDefinition>().First());

        if (allUsedIDs.Contains(id))
            return null;

        button.name = label.GetLocalizedString().Replace(" ", "");
        button._wrapAround = wrapAround;
        button._referenceId = $"setting.{id}";
        button._label = label;
        button._detailsText = description;
        button._applyImmediately = applyImmediately;
        button._requireConfirmation = confirm;
        button._defaultValueIndex = defaultOptionIndex;
        
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
                
                var scriptedOptionsU1 = new Il2CppReferenceArray<ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedFloat, float>>(0);
                var scriptedOptionsU2 = new Il2CppReferenceArray<ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedInt, int>>(0);           
                var scriptedOptionsU3 = new Il2CppSystem.Collections.Generic.List<ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedQuality, QualityLevel>>(0);

                
                scriptedOptions[0] = scripedOption;
                
                scriptedOptions[0]._scriptedValue = value.valueBool;
                scriptedOptions[0]._value = value.actualBool;
                
                preset._scriptedBoolSettings = scriptedOptions;
                preset._scriptedFloatSettings = scriptedOptionsU1;
                preset._scriptedIntSettings = scriptedOptionsU2;
                preset._scriptedQualitySettings = scriptedOptionsU3;
            }
            else if (value.valueInt)
            {
                var scripedOption = new ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedInt, int>();
                
                var scriptedOptions = new Il2CppReferenceArray<ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedInt, int>>(1);
                
                var scriptedOptionsU1 = new Il2CppReferenceArray<ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedFloat, float>>(0);
                var scriptedOptionsU2 = new Il2CppReferenceArray<ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedBool, bool>>(0);         
                var scriptedOptionsU3 = new Il2CppSystem.Collections.Generic.List<ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedQuality, QualityLevel>>(0);

                
                scriptedOptions[0] = scripedOption;
                
                scriptedOptions[0]._scriptedValue = value.valueInt;
                scriptedOptions[0]._value = value.actualInt;

                preset._scriptedIntSettings = scriptedOptions;
                preset._scriptedBoolSettings = scriptedOptionsU2;
                preset._scriptedFloatSettings = scriptedOptionsU1;
                preset._scriptedQualitySettings = scriptedOptionsU3;
            }
            else if (value.valueFloat)
            {

                var scripedOption = new ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedFloat, float>();

                var scriptedOptions = new Il2CppReferenceArray<ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedFloat, float>>(1);
                
                var scriptedOptionsU1 = new Il2CppReferenceArray<ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedBool, bool>>(0);
                var scriptedOptionsU2 = new Il2CppReferenceArray<ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedInt, int>>(0);
                var scriptedOptionsU3 = new Il2CppSystem.Collections.Generic.List<ScriptedValuePresetOptionDefinition.ScriptedValueSetting<ScriptedQuality, QualityLevel>>(0);

                scriptedOptions[0] = scripedOption;

                scriptedOptions[0]._scriptedValue = value.valueFloat;   
                scriptedOptions[0]._value = value.actualFloat;


                preset._scriptedFloatSettings = scriptedOptions;
                preset._scriptedBoolSettings = scriptedOptionsU1;
                preset._scriptedIntSettings = scriptedOptionsU2;
                preset._scriptedQualitySettings = scriptedOptionsU3;
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
        
        button._optionsItemModels._items[0]._presets = presetsSet;
        
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

    /// <summary>
    /// Creates a custom setting category that gets injected into the options menu.
    /// </summary>
    /// <param name="label">The category name</param>
    /// <param name="icon">The sprite used as the category icon shown when the category is not selected</param>
    /// <param name="options">
    /// The array of settings that gets injected. To get a good array, follow this example
    /// <code>
    /// List&lt;ScriptedValuePresetOptionDefinition&gt; options =
    ///     new List&lt;ScriptedValuePresetOptionDefinition&gt;();
    /// 
    /// options.Add(CustomSettingsCreator.Create(
    ///     CustomSettingsCreator.BuiltinSettingsCategory.ManualOrCustom,
    ///     // input rest of parameters 
    /// ));
    /// 
    /// // second option
    /// options.Add(CustomSettingsCreator.Create(
    ///     CustomSettingsCreator.BuiltinSettingsCategory.ManualOrCustom,
    ///     // input rest of parameters 
    /// ));
    ///
    /// // Category
    /// CustomSettingsCreator.CreateCategory(
    ///     AddTranslationFromSR2E("setting.categoryname", "l.sr2ecategory", "UI"), SR2EUtils.ConvertToSprite(SR2EUtils.LoadImage("category")),
    /// options.ToArray());
    /// </code>
    /// </param>
    /// <returns></returns>
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
}*/