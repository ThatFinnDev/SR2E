namespace SR2E.Commands;

public class ClearInventoryCommand : SR2Command
{
    public override string ID => "clearinv";
    public override string Usage => "clearinv [slot]";
    public override string Description => "Clears your inventory";
    public override string ExtendedDescription => "Clears your inventory, careful not to use this by mistake!";
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return new List<string> { "1", "2", "3", "4"};
        return null;
    }
    public override bool Execute(string[] args)
    {
        if (args != null && args.Length != 1) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();

        int numberOfSlots = SceneContext.Instance.PlayerState.Ammo.Slots.Length - 1;
        int slotToClear = -1;
        if (args != null && args.Length == 1)
        {
            try { slotToClear = int.Parse(args[0]); }
            catch { SendError(args[0] + " is not a valid integer!"); return false; }

            if (slotToClear<=0) { SendError(args[0] + " is not an integer above 0!"); return false; }
            if(slotToClear>numberOfSlots) { SendError($"There are only {numberOfSlots} slots !"); return false; } }

        slotToClear -= 1;
        if(slotToClear==-1)
        {
            foreach (Ammo.Slot slot in SceneContext.Instance.PlayerState.Ammo.Slots)
                slot.Clear();
            SendMessage("Successfully cleared your inventory");
            return true;
        }

        bool isUnlocked = SceneContext.Instance.PlayerState.Ammo.Slots[slotToClear].IsUnlocked;
        if (!isUnlocked)
        {
            SendMessage($"Slot {slotToClear+1} hasn't been unlocked!");
            return false;
        }
        SceneContext.Instance.PlayerState.Ammo.Slots[slotToClear].Clear();
        SendMessage($"Successfully cleared slot number {slotToClear+1}!");
        return true;
    }
}