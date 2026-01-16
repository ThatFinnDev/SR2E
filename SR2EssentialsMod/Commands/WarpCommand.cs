using SR2E.Enums;
using SR2E.Managers;
using SR2E.Storage;

namespace SR2E.Commands;

internal class WarpCommand : SR2ECommand
{
    public override string ID => "warp";
    public override string Usage => "warp <location>";
    public override CommandType type => CommandType.Warp | CommandType.Cheat;

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
        {
            List<string> warps = new List<string>();
            foreach (KeyValuePair<string, Warp> pair in SR2ESaveManager.data.warps) warps.Add(pair.Key);
            return warps;
        }
        return null;
    }
    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,1)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();
        string name = args[0];
        Warp warp = SR2EWarpManager.GetWarp(name);
        if (warp == null) return SendError(translation("cmd.warpstuff.nowarpwithname",name));

        SR2EError error = warp.WarpPlayerThere();
        switch (error)
        {
            case SR2EError.NoError: SendMessage(translation("cmd.warp.success",name)); return true;
            case SR2EError.NotInGame: return SendLoadASaveFirst();
            case SR2EError.PlayerNull: return SendLoadASaveFirst();
            case SR2EError.TeleportablePlayerNull: return SendNullTeleportablePlayer();
            case SR2EError.SRCharacterControllerNull: return SendNullSRCharacterController();
            case SR2EError.SceneGroupNotSupported: return SendUnsupportedSceneGroup(warp.sceneGroup);
            case SR2EError.DoesntExist: return SendError(translation("cmd.warpstuff.nowarpwithname",name));
        }
        return SendUnknown();

    }

}