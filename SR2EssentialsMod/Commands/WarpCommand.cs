<<<<<<< HEAD
﻿namespace SR2E.Commands;

public class WarpCommand : SR2Command
{
    public override string ID => "warp";
    public override string Usage => "warp <location>";
=======
﻿using SR2E.Enums;
using SR2E.Managers;
using SR2E.Storage;

namespace SR2E.Commands;

internal class WarpCommand : SR2ECommand
{
    public override string ID => "warp";
    public override string Usage => "warp <location>";
    public override CommandType type => CommandType.Warp;
>>>>>>> experimental

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
        {
            List<string> warps = new List<string>();
<<<<<<< HEAD
            foreach (KeyValuePair<string, SR2ESaveManager.Warp> pair in SR2ESaveManager.data.warps) warps.Add(pair.Key);
            return warps;
        }

        return null;
    }

=======
            foreach (KeyValuePair<string, Warp> pair in SR2ESaveManager.data.warps) warps.Add(pair.Key);
            return warps;
        }
        return null;
    }
>>>>>>> experimental
    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,1)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();
        string name = args[0];
<<<<<<< HEAD
        SR2ESaveManager.Warp warp = SR2ESaveManager.WarpManager.GetWarp(name);
=======
        Warp warp = SR2EWarpManager.GetWarp(name);
>>>>>>> experimental
        if (warp == null) return SendError(translation("cmd.warpstuff.nowarpwithname",name));

        SR2EError error = warp.WarpPlayerThere();
        switch (error)
        {
<<<<<<< HEAD
            case SR2EError.NoError:
                SendMessage(translation("cmd.warp.success",name));
                return true;
            case SR2EError.NotInGame: return SendLoadASaveFirst();
            case SR2EError.PlayerNull: return SendLoadASaveFirst();
            case SR2EError.TeleportablePlayerNull: return SendError(translation("cmd.error.teleportableplayernull"));
            case SR2EError.SRCharacterControllerNull: return SendError(translation("cmd.error.srccnull"));
            case SR2EError.SceneGroupNotSupported: return SendError(translation("cmd.error.scenegroupnotsupported",warp.sceneGroup));
            case SR2EError.DoesntExist: return SendError(translation("cmd.warpstuff.nowarpwithname",name));
        }

        SendError(translation("cmd.warp.unknownerror"));
        return false;
=======
            case SR2EError.NoError: SendMessage(translation("cmd.warp.success",name)); return true;
            case SR2EError.NotInGame: return SendLoadASaveFirst();
            case SR2EError.PlayerNull: return SendLoadASaveFirst();
            case SR2EError.TeleportablePlayerNull: return SendNullTeleportablePlayer();
            case SR2EError.SRCharacterControllerNull: return SendNullSRCharacterController();
            case SR2EError.SceneGroupNotSupported: return SendUnsupportedSceneGroup(warp.sceneGroup);
            case SR2EError.DoesntExist: return SendError(translation("cmd.warpstuff.nowarpwithname",name));
        }
        return SendUnknown();
>>>>>>> experimental

    }

}