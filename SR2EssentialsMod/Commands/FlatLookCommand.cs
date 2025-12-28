using SR2E.Components;
using SR2E.Patches.General;
using SR2E.Patches.Options;
using UnityEngine.InputSystem;

namespace SR2E.Commands;

internal class FlatLookCommand : SR2ECommand
{
    public override string ID => "flatlook";
    public override string Usage => "flatlook";
    public override CommandType type => CommandType.Fun;

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0, 0)) return SendNoArguments();
        if(OptionsUIRootApplyPatch.customMasterTextureLimit==-1)
        {
            OptionsUIRootApplyPatch.customMasterTextureLimit = int.MaxValue;
            SendMessage(translation("cmd.flatlook.success"));
        }
        else
        {
            OptionsUIRootApplyPatch.customMasterTextureLimit = -1;
            SendMessage(translation("cmd.flatlook.success2"));
        }
        OptionsUIRootApplyPatch.Apply();
        return true;
    }

}