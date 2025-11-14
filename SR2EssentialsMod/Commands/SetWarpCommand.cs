using Il2CppMonomiPark.SlimeRancher.Regions;
using SR2E.Enums;
using SR2E.Managers;
using SR2E.Storage;

namespace SR2E.Commands;

internal class SetWarpCommand : SR2ECommand
{
    public override string ID => "setwarp";
    public override string Usage => "setwarp <name>";
    public override CommandType type => CommandType.Warp;
    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,1)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst(); 

        string name = args[0];

        Vector3 pos = sceneContext.Player.transform.position;
        Quaternion rotation = sceneContext.Player.transform.rotation;
        string sceneGroup = sceneContext.RegionRegistry.CurrentSceneGroup.ReferenceId;

        SR2EError error = SR2EWarpManager.AddWarp(name, new Warp(sceneGroup, pos, rotation));
        if (error == SR2EError.AlreadyExists) return SendError(translation("cmd.warpstuff.alreadywarpwithname",name));

        SendMessage(translation("cmd.setwarp.success",name));
        return true;
    }
}
