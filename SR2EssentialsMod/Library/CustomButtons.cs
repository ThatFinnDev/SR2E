using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppMonomiPark.SlimeRancher.UI.Pause;
using SR2E.Patches;
using UnityEngine.Localization;

namespace SR2E.Library;
/*
public class CustomPauseMenuButton
{
    public string name;
    public string label;
    public int insertIndex;
    public System.Action action;

    public CustomPauseMenuButton(string name,string label, int insertIndex, System.Action action)
    {
        this.name = name;
        this.label = label;
        this.insertIndex = insertIndex;
        this.action = action;
    }
}*/
public class CustomMainMenuButton
{
    public string name;
    public LocalizedString label;
    public Sprite icon;
    public int insertIndex;
    internal GameObject _prefabToSpawn;
    internal CreateNewUIItemDefinition _definition;
    public System.Action action;

    public CustomMainMenuButton(string name, LocalizedString label, Sprite icon, int insertIndex, System.Action action)
    {
        this.name = name;
        this.label = label;
        this.icon = icon;
        this.insertIndex = insertIndex;
        this.action = action;
        
        
        foreach (CustomMainMenuButton entry in SR2MainMenuButtonPatch.buttons)
            if (entry.name == this.name) { MelonLogger.Error($"There is already a button with the name {this.name}"); return; }
          
        SR2MainMenuButtonPatch.buttons.Add(this);
        if (SR2EEntryPoint.mainMenuLoaded)
        {
            MainMenuLandingRootUI mainMenu = Object.FindObjectOfType<MainMenuLandingRootUI>();
            if(mainMenu!=null)
            {
                mainMenu.gameObject.SetActive(false);
                mainMenu.gameObject.SetActive(true);
            }
        }
    }
}

public class CustomPauseMenuButton
{
    public string name;
    public LocalizedString label;
    public int insertIndex;
    internal CustomPauseItemModel _model;
    public System.Action action;

    public CustomPauseMenuButton(string name, LocalizedString label, int insertIndex, System.Action action)
    {
        this.name = name;
        this.label = label; ;
        this.insertIndex = insertIndex;
        this.action = action;
        
        foreach (CustomPauseMenuButton entry in SR2PauseMenuButtonPatch.buttons)
            if (entry.name == this.name) { MelonLogger.Error($"There is already a button with the name {this.name}"); return; }

        SR2PauseMenuButtonPatch.buttons.Add(this);
    }
}

public class CustomPauseItemModel : ResumePauseItemModel
{
    public System.Action action;
    public override void InvokeBehavior()
    {
        action.Invoke();
        return;
    }
}