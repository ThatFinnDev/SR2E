using SR2E.Components;
using SR2E.Patches.General;
using UnityEngine.InputSystem;

namespace SR2E.Commands;

internal class FlatCommand : SR2ECommand
{
    public override string ID => "flat";
    public override string Usage => "flat";
    public override CommandType type => CommandType.Fun;

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0, 0)) return SendNoArguments();
        if(OptionsUIRootApplyPatch.customMasterTextureLimit==-1)
        {
            OptionsUIRootApplyPatch.customMasterTextureLimit = int.MaxValue;
            SendMessage(translation("cmd.flat.success"));
        }
        else
        {
            OptionsUIRootApplyPatch.customMasterTextureLimit = -1;
            SendMessage(translation("cmd.flat.success2"));
        }
        OptionsUIRootApplyPatch.Apply();
        return true;
    }

}