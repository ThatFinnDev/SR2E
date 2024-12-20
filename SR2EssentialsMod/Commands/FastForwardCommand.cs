namespace SR2E.Commands;

public class FastForwardCommand : SR2Command
{
    public override string ID => "fastforward";
    public override string Usage => "fastforward [hour amount]";
    public override CommandType type => CommandType.Cheat;

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return new List<string> { "1", "2", "5", "10", "12", "18" };
        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!inGame) return SendLoadASaveFirst();
        if (!args.IsBetween(0,1)) return SendUsage();
         
        double timeToFastForwardTo = SceneContext.Instance.TimeDirector.GetNextDawn();
        float duration = float.Parse(args[0]);
        if (args.Length == 1)
        {
            try { duration = float.Parse(args[0]); }
            catch { return SendError(translation("cmd.error.notvalidfloat",args[0])); }
            if (duration <= 0) return SendError(translation("cmd.error.notintabove", args[0], 0));
            timeToFastForwardTo = SceneContext.Instance.TimeDirector.HoursFromNow(duration);
        }

        SceneContext.Instance.TimeDirector.FastForwardTo(timeToFastForwardTo);
        SendMessage(translation("cmd.fastforward.success",timeToFastForwardTo));
        return true;
    }

}
