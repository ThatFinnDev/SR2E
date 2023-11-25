using System.Collections.Generic;
using System.Linq;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.Regions;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.SlimeRancher.World.Teleportation;
using Il2CppSystem;
using MelonLoader;
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
            if (argIndex == 0)
            {
                List<string> warps = new List<string>();
                foreach (KeyValuePair<string,Warp> pair in SR2Warps.warps)
                { warps.Add(pair.Key); }
                return warps;
            }

            return null;
        }
        public override bool Execute(string[] args)
        {
            if (SceneContext.Instance == null) { SR2Console.SendError("Load a save first!"); return false; }
            if (SceneContext.Instance.PlayerState == null) { SR2Console.SendError("Load a save first!"); return false; }

            if (args == null)
            { SR2Console.SendMessage($"Usage: {Usage}"); return false; }

            if (args.Length != 1)
            { SR2Console.SendMessage($"Usage: {Usage}"); return false; }
            
            string name = args[0];
            if (!SR2Warps.warps.ContainsKey(name))
            { SR2Console.SendError($"There is no warp with the name: {name}"); return false; }

            Warp warp = SR2Warps.warps[name];
            TeleportablePlayer p = SceneContext.Instance.Player.GetComponent<TeleportablePlayer>();
            if(p==null)
            { SR2Console.SendError($"TeleportablePlayer is null!"); return false; }

            SRCharacterController cc = SceneContext.Instance.Player.GetComponent<SRCharacterController>();
            foreach (SceneGroup group in SystemContext.Instance.SceneLoader.SceneGroupList.items)
                if (group.IsGameplay)
                    if (group.ReferenceId == warp.sceneGroup)
                    {
                        if (warp.sceneGroup == p.SceneGroup.ReferenceId)
                        {
                            cc.Position = warp.position;
                            cc.Rotation = warp.rotation;
                            cc.Velocity = Vector3.zero;
                        }
                        else
                        {
                            Il2CppArrayBase<StaticTeleporterNode> nodes = Object.FindObjectsOfType<StaticTeleporterNode>();
                            bool noToBreak = false;
                            foreach (StaticTeleporterNode node in nodes)
                            {
                                if (noToBreak)
                                    break;
                                try
                                {
                                    if (node.NodeId != null)
                                        switch (warp.sceneGroup)
                                        {
                                            case "SceneGroup.ConservatoryFields":
                                                if(true)
                                                {
                                                    SR2Warps.warpTo = warp;
                                                    GameObject prefab = SR2EEntryPoint.getIdentifiableByName("TeleporterHomeBlue").prefab;
                                                    GameObject obj = GameObject.Instantiate(prefab, cc.Position, Quaternion.identity);
                                                    obj.SetActive(true);
                                                    GadgetTeleporterNode telNode = SR2Console.getObjRec<GadgetTeleporterNode>(obj.transform, "Teleport Collider");
                                                    telNode.GetComponent<StaticTeleporterNode>()._hasDestination = true;
                                                    telNode.GetComponent<StaticTeleporterNode>().UpdateFX();
                                                    noToBreak = true;
                                                }
                                                break;
                                            case "SceneGroup.RumblingGorge":
                                                if(true)
                                                {
                                                    SR2Warps.warpTo = warp;
                                                    GameObject prefab = SR2EEntryPoint.getIdentifiableByName("TeleporterZoneGorge").prefab;
                                                    GameObject obj = GameObject.Instantiate(prefab, cc.Position, Quaternion.identity);
                                                    obj.SetActive(true);
                                                    GadgetTeleporterNode telNode = SR2Console.getObjRec<GadgetTeleporterNode>(obj.transform, "Teleport Collider");
                                                    telNode.GetComponent<StaticTeleporterNode>()._hasDestination = true;
                                                    telNode.GetComponent<StaticTeleporterNode>().UpdateFX();
                                                    noToBreak = true;
                                                }
                                                break;
                                            case "SceneGroup.LuminousStrand":
                                                if(true)
                                                {
                                                    SR2Warps.warpTo = warp;
                                                    GameObject prefab = SR2EEntryPoint.getIdentifiableByName("TeleporterZoneStrand").prefab;
                                                    GameObject obj = GameObject.Instantiate(prefab, cc.Position, Quaternion.identity);
                                                    obj.SetActive(true);
                                                    GadgetTeleporterNode telNode = SR2Console.getObjRec<GadgetTeleporterNode>(obj.transform, "Teleport Collider");
                                                    telNode.GetComponent<StaticTeleporterNode>()._hasDestination = true;
                                                    telNode.GetComponent<StaticTeleporterNode>().UpdateFX();
                                                    noToBreak = true;
                                                }
                                                break;
                                            case "SceneGroup.PowderfallBluffs":
                                                if(true)
                                                {
                                                    SR2Warps.warpTo = warp;
                                                    GameObject prefab = SR2EEntryPoint.getIdentifiableByName("TeleporterZoneBluffs").prefab;
                                                    GameObject obj = GameObject.Instantiate(prefab, cc.Position, Quaternion.identity);
                                                    obj.SetActive(true);
                                                    GadgetTeleporterNode telNode = SR2Console.getObjRec<GadgetTeleporterNode>(obj.transform, "Teleport Collider");
                                                    telNode.GetComponent<StaticTeleporterNode>()._hasDestination = true;
                                                    telNode.GetComponent<StaticTeleporterNode>().UpdateFX();
                                                    noToBreak = true;
                                                }
                                                break;
                                            
                                        }
                                    
                                }catch (Exception e) {}
                            }
                        }
                        if(SR2Console.isOpen)
                            SR2Console.Close();
                        SR2Console.SendMessage($"Successfully warped to the warp {name}!"); 
                        return true;
                    }
                
            
            SR2Console.SendError("An unknown error occured!");
            return false;

        }
        
        static T Get<T>(string name) where T : UnityEngine.Object => Resources.FindObjectsOfTypeAll<T>().FirstOrDefault((T x) => x.name == name);

    }
}