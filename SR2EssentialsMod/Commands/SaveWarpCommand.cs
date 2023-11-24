using System.Collections.Generic;
using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.Regions;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using MelonLoader;
using UnityEngine;

namespace SR2E.Commands
{
    public class SaveWarpCommand : SR2CCommand
    {
        public override string ID => "setwarp";
        public override string Usage => "setwarp <name>";
        public override string Description => "Saves your location, so you can teleport to it later!";
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

            if (args.Length != 1)
            { SR2Console.SendMessage($"Usage: {Usage}"); return false; }
            
            if (SceneContext.Instance == null) { SR2Console.SendError("Load a save first!"); return false; }
            if (SceneContext.Instance.PlayerState == null) { SR2Console.SendError("Load a save first!"); return false; }

            string name = args[0];
            if (SR2Warps.warps.ContainsKey(name))
            { SR2Console.SendError($"There is already a warp with the name: {name}"); return false; }

            Vector3 pos = SceneContext.Instance.Player.transform.position;
            Quaternion rotation = SceneContext.Instance.Player.transform.rotation;
            string sceneGroup = SceneContext.Instance.Player.GetComponent<RegionMember>().SceneGroup.ReferenceId;
            
            SR2Warps.warps.Add(name,new Warp(sceneGroup,pos,rotation));
            SR2Warps.SaveWarps();
            
            SR2Console.SendMessage($"Successfully added the Warp: {name}");
            return true;
        }
    }
}