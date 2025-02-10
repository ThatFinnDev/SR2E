using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.Regions;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.World.Teleportation;
using SR2E.Enums;
using SR2E.Storage;

namespace SR2E.Managers;

public static class SR2EWarpManager
{
    internal static Dictionary<string, StaticTeleporterNode> teleporters = new Dictionary<string, StaticTeleporterNode>();
    internal static Warp warpTo = null;

    public static SR2EError AddWarp(string warpName, Warp warp)
    {
        if (SR2ESaveManager.data.warps.ContainsKey(warpName)) return SR2EError.AlreadyExists;
        SR2ESaveManager.data.warps.Add(warpName, warp);
        SR2ESaveManager.Save();
        return SR2EError.NoError;
    }

    public static Warp GetWarp(string warpName)
    {
        if (!SR2ESaveManager.data.warps.ContainsKey(warpName)) return null;
        return SR2ESaveManager.data.warps[warpName];
    }

    public static SR2EError RemoveWarp(string warpName)
    {
        if (!SR2ESaveManager.data.warps.ContainsKey(warpName)) return SR2EError.DoesntExist;
        SR2ESaveManager.data.warps.Remove(warpName);
        SR2ESaveManager.Save();
        return SR2EError.NoError;
    }
    internal static void OnSceneUnloaded()
    {
        if(warpTo==null) return;
        if (SceneContext.Instance == null) { warpTo = null; return; }
        if (SceneContext.Instance.PlayerState == null) { warpTo = null; return; }
        
        foreach (SceneGroup group in SystemContext.Instance.SceneLoader.SceneGroupList.items)
            if (group.IsGameplay) if (group.ReferenceId == warpTo.sceneGroup)
                if (warpTo.sceneGroup == SceneContext.Instance.Player.GetComponent<RegionMember>().SceneGroup.ReferenceId)
                {
                    SRCharacterController cc = SceneContext.Instance.Player.GetComponent<SRCharacterController>();
                    cc.Position = warpTo.position;
                    cc.Rotation = warpTo.rotation;
                    cc.BaseVelocity = Vector3.zero;
                    warpTo = null;
                    break;
                }
    }
}