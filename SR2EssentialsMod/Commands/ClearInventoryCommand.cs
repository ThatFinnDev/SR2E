namespace SR2E.Commands;

public class ClearInventoryCommand : SR2Command
{
    public override string ID => "clearinv";
    public override string Usage => "clearinv [slot]";
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return new List<string> { "1", "2", "3", "4"};
        return null;
    }
    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0,1)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();

        int numberOfSlots = SceneContext.Instance.PlayerState.Ammo.Slots.Length - 1;
        int slotToClear = -1;
        if (args != null && args.Length == 1)
        {
            try { slotToClear = int.Parse(args[0]); }
            catch { SendError(translation("cmd.error.notvalidint",args[0])); return false; }

            if (slotToClear<=0) return SendError(translation("cmd.error.notintabove",args[0])); 
            if(slotToClear>numberOfSlots) return SendError(translation("cmd.clearinv.error.slotdoesntexist",numberOfSlots));
            
        }

        slotToClear -= 1;
        if(slotToClear==-1)
        {
            foreach (Ammo.Slot slot in SceneContext.Instance.PlayerState.Ammo.Slots)
                slot.Clear();
            SendMessage(translation("cmd.clearinv.success"));
            return true;
        }

        bool isUnlocked = SceneContext.Instance.PlayerState.Ammo.Slots[slotToClear].IsUnlocked;
        if (!isUnlocked)
        {
            
            SendMessage(translation("cmd.clearinv.error.slotnotunlocked",slotToClear+1));
            return false;
        }
        SceneContext.Instance.PlayerState.Ammo.Slots[slotToClear].Clear();
        SendMessage(translation("cmd.clearinv.successsingle",slotToClear+1));
        return true;
    }
}