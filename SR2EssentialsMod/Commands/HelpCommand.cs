namespace SR2E.Commands
{
    public class HelpCommand : SR2CCommand
    {
        public override string ID => "help";
        public override string Usage => "help [cmdName]";
        public override string Description => "Displays all commands available and their usage";
        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            if (argIndex==0)
            {
                List<string> list = new List<string>();
                foreach (KeyValuePair<string, SR2CCommand> entry in SR2Console.commands)
                {
                    list.Add(entry.Key);
                }
                return list;
            }
            return null;
        }
        public override bool Execute(string[] args)
        {
            if (args == null)
            {
                SR2Console.SendMessage("<color=blue>List of all currently registered commands:</color>");
                SR2Console.SendMessage("<color=blue><> is a required argument; [] is an optional argument</color>");

                foreach (KeyValuePair<string, SR2CCommand> entry in SR2Console.commands)
                {
                    SR2Console.SendMessage(entry.Value.Usage+" | "+entry.Value.Description);
                }
                return true;
            }
            if (args.Length == 1)
            {
                if (SR2Console.commands.ContainsKey(args[0]))
                {
                    SR2Console.SendMessage($"Usage: {SR2Console.commands[args[0]].Usage}");
                }
                SR2Console.SendMessage($"The key '<color=white>{args[0]}</color>' is not a valid command");
                return false;
            }
            
            SR2Console.SendMessage($"Usage: {Usage}");
            return false;
        }
    }
}