using Il2CppMonomiPark.SlimeRancher.Player;

namespace SR2E.Commands;

internal class ClearInventoryCommand : SR2ECommand
{
    public override string ID => "clearinv";
    public override string Usage => "clearinv [slot]";
    public override CommandType type => CommandType.Cheat;
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return new List<string> { "1", "2", "3", "4", "5", "6"};
        return null;
    }
    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0,1)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();

        int numberOfSlots = sceneContext.PlayerState.Ammo.Slots.Length - 1;
        int slotToClear = -1;
        if (args!=null)
        {
            try { slotToClear = int.Parse(args[0]); }
            catch { SendError(translation("cmd.error.notvalidint",args[0])); return false; }
            if (slotToClear<=0) return SendNotValidInt(args[0]); 
            if(slotToClear>numberOfSlots) return SendError(translation("cmd.clearinv.error.slotdoesntexist",numberOfSlots));
            slotToClear -= 1;
        }

        if(slotToClear==-1)
        {
            foreach (AmmoSlot slot in sceneContext.PlayerState.Ammo.Slots)
                if(slot.IsUnlocked) slot.Clear();
            SendMessage(translation("cmd.clearinv.success"));
            return true;
        }

        bool isUnlocked = sceneContext.PlayerState.Ammo.Slots[slotToClear].IsUnlocked;
        if (!isUnlocked) return SendError(translation("cmd.clearinv.error.slotnotunlocked",slotToClear+1));
           
        
        sceneContext.PlayerState.Ammo.Slots[slotToClear].Clear();
        SendMessage(translation("cmd.clearinv.successsingle",slotToClear+1));
        return true;
    }
}