namespace SR2E.Commands;

internal class GiveCommand : SR2ECommand
{
    public override string ID => "give";
    public override string Usage => "give <item> [amount]";
    public override CommandType type => CommandType.Cheat;

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return getVaccableListByPartialName(args == null ? null : args[0], true);
        if (argIndex == 1)
            return new List<string> { "1", "5", "10", "20", "30", "50" };

        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,2)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();

        string identifierTypeName = args[0];
        IdentifiableType type = getIdentByName(identifierTypeName);
        if (type == null) return SendNotValidIdentType(identifierTypeName);
        string itemName = type.getName();
        if (type.isGadget()) return SendIsGadgetNotItem(itemName);
        
        int amount = 1;
        if (args.Length == 2) if(!this.TryParseInt(args[1], out amount,0, false)) return false;

        for (int i = 0; i < amount; i++)
            SceneContext.Instance.PlayerState.Ammo.MaybeAddToSlot(type, null);

        SendMessage(translation("cmd.give.success",amount,itemName));
        return true;
    }
}
