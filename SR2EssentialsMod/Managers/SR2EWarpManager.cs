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

    /// <summary>
    /// Saves warp to be used in the warp command into a name
    /// </summary>
    /// <param name="warpName">The name for the warp</param>
    /// <param name="warp">The Warp</param>
    /// <returns>SR2EError: NoError or AlreadyExists</returns>
    public static SR2EError AddWarp(string warpName, Warp warp)
    {
        if (SR2ESaveManager.data.warps.ContainsKey(warpName)) return SR2EError.AlreadyExists;
        SR2ESaveManager.data.warps.Add(warpName, warp);
        SR2ESaveManager.Save();
        return SR2EError.NoError;
    }

    /// <summary>
    /// Gets a saved warp from a name
    /// </summary>
    /// <param name="warpName"></param>
    /// <returns>The saved warp</returns>
    public static Warp GetWarp(string warpName)
    {
        if (!SR2ESaveManager.data.warps.ContainsKey(warpName)) return null;
        return SR2ESaveManager.data.warps[warpName];
    }

    /// <summary>
    /// Removes a saved warp by its name
    /// </summary>
    /// <param name="warpName">The warp to be removed</param>
    /// <returns>SR2EError: NoError, DoesntExist</returns>
    public static SR2EError RemoveWarp(string warpName)
    {
        if (!SR2ESaveManager.data.warps.ContainsKey(warpName)) return SR2EError.DoesntExist;
        SR2ESaveManager.data.warps.Remove(warpName);
        SR2ESaveManager.Save();
        return SR2EError.NoError;
    }
    
    internal static void OnSceneLoaded()
    {
        if(warpTo==null) return;
        if (sceneContext == null) { warpTo = null; return; }
        if (sceneContext.PlayerState == null) { warpTo = null; return; }
        
        foreach (SceneGroup group in systemContext.SceneLoader.SceneGroupList.items)
            if (group.IsGameplay) if (group.ReferenceId == warpTo.sceneGroup)
                if (warpTo.sceneGroup == sceneContext.RegionRegistry.CurrentSceneGroup.ReferenceId)
                {
                    SRCharacterController cc = sceneContext.Player.GetComponent<SRCharacterController>();
                    cc.Position = warpTo.position;
                    cc.Rotation = warpTo.rotation;
                    cc.BaseVelocity = Vector3.zero;
                    warpTo = null;
                    break;
                }
    }
}