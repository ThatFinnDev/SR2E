using System;
using System.IO;
using Il2CppMonomiPark.SlimeRancher.Options;
using SR2E.Buttons;
using SR2E.Enums;
using SR2E.Saving;

namespace SR2E.Storage;

public static class SR2EOptionsButtonManager
{
    internal static Dictionary<NativeOptionsUICategory,HashSet<CustomOptionsUIButton>> customOptionsUIButtonsInNative = new();
    internal static Dictionary<CustomOptionsUICategory,HashSet<CustomOptionsUIButton>> customOptionsUICategories = new();
    static string path => Path.Combine(SR2EEntryPoint.DataPath, "customsettings.sr2ers");
    static CustomOptionsSave _save;
    internal static CustomOptionsSave save
    {
        get
        {
            if (_save == null)
            {
                try
                {
                    _save=RootSave.FromBytes<CustomOptionsSave>(File.ReadAllBytes(path));
                }
                catch { }
                if(_save==null) _save = new CustomOptionsSave();
            }
            return _save;
        }
    }

    static void Save()
    {
        try
        {
            File.WriteAllBytes(path,save.ToBytes());
        }
        catch { }
    }
    
    /// <summary>
    /// If you have a OptionsButtonValues and you've set up a saveID for saving,<br />
    /// you can use this function do set the index of the value<br />
    /// This function cannot be used with save-specific saveID's
    /// </summary>
    /// <param name="saveID">The saveID you've set</param>
    /// <param name="value">The new index</param>
    public static void SetValuesButton(string saveID, int value)
    {
        save.valueButtons[saveID] = value;
        Save();
    }
    /// <summary>
    /// If you have a OptionsButtonValues and you've set up a saveID for saving,<br />
    /// you can use this function do get the index of the value<br />
    /// This function cannot be used with save-specific saveID's<br />
    /// If you've never added the button, defaultValue will be returned
    /// </summary>
    /// <param name="saveID">The saveID you want to get</param>
    /// <param name="defaultValue">The fallback index</param>
    public static int GetValuesButton(string saveID, int defaultValue = -1)
    {
        if (save.valueButtons.ContainsKey(saveID))
            return save.valueButtons[saveID];
        return defaultValue;
    }

    internal static void InitializeValuesButton(string saveID, int value)
    {
        if(!save.valueButtons.ContainsKey(saveID))
        {
            save.valueButtons.Add(saveID, value);
            Save();
        }
    }
    internal static void GenerateMissingButtons()
    {
        if (!InjectOptionsButtons.HasFlag()) return;
        foreach (var category in customOptionsUIButtonsInNative)
            foreach (var button in category.Value)
                button.GetOptionsItemDef();
        
        foreach (var category in customOptionsUICategories)
            foreach (var button in category.Value) 
                button.GetOptionsItemDef();
        
    }
    internal static void LoadCustomOptionsButtons(string optionsConfigurationName)
    {
        var configuration = Get<OptionsConfiguration>(optionsConfigurationName);
        if (configuration == null) return;
        foreach (var categoryObj in configuration.items)
            foreach (var category in customOptionsUIButtonsInNative)
            {
                if (categoryObj.name == "Display" && category.Key != NativeOptionsUICategory.Display) continue;
                if (categoryObj.name == "Video" && category.Key != NativeOptionsUICategory.Video) continue;
                if (categoryObj.name == "Input" && category.Key != NativeOptionsUICategory.Input) continue;
                if (categoryObj.name == "BindingsKbm" && category.Key != NativeOptionsUICategory.BindingsKeyboardMouse) continue;
                if (categoryObj.name == "BindingsGamepad" && category.Key != NativeOptionsUICategory.BindingsController) continue;
                if (categoryObj.name == "Audio" && category.Key != NativeOptionsUICategory.Audio) continue;
                if (categoryObj.name == "GameplayIn_MainMenu" && category.Key != NativeOptionsUICategory.Gameplay) continue;
                if (categoryObj.name == "GameplayIn_InGame" && category.Key != NativeOptionsUICategory.Gameplay) continue;
                
                foreach (var button in category.Value)
                {
                    var def = button.GetOptionsItemDef();
                    if (!categoryObj.items.Contains(def))
                        categoryObj.items.Insert(Math.Clamp(button.insertIndex,0,configuration.items.Count),def);
                }
            }
        foreach (var category in customOptionsUICategories)
        {
            var categoryObj = category.Key._category;
            if (category.Key.visibleState != OptionsButtonType.OptionsUI &&
                category.Key.visibleState != OptionsButtonType.OptionsUIInGameOnly) continue;
            if(categoryObj==null)
            {
                categoryObj = ScriptableObject.CreateInstance<OptionsItemCategory>();
                categoryObj._title = category.Key.label;
                categoryObj.name = category.Key.label.GetCompactLocalized();
                categoryObj._icon = category.Key.icon;
                categoryObj.items = new Il2CppSystem.Collections.Generic.List<OptionsItemDefinition>();
                categoryObj._showRebindButton = false;
                category.Key._category = categoryObj;
            }
            if (!configuration.items.Contains(categoryObj))
                configuration.items.Insert(Math.Clamp(category.Key.insertIndex,0,configuration.items.Count),categoryObj);
            foreach (var button in category.Value)
            {
                var def = button.GetOptionsItemDef();
                if (!categoryObj.items.Contains(def))
                    categoryObj.items.Insert(Math.Clamp(button.insertIndex,0,configuration.items.Count),def);
            }

        }
    }
    internal class CustomOptionsSave : RootSave
    {
        [StoreInSave] internal Dictionary<string, int> valueButtons = new();
    }
}