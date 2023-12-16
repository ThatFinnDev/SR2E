using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using SR2E.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Localization;

namespace SR2E.Library.Buttons;

public class CustomMainMenuButton
{
    public LocalizedString label;
    public Sprite icon;
    public int insertIndex;
    public GameObject _prefabToSpawn;
    internal CreateNewUIItemDefinition _definition;
    public System.Action action;

    public CustomMainMenuButton(LocalizedString label, Sprite icon, int insertIndex, System.Action action)
    {
        this.label = label;
        this.icon = icon;
        this.insertIndex = insertIndex;
        this.action = action;


        foreach (CustomMainMenuButton entry in SR2MainMenuButtonPatch.buttons)
            if (entry.label == this.label) { MelonLogger.Error($"There is already a button with the name {this.label}"); return; }

        SR2MainMenuButtonPatch.buttons.Add(this);
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
