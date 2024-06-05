namespace SR2E.Commands;

public class WarpCommand : SR2Command
{
    public override string ID => "warp";
    public override string Usage => "warp <location>";

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
        if (!inGame) return SendLoadASaveFirst();
        string name = args[0];
        SR2ESaveManager.Warp warp = SR2ESaveManager.WarpManager.GetWarp(name);
        if (warp == null) return SendError(translation("cmd.warpstuff.nowarpwithname",name));

        SR2EError error = warp.WarpPlayerThere();
        switch (error)
        {
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

    }

}