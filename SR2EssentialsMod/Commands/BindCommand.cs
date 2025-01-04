using System;
using System.Text;
using SR2E.Managers;
using UnityEngine.InputSystem;
using Key = SR2E.Enums.Key;

namespace SR2E.Commands;

internal class BindCommand : SR2ECommand
{
    public override string ID => "bind";
    public override string Usage => "bind <key> <command>";
    public override CommandType type => CommandType.Binding;

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return getKeyListByPartialName(args[0],true);
        if (argIndex == 1)
        {
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, SR2ECommand> entry in SR2ECommandManager.commands) list.Add(entry.Key);
            return list;
        }

        string secondArg = args[1];
        foreach (KeyValuePair<string, SR2ECommand> entry in SR2ECommandManager.commands)
        {
            if (entry.Key == secondArg) return entry.Value.GetAutoComplete(argIndex - 2, args);
        }

        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(2,-1)) return SendUsage();

        int e;
        string keyToParse = args[0];

        if (args[0].ToCharArray().Length == 1)
            if (int.TryParse(args[0], out e))
                keyToParse = "Digit" + args[0];

        Key key;
        if (Key.TryParse(keyToParse, true, out key))
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i < args.Length; i++)
                builder.Append(args[i] + " ");
            

            string executeString = builder.ToString();

            SR2EBindingManger.BindKey(key, executeString);
            SendMessage(translation("cmd.bind.success", executeString, key));
            return true;
        }

        SendMessage(translation("cmd.error.notvalidkeycode", args[0]));
        return false;
    }
}

