namespace SR2E.Commands;

public class GiveGadgetCommand : SR2Command
{
    public override string ID => "givegadget";
    public override string Usage => "givegadget <gadget> [amount]";
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return getIdentListByPartialName(args == null ? null : args[0], false, true);
        if (argIndex == 1)
            return new List<string> { "1", "5", "10", "20", "30", "50" };

        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,2)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();


        string identifierTypeName = args[0];
        GadgetDefinition type = getGadgetDefByName(identifierTypeName);
        if(type==null) return SendError(translation("cmd.error.notvalidgadget",identifierTypeName));
        string itemName = type.getName();

        int amount = 1;
        if (args.Length == 2)
        {
            if (!int.TryParse(args[1], out amount)) return SendError(translation("cmd.error.notvalidint",args[1]));
            if (amount <= 0) return SendError(translation("cmd.error.notintabove",args[1],0));
        }

        SceneContext.Instance.GadgetDirector.AddItem(type, amount);
        SendMessage(translation("cmd.givegadget.success",amount,itemName));

        return true;
    }
}
