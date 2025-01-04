using SR2E.Managers;
using UnityEngine.InputSystem;
using Key = SR2E.Enums.Key;

namespace SR2E.Commands;
internal class UnbindCommand : SR2ECommand
{
    public override string ID => "unbind";
    public override string Usage => "unbind <key>";
    public override CommandType type => CommandType.Binding;
    
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return getKeyListByPartialName(args[0],true);

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
            if (!SR2EBindingManger.isKeyBound(key))
                return SendError(translation("cmd.unbind.notbound", args[0]));

            SR2EBindingManger.UnbindKey(key);
            SendMessage(translation("cmd.unbind.success", key));
            return true;
        }

        return SendError(translation("cmd.error.notvalidkeycode", args[0]));
       
    }
}