namespace SR2E.Commands;

<<<<<<< HEAD
public class GiveCommand : SR2Command
{
    public override string ID => "give";
    public override string Usage => "give <item> [amount]";
=======
internal class GiveCommand : SR2ECommand
{
    public override string ID => "give";
    public override string Usage => "give <item> [amount]";
    public override CommandType type => CommandType.Cheat;
>>>>>>> experimental

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
<<<<<<< HEAD
            return getIdentListByPartialName(args == null ? null : args[0], true, false,true);
=======
            return getVaccableListByPartialName(args == null ? null : args[0], true);
>>>>>>> experimental
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
<<<<<<< HEAD
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

=======
        if (type == null) return SendNotValidIdentType(identifierTypeName);
        string itemName = type.getName();
        if (type.isGadget()) return SendIsGadgetNotItem(itemName);
        
        int amount = 1;
        if (args.Length == 2) if(!this.TryParseInt(args[2], out amount,0, false)) return false;
>>>>>>> experimental

        for (int i = 0; i < amount; i++)
            SceneContext.Instance.PlayerState.Ammo.MaybeAddToSlot(type, null);

<<<<<<< HEAD

=======
>>>>>>> experimental
        SendMessage(translation("cmd.give.success",amount,itemName));
        return true;
    }
}
