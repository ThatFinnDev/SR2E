using Il2CppMonomiPark.SlimeRancher.UI.RanchHouse;
using SR2E.Patches.InGame;
using UnityEngine.Localization;

namespace SR2E.Buttons;

public class CustomRanchUIButton
{
    public LocalizedString label;
    public int insertIndex;
    internal RanchHouseMenuItemModel _model;
    public System.Action action;
    public bool enabled = true;

    public CustomRanchUIButton(LocalizedString label, int insertIndex, System.Action action)
    {
        this.label = label; ;
        this.insertIndex = insertIndex;
        this.action = action;

        foreach (CustomRanchUIButton entry in SR2RanchUIButtonPatch.buttons)
            if (entry.label == this.label) { MelonLogger.Error($"There is already a button with the name {this.label}"); return; }

        SR2RanchUIButtonPatch.buttons.Add(this);
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