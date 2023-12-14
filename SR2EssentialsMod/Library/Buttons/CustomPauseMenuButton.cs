using SR2E.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Localization;

namespace SR2E.Library.Buttons;

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
