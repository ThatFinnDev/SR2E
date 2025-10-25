using SR2E.Enums;
using SR2E.Managers;
using UnityEngine.InputSystem;

namespace SR2E.Commands;
internal class UnbindCommand : SR2ECommand
{
    public override string ID => "unbind";
    public override string Usage => "unbind <key>";
    public override CommandType type => CommandType.Binding;
    
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0) return LookupEUtil.GetLKeyStringListByPartialName(args[0],true,MAX_AUTOCOMPLETE.Get());
        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,1)) return SendUsage();

        LKey key;
        if (!TryParseLKey(args[0], out key)) return false;
        
        if (!SR2EBindingManger.isKeyBound(key)) return SendError(translation("cmd.unbind.notbound", args[0]));
        SR2EBindingManger.UnbindKey(key);
        SendMessage(translation("cmd.unbind.success", key));
        return true;
    }
}