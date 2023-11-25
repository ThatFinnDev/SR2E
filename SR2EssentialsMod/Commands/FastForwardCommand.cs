namespace SR2E.Commands
{
    public class FastForwardCommand : SR2CCommand
    {
        public override string ID => "fastforward";
        public override string Usage => "fastforward [hour amount]";
        public override string Description  => "Fast forwards to next morning, or the amount of hours you request";
        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            if (argIndex == 0)
            {
                List<string> list = new List<string>();
                list.Add("1");
                list.Add("2");
                list.Add("5");
                list.Add("10");
                return list;
            }

            return null;
        }
        public override bool Execute(string[] args)
        {
            double timeToFastForwardTo = SceneContext.Instance.TimeDirector.GetNextDawn();
            if((args?.Length ?? 0) == 1)
            {
                if(float.TryParse(args[0],out var amount))
                {
                    timeToFastForwardTo = SceneContext.Instance.TimeDirector.HoursFromNow(amount);
                }
                else
                {
                    SR2Console.SendError($"{args[0]} is not a valid floating point number!");
                    return false;
                }
            }
            SceneContext.Instance.TimeDirector.FastForwardTo(timeToFastForwardTo);
            SR2Console.SendMessage($"Successfully fastforwarded {timeToFastForwardTo} hours");
            return true;
        }
    }
}