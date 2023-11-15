using System.Collections.Generic;
using MelonLoader;

namespace SR2E.Commands
{
    public class HelpCommand : SR2CCommand
    {
        public override string ID { get; } = "help";
        public override string Usage { get; } = "help";
        public override string Description { get; } = "Displays all commands available, or an extended description of a command";
        
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
            SR2Console.SendMessage($"Usage: {Usage}");
            return false;
        }
    }
}