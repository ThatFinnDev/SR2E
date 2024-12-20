namespace SR2E.Commands;

public class TimeScaleCommand : SR2Command
{
    public override string ID => "timescale";
    public override string Usage => "timescale <scale>";
    public override CommandType type => CommandType.Cheat;


    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return new List<string> { ".25", ".5", "1", "2", "5" };
        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,1)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();

        float speed;
        if (!float.TryParse(args[0], out speed)) return SendError(translation("cmd.error.notvalidfloat"));
        if (speed < 0.25 || speed > 5) return SendError(translation("cmd.timescale.between", 0.25,5));

        SceneContext.Instance.TimeDirector._timeFactor = speed;
        Time.timeScale = speed;
        SendMessage(translation("cmd.timescale.success",speed));
        return true;
    }
}

