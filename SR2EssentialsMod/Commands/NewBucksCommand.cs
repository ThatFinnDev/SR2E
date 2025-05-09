﻿namespace SR2E.Commands;

internal class NewBucksCommand : SR2ECommand
{
    public override string ID => "newbucks";
    public override string Usage => "newbucks <amount>";
    public override CommandType type => CommandType.Cheat;

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0) return new List<string> { "100", "1000", "10000", "100000", "1000000", "10000000" };
        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,1)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();

        int amount = 0;
        if (!this.TryParseInt(args[0], out amount)) return false;

        int newNewBuckAmount = Mathf.Clamp(amount + SceneContext.Instance.PlayerState._model.currency, 0, int.MaxValue);
        SceneContext.Instance.PlayerState._model.SetCurrency(newNewBuckAmount);
        SceneContext.Instance.PlayerState._model.SetCurrencyEverCollected(newNewBuckAmount);
        SendMessage(translation("cmd.newbucks.success",amount));
        return true;
    }


}
