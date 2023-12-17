namespace SR2E.Commands;

public class RefillSlotsCommand : SR2CCommand
{
    public override string ID => "refillslots";
    public override string Usage => "refillslots";
    public override string Description => "Refills all of your slots";

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        return null;
    }

    public override bool Execute(string[] args)
    {
        if (args != null)
        {
            SR2Console.SendError($"The '<color=white>{ID}</color>' command takes no arguments");
            return false;
        }

        if (!inGame) { SR2Console.SendError("Load a save first!"); return false; }

        for (int i = 0; i < SceneContext.Instance.PlayerState.Ammo.Slots.Count; i++)
        {
            Ammo.Slot slot = SceneContext.Instance.PlayerState.Ammo.Slots[i];
            if (slot.IsUnlocked)
                if (slot.Id != null)
                    slot.Count = SceneContext.Instance.PlayerState.Ammo._ammoModel.GetSlotMaxCount(slot.Id, i);
        }

        SR2Console.SendMessage("Successfully refilled your slots");

        return true;
    }

    public override bool SilentExecute(string[] args)
    {
        if (!inGame) return true;

        for (int i = 0; i < SceneContext.Instance.PlayerState.Ammo.Slots.Count; i++)
        {
            Ammo.Slot slot = SceneContext.Instance.PlayerState.Ammo.Slots[i];
            if (slot.IsUnlocked)
                if (slot.Id != null)
                    slot.Count = SceneContext.Instance.PlayerState.Ammo._ammoModel.GetSlotMaxCount(slot.Id, i);
        }

        return true;
    }
}
    