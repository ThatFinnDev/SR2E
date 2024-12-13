namespace SR2E.Commands;

public class GiveCommand : SR2Command
{
    public override string ID => "give";
    public override string Usage => "give <item> [amount]";

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return getIdentListByPartialName(args == null ? null : args[0], true,SR2EUtils.vaccableGroup);
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
        if (type == null) return SendError(translation("cmd.error.notvalididenttype", identifierTypeName));
        string itemName = type.getName();
        if (type.isGadget()) return SendError(translation("cmd.give.isgadgetnotitem",itemName));
        

        int amount = 1;
        if (args.Length == 2)
        {
            try { amount = int.Parse(args[1]); }
            catch { return SendError(translation("cmd.error.notvalidint",args[1])); }
            if (amount <= 0) return SendError(translation("cmd.error.notintabove",args[1],0));
        }


        for (int i = 0; i < amount; i++)
            SceneContext.Instance.PlayerState.Ammo.MaybeAddToSlot(type, null);


        SendMessage(translation("cmd.give.success",amount,itemName));
        return true;
    }
}
