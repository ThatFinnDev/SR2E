using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu.Definition;
using UnityEngine.Localization;
using SR2E.Patches.MainMenu;
namespace SR2E.Buttons;

public class CustomMainMenuButton
{
    public LocalizedString label;
    public Sprite icon;
    public int insertIndex;
    internal CustomMainMenuItemDefinition _definition;
    public System.Action action;

    public CustomMainMenuButton(LocalizedString label, Sprite icon, int insertIndex, System.Action action)
    {
        this.label = label;
        this.icon = icon;
        this.insertIndex = insertIndex;
        this.action = action;


        foreach (CustomMainMenuButton entry in MainMenuLandingRootUIInitPatch.buttons)
            if (entry.label == this.label) { MelonLogger.Error($"There is already a button with the name {this.label}"); return; }

        MainMenuLandingRootUIInitPatch.buttons.Add(this);
        if (SR2EEntryPoint.mainMenuLoaded)
        {
            MainMenuLandingRootUI mainMenu = Object.FindObjectOfType<MainMenuLandingRootUI>();
            if (mainMenu != null)
            {
                mainMenu.gameObject.SetActive(false);
                mainMenu.gameObject.SetActive(true);
            }
        }
    }
}
