<<<<<<< HEAD
﻿namespace SR2E.Commands;

public class DeleteWarpCommand : SR2Command
{
    public override string ID => "delwarp";
    public override string Usage => "delwarp <name>";
=======
﻿using SR2E.Enums;
using SR2E.Managers;
using SR2E.Storage;

namespace SR2E.Commands;

internal class DeleteWarpCommand : SR2ECommand
{
    public override string ID => "delwarp";
    public override string Usage => "delwarp <name>";
    public override CommandType type => CommandType.Warp;
>>>>>>> experimental

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
        {
            List<string> warps = new List<string>();
<<<<<<< HEAD
            foreach (KeyValuePair<string, SR2ESaveManager.Warp> pair in SR2ESaveManager.data.warps) warps.Add(pair.Key);
=======
            foreach (KeyValuePair<string, Warp> pair in SR2ESaveManager.data.warps) warps.Add(pair.Key);
>>>>>>> experimental
            return warps;
        }

        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,1)) return SendUsage();

<<<<<<< HEAD
        switch (SR2ESaveManager.WarpManager.RemoveWarp(args[0]))
=======
        switch (SR2EWarpManager.RemoveWarp(args[0]))
>>>>>>> experimental
        {
            case SR2EError.DoesntExist: return SendError(translation("cmd.warpstuff.nowarpwithname",args[0]));
            default: SendMessage(translation("cmd.delwarp.success",args[0])); return true;
        }
    }
}
