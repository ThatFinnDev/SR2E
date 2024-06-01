namespace SR2E.Commands;

public class NewBucksCommand : SR2Command
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
        if (args == null || args.Length != 1) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();

        int amount = 0;
        if (!int.TryParse(args[0], out amount))
        { SendError(args[0] + " is not a valid integer!"); return false; }


        int newNewBuckAmount = Mathf.Clamp(amount + SceneContext.Instance.PlayerState._model.currency, 0, int.MaxValue);
        SceneContext.Instance.PlayerState._model.SetCurrency(newNewBuckAmount);
        SceneContext.Instance.PlayerState._model.SetCurrencyEverCollected(newNewBuckAmount);
        SendMessage($"Successfully addded {amount} newbucks");
        return true;
    }


}
