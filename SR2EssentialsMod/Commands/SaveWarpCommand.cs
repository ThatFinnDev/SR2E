using Il2CppMonomiPark.SlimeRancher.Regions;

namespace SR2E.Commands;

public class SaveWarpCommand : SR2Command
{
    public override string ID => "setwarp";
    public override string Usage => "setwarp <name>";
    public override string Description => "Saves your location, so you can teleport to it later!";


    public override bool Execute(string[] args)
    {
        if (args == null || args.Length != 1) return SendUsage();
        if (!inGame) return SendLoadASaveFirst(); 

        string name = args[0];

        Vector3 pos = SceneContext.Instance.Player.transform.position;
        Quaternion rotation = SceneContext.Instance.Player.transform.rotation;
        string sceneGroup = SceneContext.Instance.Player.GetComponent<RegionMember>().SceneGroup.ReferenceId;

        SR2EError error =
            SR2ESaveManager.WarpManager.AddWarp(name, new SR2ESaveManager.Warp(sceneGroup, pos, rotation));
        if (error == SR2EError.DoesntExist)
        {
            SendError($"There is already a warp with the name: {name}");
            return false;
        }

        SendMessage($"Successfully added the Warp: {name}");
        return true;
    }
}
