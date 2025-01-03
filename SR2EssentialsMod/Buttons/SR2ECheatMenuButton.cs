using Il2CppTMPro;
using SR2E.Menus;
using UnityEngine.UI;

namespace SR2E.Buttons;

public class SR2ECheatMenuButton
{
    public string label;
    public System.Action action;
    public Button buttonInstance;
    public TextMeshProUGUI textInstance;
    public SR2ECheatMenuButton(string label, System.Action action)
    {
        this.label = label;
        this.action = action;

        foreach (SR2ECheatMenuButton entry in SR2ECheatMenu.cheatButtons)
            if (entry.label == this.label) { MelonLogger.Error($"There is already a button with the name {this.label}"); return; }

        SR2ECheatMenu.cheatButtons.Add(this);
    }
}