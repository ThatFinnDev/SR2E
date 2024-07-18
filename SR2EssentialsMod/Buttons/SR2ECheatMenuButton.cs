namespace SR2E.Buttons;

public class SR2ECheatMenuButton
{
    public string label;
    public int insertIndex;
    public System.Action action;
    
    public SR2ECheatMenuButton(string label, int insertIndex, System.Action action)
    {
        this.label = label; ;
        this.insertIndex = insertIndex;
        this.action = action;

        foreach (SR2ECheatMenuButton entry in SR2ECheatMenu.cheatButtons)
            if (entry.label == this.label) { MelonLogger.Error($"There is already a button with the name {this.label}"); return; }

        SR2ECheatMenu.cheatButtons.Add(this);
    }
}