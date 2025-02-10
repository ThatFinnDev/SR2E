namespace SR2E.Commands;

internal class QuitCommand : SR2ECommand
{
    public override string ID => "quit";
    public override string Usage => "quit";
    public override CommandType type => CommandType.Common;

    private static bool secondsPassed = true;
    private const int waitTime = 10;
    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0,0)) return SendNoArguments();
        if (secondsPassed)
        {
            secondsPassed = false;
            ExecuteInSeconds(() => { secondsPassed = true;},waitTime);
            SendMessage(translation("cmd.quit.again",waitTime));
            return false;
        }
        SendMessage(translation("cmd.quit.success"));
        Application.Quit();
        return true;
    }
}