namespace SR2E.Commands;

public class FastForwardCommand : SR2Command
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
        if (!inGame) return SendLoadASaveFirst();
        if (args != null && args.Length != 1) return SendUsage();
         
        double timeToFastForwardTo = SceneContext.Instance.TimeDirector.GetNextDawn();
        float duration = float.Parse(args[0]);
        if (args.Length == 1)
        {
            try { duration = float.Parse(args[0]); }
            catch { SendError(args[0] + " is not a valid float!"); return false; }
            if (duration<=0) { SendError(args[0] + " is not a float above 0!"); return false; }
            timeToFastForwardTo = SceneContext.Instance.TimeDirector.HoursFromNow(duration);
        }

        SceneContext.Instance.TimeDirector.FastForwardTo(timeToFastForwardTo);
        SendMessage($"Successfully fastforwarded {timeToFastForwardTo} hours!");
        return true;
    }

}
