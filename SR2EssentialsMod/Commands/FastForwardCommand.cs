namespace SR2E.Commands;

public class FastForwardCommand : SR2CCommand
{
    public override string ID => "fastforward";
    public override string Usage => "fastforward [hour amount]";
    public override string Description => "Fast forwards to next morning, or the amount of hours you request";

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return new List<string> { "1", "2", "5", "10", "12", "18" };
        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!inGame) return SendLoadASaveFirstMessage();

        double timeToFastForwardTo = SceneContext.Instance.TimeDirector.GetNextDawn();
        if ((args?.Length ?? 0) == 1)
        {
            if (float.TryParse(args[0], out var amount)) timeToFastForwardTo = SceneContext.Instance.TimeDirector.HoursFromNow(amount);
            else { SR2EConsole.SendError($"{args[0]} is not a valid floating point number!"); return false; }
        }

        SceneContext.Instance.TimeDirector.FastForwardTo(timeToFastForwardTo);
        SR2EConsole.SendMessage($"Successfully fastforwarded {timeToFastForwardTo} hours");
        return true;
    }

    public override bool SilentExecute(string[] args)
    {
        if (!inGame) return true;
        double timeToFastForwardTo = SceneContext.Instance.TimeDirector.GetNextDawn();
        if ((args?.Length ?? 0) == 1)
        {
            if (float.TryParse(args[0], out var amount)) timeToFastForwardTo = SceneContext.Instance.TimeDirector.HoursFromNow(amount);
            else return true;
        }

        SceneContext.Instance.TimeDirector.FastForwardTo(timeToFastForwardTo);
        return true;
    }
}
