namespace SR2E.Commands;

public class NewBucksCommand : SR2CCommand
{
    public override string ID => "newbucks";
    public override string Usage => "newbucks <amount>";
    public override string Description => "Adds or removes newbucks";

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return new List<string> { "100", "1000", "10000", "100000", "1000000", "10000000" };
        return null;
    }

    public override bool Execute(string[] args)
    {
        if (args == null)
        { SR2Console.SendMessage($"Usage: {Usage}"); return false; }

        if (args.Length != 1)
        { SR2Console.SendMessage($"Usage: {Usage}"); return false; }

        if (!SR2EUtils.inGame)
        { SR2Console.SendError("Load a save first!"); return false; }

        int amount = 0;
        if (!int.TryParse(args[0], out amount))
        { SR2Console.SendError(args[1] + " is not a valid integer!"); return false; }


        int newNewBuckAmount = Mathf.Clamp(amount + SceneContext.Instance.PlayerState._model.currency, 0, int.MaxValue);
        SceneContext.Instance.PlayerState._model.SetCurrency(newNewBuckAmount);
        SceneContext.Instance.PlayerState._model.SetCurrencyEverCollected(newNewBuckAmount);
        SR2Console.SendMessage($"Successfully addded {amount} newbucks");
        return true;
    }


}
