﻿namespace SR2E.Commands;

public class DeleteWarpCommand : SR2Command
{
    public override string ID => "delwarp";
    public override string Usage => "delwarp <name>";

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
        {
            List<string> warps = new List<string>();
            foreach (KeyValuePair<string, SR2ESaveManager.Warp> pair in SR2ESaveManager.data.warps) warps.Add(pair.Key);
            return warps;
        }

        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,1)) return SendUsage();

        switch (SR2ESaveManager.WarpManager.RemoveWarp(args[0]))
        {
            case SR2EError.DoesntExist: return SendError(translation("cmd.warpstuff.nowarpwithname",args[0]));
            default: SendMessage(translation("cmd.delwarp.success",args[0])); return true;
        }
    }
}
