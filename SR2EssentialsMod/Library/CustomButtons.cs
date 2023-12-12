using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using Il2CppMonomiPark.SlimeRancher.UI.Pause;
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