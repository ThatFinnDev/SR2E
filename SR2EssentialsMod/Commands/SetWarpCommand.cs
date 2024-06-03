﻿using Il2CppMonomiPark.SlimeRancher.Regions;

namespace SR2E.Commands;

public class SetWarpCommand : SR2Command
{
    public override string ID => "setwarp";
    public override string Usage => "setwarp <name>";


    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,1)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst(); 

        string name = args[0];

        Vector3 pos = SceneContext.Instance.Player.transform.position;
        Quaternion rotation = SceneContext.Instance.Player.transform.rotation;
        string sceneGroup = SceneContext.Instance.Player.GetComponent<RegionMember>().SceneGroup.ReferenceId;

        SR2EError error = SR2ESaveManager.WarpManager.AddWarp(name, new SR2ESaveManager.Warp(sceneGroup, pos, rotation));
        if (error == SR2EError.AlreadyExists)
            return SendError(translation("cmd.warpstuff.alreadywarpwithname",name));

        SendMessage(translation("cmd.setwarp.success",name));
        return true;
    }
}
