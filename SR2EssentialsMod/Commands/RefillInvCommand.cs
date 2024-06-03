namespace SR2E.Commands;

public class RefillInvCommand : SR2Command
{
    public override string ID => "refillinv";
    public override string Usage => "refillinv [slot]";
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
        int slotToFill = -1;
        if (args != null && args.Length == 1)
        {
            try { slotToFill = int.Parse(args[0]); }
            catch { SendError(translation("cmd.error.notvalidint",args[0])); return false; }

            if (slotToFill<=0) { SendError(translation("cmd.error.notintabove",args[0])); return false; }
            if(slotToFill>numberOfSlots) { SendError(translation("cmd.refillinv.error.slotdoesntexist",numberOfSlots)); return false; } }

        slotToFill -= 1;
        if(slotToFill==-1)
        {
            for (int i = 0; i < SceneContext.Instance.PlayerState.Ammo.Slots.Count; i++)
            {
                Ammo.Slot slot = SceneContext.Instance.PlayerState.Ammo.Slots[i];
                if (slot.IsUnlocked)
                    if (slot.Id != null)
                        slot.Count = SceneContext.Instance.PlayerState.Ammo._ammoModel.GetSlotMaxCount(slot.Id, i);
            }
            SendMessage(translation("cmd.refillinv.success"));
            return true;
        }

        bool isUnlocked = SceneContext.Instance.PlayerState.Ammo.Slots[slotToFill].IsUnlocked;
        if (!isUnlocked)
        {
            
            SendMessage(translation("cmd.refillinv.error.slotnotunlocked",slotToFill+1));
            return false;
        }

        if (SceneContext.Instance.PlayerState.Ammo.Slots[slotToFill].Id==null)
        {
            SendMessage(translation("cmd.refillinv.error.slotempty",slotToFill+1));
            return false;
        }
        SceneContext.Instance.PlayerState.Ammo.Slots[slotToFill].Clear();
        SendMessage(translation("cmd.refillinv.successsingle",slotToFill+1));
        return true;
    }
    /*
    public override bool Execute(string[] args)
    {
        if (args != null) return SendNoArguments();
        if (!inGame) return SendLoadASaveFirst();

        for (int i = 0; i < SceneContext.Instance.PlayerState.Ammo.Slots.Count; i++)
        {
            Ammo.Slot slot = SceneContext.Instance.PlayerState.Ammo.Slots[i];
            if (slot.IsUnlocked)
                if (slot.Id != null)
                    slot.Count = SceneContext.Instance.PlayerState.Ammo._ammoModel.GetSlotMaxCount(slot.Id, i);
        }

        SendMessage("Successfully refilled your slots");

        return true;
    }*/
}
    