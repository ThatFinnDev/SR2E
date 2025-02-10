<<<<<<< HEAD
﻿using System;
using System.Text;
using UnityEngine.InputSystem;

namespace SR2E.Commands;

public class BindCommand : SR2Command
{
    public override string ID => "bind";
    public override string Usage => "bind <key> <command>";
=======
﻿using System.Text;
using SR2E.Enums;
using SR2E.Managers;

namespace SR2E.Commands;

internal class BindCommand : SR2ECommand
{
    public override string ID => "bind";
    public override string Usage => "bind <key> <command>";
    public override CommandType type => CommandType.Binding;
>>>>>>> experimental

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return getKeyListByPartialName(args[0],true);
        if (argIndex == 1)
        {
            List<string> list = new List<string>();
<<<<<<< HEAD
            foreach (KeyValuePair<string, SR2Command> entry in SR2EConsole.commands) list.Add(entry.Key);
=======
            foreach (KeyValuePair<string, SR2ECommand> entry in SR2ECommandManager.commands) list.Add(entry.Key);
>>>>>>> experimental
            return list;
        }

        string secondArg = args[1];
<<<<<<< HEAD
        foreach (KeyValuePair<string, SR2Command> entry in SR2EConsole.commands)
=======
        foreach (KeyValuePair<string, SR2ECommand> entry in SR2ECommandManager.commands)
>>>>>>> experimental
        {
            if (entry.Key == secondArg) return entry.Value.GetAutoComplete(argIndex - 2, args);
        }

        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(2,-1)) return SendUsage();

<<<<<<< HEAD
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

            SR2ESaveManager.BindingManger.BindKey(key, executeString);
            SendMessage(translation("cmd.bind.success", executeString, key));
            return true;
        }

        SendMessage(translation("cmd.error.notvalidkeycode", args[0]));
        return false;
=======
        Key key;
        if (!this.TryParseKeyCode(args[0], out key)) return false;
        
        StringBuilder builder = new StringBuilder();
        for (int i = 1; i < args.Length; i++) builder.Append(args[i] + " ");
        
        string executeString = builder.ToString();

        SR2EBindingManger.BindKey(key, executeString);
        SendMessage(translation("cmd.bind.success", executeString, key));
        return true;
>>>>>>> experimental
    }
}

