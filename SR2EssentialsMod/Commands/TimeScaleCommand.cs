
namespace SR2E.Commands
{
    internal class TimeScaleCommand : SR2CCommand
    {
        public override string ID => "timescale";

        public override string Usage => "timescale <scale>";

        public override string Description => "Modifies game speed";
        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            if (argIndex == 0)
                return new List<string> { ".25", ".5", "1", "2", "5"};
            return null;
        }
        public override bool Execute(string[] args)
        {
            if (args == null || args.Length != 1) return SendUsage();
            if (!inGame) return SendLoadASaveFirst();

            float speed;
            if (!float.TryParse(args[0], out speed))
            { SR2EConsole.SendError(args[0] + " is not a valid float!"); return false; }

            if (speed <0.25 || speed > 5)
            { SR2EConsole.SendError("It has to be between 0.25 and 5!"); return false; }
            SceneContext.Instance.TimeDirector._timeFactor = speed;
            Time.timeScale = speed;
            SR2EConsole.SendMessage($"Timescale is now: {speed}");
            return true;
        }
    }
}
