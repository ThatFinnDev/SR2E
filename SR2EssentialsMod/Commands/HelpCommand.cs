using SR2E.Managers;

namespace SR2E.Commands;

internal class HelpCommand : SR2ECommand
{
    public override string ID => "help";
    public override string Usage => "help [cmdName]";
    public override CommandType type => CommandType.Common;

    public string GetCommandDescription(string command)
    {
        if (SR2ECommandManager.commands.ContainsKey(command))
            return SR2ECommandManager.commands[command].ExtendedDescription;
        return string.Empty;
    }

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
        {
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, SR2ECommand> entry in SR2ECommandManager.commands)
                if (!entry.Value.Hidden) list.Add(entry.Key);
            return list;
        }
        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0,1)) return SendUsage();
        if (args == null)
        {
            string currText = translation("cmd.help.success")+"\n";


            foreach (KeyValuePair<string, SR2ECommand> entry in SR2ECommandManager.commands)
                if (!entry.Value.Hidden)
                    currText = $"{currText}\n{entry.Value.Usage} - {GetCommandDescription(entry.Key)}";
            SendMessage(currText);
            return true;
        }
        var desc = GetCommandDescription(args[0]);
        if (SR2ECommandManager.commands.ContainsKey(args[0]))
        {
            SendMessage(translation("cmd.help.successspecific",SR2ECommandManager.commands[args[0]].Usage,desc));
            return true;
        }
        return SendError(translation("cmd.help.notvalidcommand",args[0]));
        
    }
}