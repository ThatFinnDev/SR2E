using Il2CppMonomiPark.SlimeRancher.Options;
using SR2E.Enums;
using SR2E.Patches.MainMenu;
using SR2E.Storage;
using UnityEngine.Localization;

namespace SR2E.Buttons;

public class CustomOptionsUICategory
{
    private HashSet<CustomOptionsUIButton> buttons = new ();
    public LocalizedString label;
    public int insertIndex;
    internal OptionsItemCategory _category;
    public Sprite icon;
    public OptionsUIVisibleState visibleState;
    public bool enabled = true;

    public CustomOptionsUICategory(LocalizedString label, int insertIndex, Sprite icon, OptionsUIVisibleState visibleState)
    {
        this.label = label; 
        this.insertIndex = insertIndex;
        this.icon = icon;
        this.visibleState = visibleState;

        SR2EOptionsButtonManager.customOptionsUICategories.Add(this,new HashSet<CustomOptionsUIButton>());
    }
    

    public void AddButton(CustomOptionsUIButton button)
    { 
        if(!SR2EOptionsButtonManager.customOptionsUICategories[this].Contains(button))
            SR2EOptionsButtonManager.customOptionsUICategories[this].Add(button);
    }
    public void RemoveButton(CustomOptionsUIButton button)
    { 
        if(SR2EOptionsButtonManager.customOptionsUICategories[this].Contains(button))
            SR2EOptionsButtonManager.customOptionsUICategories[this].Remove(button);
    }
    public void Remove()
    {
        enabled = false;
    }
    public void AddAgain()
    {
        enabled = true;
    }
}