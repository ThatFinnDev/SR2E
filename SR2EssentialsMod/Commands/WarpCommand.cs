using System.Collections.Generic;
using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.Regions;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.World.Teleportation;
using Il2CppSystem;
using UnityEngine;

namespace SR2E.Commands
{
    public class WarpCommand : SR2CCommand
    {
        public override string ID => "warp";
        public override string Usage => "warp <location>";
        public override string Description => "Warps you to a saved warping point";
        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            return null;
        }
        public override bool Execute(string[] args)
        {
            if (args == null)
            {
                SR2Console.SendError($"Usage: {Usage}");
                return false;
            }
            
            if (SceneContext.Instance == null) { SR2Console.SendError("Load a save first!"); return false; }
            if (SceneContext.Instance.PlayerState == null) { SR2Console.SendError("Load a save first!"); return false; }

            string name = args[0];
            if (!SR2Warps.warps.ContainsKey(name))
            { SR2Console.SendError($"There is no warp with the name: {name}"); return false; }

            Warp warp = SR2Warps.warps[name];
            TeleportablePlayer p = SceneContext.Instance.Player.GetComponent<TeleportablePlayer>();
            if(p==null)
            {
                SR2Console.SendError($"TeleportablePlayer is null!");
                return false;
            }

            SceneGroup currentSceneGroup = SceneContext.Instance.Player.GetComponent<RegionMember>().SceneGroup;
            foreach (SceneGroup group in SystemContext.Instance.SceneLoader.SceneGroupList.items)
                if (group.IsGameplay)
                    if (group.ReferenceId == warp.sceneGroup)
                    {
                        if (currentSceneGroup != group)
                        {
                            SystemContext.Instance.SceneLoader.UnloadSceneGroup(group, currentSceneGroup, true);
                            SystemContext.Instance.SceneLoader.LoadSceneGroup(group);
                        }
                        
                        SceneContext.Instance.Player.GetComponent<SRCharacterController>().Position = new Vector3(warp.x, warp.y, warp.z);
                        SceneContext.Instance.Player.transform.position = new Vector3(warp.x, warp.y, warp.z);
                        //p.TeleportTo(new Vector3(warp.x,warp.y,warp.z),group, null,true,true);
                        SR2Console.SendError($"Successfully warped to the warp {name}!"); 
                        return true;
                    }
                
            
            SR2Console.SendError("An unknown error occured!");
            return false;

        }
    }
}