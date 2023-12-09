namespace SR2E.Commands
{
    public class HelpCommand : SR2CCommand
    {
        public override string ID => "help";
        public override string Usage => "help [cmdName]";
        public override string Description => "Displays all commands available and their usage";
        public override string ExtendedDescription => "Displays all commands available and their usage, can also take a command as a input to display it by itself.";
        
        public string GetCommandDescription(string command)
        {
            if (SR2Console.commands.ContainsKey(command))
            {
                var cmd = SR2Console.commands[command];
                if (!string.IsNullOrEmpty(cmd.ExtendedDescription))
                    return cmd.ExtendedDescription;
                else
                    return cmd.Description;
            }
            else return string.Empty;
        }
        
        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            if (argIndex==0)
            {
                List<string> list = new List<string>();
                foreach (KeyValuePair<string, SR2CCommand> entry in SR2Console.commands)
                {
                    if (!entry.Value.Hidden)
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
                string currText = null;
                currText = $"<color=#45d192>List of all currently registered commands:</color>";
                currText = $"{currText}\n<color=#45d192><> is a required argument; [] is an optional argument</color>";
                currText = $"{currText}\n";


                foreach (KeyValuePair<string, SR2CCommand> entry in SR2Console.commands)
                {
                    if (!entry.Value.Hidden) currText = $"{currText}\n{entry.Value.Usage} - {GetCommandDescription(entry.Key)}";
                }
                SR2Console.SendMessage(currText);
                return true; 
            }
            if (args.Length == 1)
            {
                var desc = GetCommandDescription(args[0]);
                if (SR2Console.commands.ContainsKey(args[0]))
                {
                    SR2Console.SendMessage($"Usage: {SR2Console.commands[args[0]].Usage}\n Description: {desc}");
                    return true;
                }
                SR2Console.SendMessage($"The key '<color=white>{args[0]}</color>' is not a valid command");
                return false;
            }
            
            SR2Console.SendMessage($"Usage: {Usage}");
            return false;
        }
    }
}