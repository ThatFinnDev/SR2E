using System;
using UnityEngine.InputSystem;

public class UnbindCommand : SR2Command
{
    public override string ID => "unbind";
    public override string Usage => "unbind <key>";

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
        {
            string firstArg = "";
            if (args != null)
                firstArg = args[0];
            List<string> list = new List<string>();
            foreach (string key in System.Enum.GetNames(typeof(Key)))
                if (!String.IsNullOrEmpty(key))
                    if (key != "None")
                        if (key.ToLower().Replace(" ", "").StartsWith(firstArg.ToLower()))
                            list.Add(key.Replace(" ", ""));

            return list;
        }

        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,1)) return SendUsage();

        int e;
        string keyToParse = args[0];

        if (args[0].ToCharArray().Length == 1)
            if (int.TryParse(args[0], out e))
                keyToParse = "Digit" + args[0];

        Key key;
        if (Key.TryParse(keyToParse, true, out key))
        {
            if (!SR2ESaveManager.BindingManger.isKeyBound(key))
                return SendError(translation("cmd.unbind.notbound", args[0]));

            SR2ESaveManager.BindingManger.UnbindKey(key);
            SendMessage(translation("cmd.unbind.success", key));
            return true;
        }

        return SendError(translation("cmd.error.notvalidkeycode", args[0]));
       
    }
}