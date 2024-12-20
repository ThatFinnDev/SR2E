using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.ScriptedValue;
using Il2CppMonomiPark.SlimeRancher.Options;
using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using UnityEngine.Localization;
using SR2E.Patches.MainMenu;
using System;
namespace SR2E.Buttons;

[RegisterTypeInIl2Cpp(false)]
public class CustomSettingsButton : ScriptedValuePresetOptionDefinition
{
    public static Dictionary<SettingsCategory, List<CustomSettingsButton>> AllSettingsButtons = new Dictionary<SettingsCategory, List<CustomSettingsButton>>()
    {
        { SettingsCategory.GameSettings, new List<CustomSettingsButton>() },
        { SettingsCategory.Display, new List<CustomSettingsButton>() },
        { SettingsCategory.Audio, new List<CustomSettingsButton>() },
        { SettingsCategory.Bindings_Controller, new List<CustomSettingsButton>() },
        { SettingsCategory.Bindings_Keyboard, new List<CustomSettingsButton>() },
        { SettingsCategory.Input, new List<CustomSettingsButton>() },
        { SettingsCategory.Gameplay_InGame, new List<CustomSettingsButton>() },
        { SettingsCategory.Gameplay_MainMenu, new List<CustomSettingsButton>() },
        { SettingsCategory.Graphics, new List<CustomSettingsButton>() },
    };

    static List<string> allUsedIDs = new List<string>();
    
    public struct OptionValue
    {
        public string id;
        public LocalizedString name;
        public ScriptedInt valueInt = null;
        public ScriptedFloat valueFloat = null;
        public ScriptedBool valueBool = null;

        public OptionValue(string id, LocalizedString name, ScriptedBool storedValue)
        {
            this.id = id;
            this.name = name;
            valueBool = storedValue;
        }
        public OptionValue(string id, LocalizedString name, ScriptedFloat storedValue)
        {
            this.id = id;
            this.name = name;            
            valueFloat = storedValue;

        }
        public OptionValue(string id, LocalizedString name, ScriptedInt storedValue)
        {
            this.id = id;
            this.name = name;            
            valueInt = storedValue;

        }
    }

    public static ScriptedInt CreateScriptedInt(int value)
    {
        var scripted = CreateInstance<ScriptedInt>();
        scripted.Value = value;
        return scripted;
    }
    public static ScriptedFloat CreateScriptedFloat(float value)
    {
        var scripted = CreateInstance<ScriptedFloat>();
        scripted.Value = value;
        return scripted;
    }
    public static ScriptedBool CreateScruptedBool(bool value)
    {
        var scripted = CreateInstance<ScriptedBool>();
        scripted.Value = value;
        return scripted;
    }
    
    public int index { get; private set; }

    public enum SettingsCategory
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
    }
    public static void ApplyButtons(SettingsCategory categoryEnum, OptionsItemCategory categoryObject)
    {
        foreach (var settingsButton in AllSettingsButtons[categoryEnum])
        {
            categoryObject.items.Insert(settingsButton.index, settingsButton);
        }
    }
    
    public static CustomSettingsButton Create(SettingsCategory category, LocalizedString label, LocalizedString description, int insertIndex, string id, bool applyImmediately, bool confirm,  params OptionValue[] values)
    {
        var button = CreateInstance<CustomSettingsButton>();
        
        if (allUsedIDs.Contains(id))
        {
            throw new Exception("A custom settings button already has this id: " + id);
        }
        
        button._wrapAround = true;
        button._referenceId = $"setting.{id}";
        button._label = label;
        button._detailsText = description;
        button.index = insertIndex;
        button._applyImmediately = applyImmediately;
        button._requireConfirmation = confirm;
        
        Il2CppReferenceArray<ScriptedValuePresetOptionDefinition.ScriptedValuePreset> presets = new Il2CppReferenceArray<ScriptedValuePreset>(values.Length);

        int i = 0;
        foreach (var value in values)
        {
            var preset = new ScriptedValuePreset();

            preset._presetLabel = value.name;
            preset._referenceId = value.id;
            
            if (value.valueBool)
            {
                var scripedOption = new ScriptedValueSetting<ScriptedBool, bool>(ClassInjector.DerivedConstructorPointer<ScriptedValueSetting<ScriptedBool, bool>>());

                var scriptedOptions = new Il2CppReferenceArray<ScriptedValueSetting<ScriptedBool, bool>>(1);
                
                scriptedOptions[0] = scripedOption;

                scriptedOptions[0]._scriptedValue = value.valueBool;

                preset._scriptedBoolSettings = scriptedOptions;
            }
            else if (value.valueInt)
            {

                var scripedOption = new ScriptedValueSetting<ScriptedInt, int>(ClassInjector.DerivedConstructorPointer<ScriptedValueSetting<ScriptedInt, int>>());
                    
                var scriptedOptions = new Il2CppReferenceArray<ScriptedValueSetting<ScriptedInt, int>>(1)
                {
                };

                scriptedOptions[0] = scripedOption;
                
                scriptedOptions[0]._scriptedValue = value.valueInt;

                preset._scriptedIntSettings = scriptedOptions;
            }
            else if (value.valueFloat)
            {
                var scripedOption = new ScriptedValueSetting<ScriptedFloat, float>(ClassInjector.DerivedConstructorPointer<ScriptedValueSetting<ScriptedFloat, float>>());

                var scriptedOptions = new Il2CppReferenceArray<ScriptedValueSetting<ScriptedFloat, float>>(1);
                
                scriptedOptions[0] = scripedOption;

                scriptedOptions[0]._scriptedValue = value.valueFloat;

                preset._scriptedFloatSettings = scriptedOptions;
            }
            else throw new InvalidCastException($"Value type for {id} isn't supported. valid types are: bool, float, and int.");

            presets[i] = preset;
        }

        button._optionsPresets = presets;
        
        allUsedIDs.Add(id);
        AllSettingsButtons[category].Add(button);
        
        return button;
    }
}
