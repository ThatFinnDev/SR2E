using SR2E.Patches.InGame;
using UnityEngine.Localization;

namespace SR2E.Buttons;

public class CustomPauseMenuButton
{
    public LocalizedString label;
    public int insertIndex;
    internal CustomPauseItemModel _model;
    public System.Action action;
    public bool enabled = true;
    public CustomPauseMenuButton(LocalizedString label, int insertIndex, System.Action action)
    {
        this.label = label; ;
        this.insertIndex = insertIndex;
        this.action = action;

        foreach (CustomPauseMenuButton entry in SR2PauseMenuButtonPatch.buttons)
            if (entry.label == this.label) { MelonLogger.Error($"There is already a button with the name {this.label}"); return; }

        SR2PauseMenuButtonPatch.buttons.Add(this);
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
