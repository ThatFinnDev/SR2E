namespace SR2E.Commands;

public class DeleteWarpCommand : SR2Command
{
    public override string ID => "delwarp";
    public override string Usage => "delwarp <name>";
    public override string Description => "Deletes a warp";
    public override string ExtendedDescription => "Deletes a warp from the <u>warp</u> command.";

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
        if (args == null || args.Length != 1) return SendUsage();

        string name = args[0];
        if (SR2ESaveManager.WarpManager.GetWarp(name) != null)
        {
            SR2ESaveManager.WarpManager.RemoveWarp(name);
            SendMessage($"Successfully deleted the Warp: {name}");
            return true;
        }

        SendError($"There is no warp with the name: {name}");
        return false;
    }
}
