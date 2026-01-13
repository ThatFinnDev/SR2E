using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu.Definition;
using UnityEngine.Localization;
using SR2E.Patches.MainMenu;
using System.Collections.Generic;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu.Definition.ButtonBehavior;
using UnityEngine;

namespace SR2E.Buttons;

public class CustomMainMenuContainerButton : CustomMainMenuButton
{
    private HashSet<CustomMainMenuButton> customMainMenuButtons = new ();

    public void AddSubButton(CustomMainMenuButton button, bool removeFromCurrent = true)
    {
        if (removeFromCurrent) MainMenuLandingRootUIInitPatch.buttons[button] = new HashSet<CustomMainMenuContainerButton>();
        MainMenuLandingRootUIInitPatch.buttons[button].Add(this);
    }

    internal CustomMainMenuContainerButton(int THIS_IS_A_STUB) : base(THIS_IS_A_STUB)
    {
        
    }
    public CustomMainMenuContainerButton(LocalizedString label, Sprite icon, int insertIndex, System.Action action) : base(label, icon, insertIndex, action)
    {
    }
}
