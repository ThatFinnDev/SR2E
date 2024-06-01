namespace SR2E.Commands;

public class RefillSlotsCommand : SR2Command
{
    public override string ID => "refillslots";
    public override string Usage => "refillslots";
    public override string Description => "Refills all of your slots";

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
    }
}
    