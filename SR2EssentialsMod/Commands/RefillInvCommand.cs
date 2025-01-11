namespace SR2E.Commands;

internal class RefillInvCommand : SR2ECommand
{
    public override string ID => "refillinv";
    public override string Usage => "refillinv [slot]";
    public override CommandType type => CommandType.Cheat;

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return new List<string> { "1", "2", "3", "4" };
        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0, 1)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();

        int numberOfSlots = SceneContext.Instance.PlayerState.Ammo.Slots.Length - 1;
        int slotToFill = -1;

        if(args!=null)
            try
            {
                slotToFill = int.Parse(args[0]);
                if (slotToFill <= 0) return SendError(translation("cmd.error.notintabove", args[0]));
                if (slotToFill > numberOfSlots) return SendError(translation("cmd.refillinv.error.slotdoesntexist", numberOfSlots));
                slotToFill -= 1;
            }
            catch { return SendNotValidInt(args[0]); }
        if (args==null)
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
            return SendError(translation("cmd.refillinv.error.slotnotunlocked", slotToFill + 1));


        if (SceneContext.Instance.PlayerState.Ammo.Slots[slotToFill].Id == null)
            return SendError(translation("cmd.refillinv.error.slotempty", slotToFill + 1));

        Ammo.Slot invSlot = SceneContext.Instance.PlayerState.Ammo.Slots[slotToFill];
        invSlot.Count = SceneContext.Instance.PlayerState.Ammo._ammoModel.GetSlotMaxCount(invSlot.Id, slotToFill);
        SendMessage(translation("cmd.refillinv.successsingle", slotToFill + 1));
        return true;
    }
}
    