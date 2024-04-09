namespace SR2E.Commands;

public class ClearInventoryCommand : SR2CCommand
{
    public override string ID => "clearinv";
    public override string Usage => "clearinv";
    public override string Description => "Clears your inventory";
    public override string ExtendedDescription => "Clears your inventory, careful not to use this by mistake!";
    public override bool Execute(string[] args)
    {
        if (args != null) return SendNoArguments();
        if (!inGame) return SendLoadASaveFirst();

        foreach (Ammo.Slot slot in SceneContext.Instance.PlayerState.Ammo.Slots)
            slot.Clear();
        
        SR2EConsole.SendMessage("Successfully cleared your inventory");
        return true;
    }
    public override bool SilentExecute(string[] args)
    {
        if (!inGame) return true;
        foreach (Ammo.Slot slot in SceneContext.Instance.PlayerState.Ammo.Slots)
            slot.Clear();
        return true;
    }
}