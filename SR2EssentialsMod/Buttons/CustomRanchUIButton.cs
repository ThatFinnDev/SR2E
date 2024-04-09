using Il2CppMonomiPark.SlimeRancher.UI.RanchHouse;
using SR2E.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Localization;

namespace SR2E.Library.Buttons;

public class CustomRanchUIButton
{
    public LocalizedString label;
    public int insertIndex;
    internal RanchHouseMenuItemModel _model;
    public System.Action action;

    public CustomRanchUIButton(LocalizedString label, int insertIndex, System.Action action)
    {
        this.label = label; ;
        this.insertIndex = insertIndex;
        this.action = action;

        foreach (CustomRanchUIButton entry in SR2RanchUIButtonPatch.buttons)
            if (entry.label == this.label) { MelonLogger.Error($"There is already a button with the name {this.label}"); return; }

        SR2RanchUIButtonPatch.buttons.Add(this);
    }
}