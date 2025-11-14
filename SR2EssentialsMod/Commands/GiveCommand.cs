namespace SR2E.Commands;

internal class GiveCommand : SR2ECommand
{
    public override string ID => "give";
    public override string Usage => "give <item> [amount]";
    public override CommandType type => CommandType.Cheat;

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return LookupEUtil.GetVaccableStringListByPartialName(args == null ? null : args[0], true,MAX_AUTOCOMPLETE.Get());
        if (argIndex == 1)
            return new List<string> { "1", "5", "10", "20", "30", "50" };

        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,2)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();

        string identifierTypeName = args[0];
        IdentifiableType type = LookupEUtil.GetIdentifiableTypeByName(identifierTypeName);
        if (type == null) return SendNotValidIdentType(identifierTypeName);
        string itemName = type.GetName();
        if (type.isGadget()) return SendIsGadgetNotItem(itemName);
        
        int amount = 1;
        if (args.Length == 2) if(!TryParseInt(args[1], out amount,1, true)) return false;

        
        for (int i = 0; i < amount; i++)
            sceneContext.PlayerState.Ammo.MaybeAddToSlot(type, null,type.GetAppearanceSet());

        SendMessage(translation("cmd.give.success",amount,itemName));
        return true;
    }
}
