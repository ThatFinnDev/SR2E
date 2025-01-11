namespace SR2E.Commands;

internal class FastForwardCommand : SR2ECommand
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
        float duration;
        if (args.Length == 1)
            if (!this.TryParseFloat(args[0], out duration, 0, false)) return false;
            else timeToFastForwardTo = SceneContext.Instance.TimeDirector.HoursFromNow(duration);
        
        SceneContext.Instance.TimeDirector.FastForwardTo(timeToFastForwardTo);
        SendMessage(translation("cmd.fastforward.success",timeToFastForwardTo));
        return true;
    }

}
