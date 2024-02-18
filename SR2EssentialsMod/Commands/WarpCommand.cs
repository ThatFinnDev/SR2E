using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.World.Teleportation;

namespace SR2E.Commands
{
    public class WarpCommand : SR2CCommand
    {
        public override string ID => "warp";
        public override string Usage => "warp <location>";
        public override string Description => "Warps you to a saved warping point";
        public override string ExtendedDescription => "Warps you to a saved warping point, use <u>setwarp</u> to create more.";
        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            if (argIndex == 0)
            {
                List<string> warps = new List<string>();
                foreach (KeyValuePair<string,Warp> pair in SR2EWarps.warps)
                { warps.Add(pair.Key); }
                return warps;
            }

            return null;
        }
        public override bool Execute(string[] args)
        {
            if (args == null || args.Length != 1) return SendUsage();
            if (!inGame) return SendLoadASaveFirstMessage();
            
            string name = args[0];
            if (!SR2EWarps.warps.ContainsKey(name))
            { SR2EConsole.SendError($"There is no warp with the name: {name}"); return false; }

            Warp warp = SR2EWarps.warps[name];
            TeleportablePlayer p = SceneContext.Instance.Player.GetComponent<TeleportablePlayer>();
            if(p==null)
            { SR2EConsole.SendError($"TeleportablePlayer is null!"); return false; }

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
                            try
                            {
                                GameObject prefab = null;
                                switch (warp.sceneGroup)
                                {
                                    case "SceneGroup.ConservatoryFields":
                                        SR2EWarps.warpTo = warp; prefab = SR2EEntryPoint.getIdentifiableByName("TeleporterHomeBlue").prefab; break;
                                    case "SceneGroup.RumblingGorge":
                                        SR2EWarps.warpTo = warp; prefab = SR2EEntryPoint.getIdentifiableByName("TeleporterZoneGorge").prefab; break;
                                    case "SceneGroup.LuminousStrand":
                                        SR2EWarps.warpTo = warp; prefab = SR2EEntryPoint.getIdentifiableByName("TeleporterZoneStrand").prefab; break;
                                    case "SceneGroup.PowderfallBluffs":
                                        SR2EWarps.warpTo = warp; prefab = SR2EEntryPoint.getIdentifiableByName("TeleporterZoneBluffs").prefab; break;
                                    default:
                                        SR2EConsole.SendError($"There place {warp.sceneGroup} does not exist!"); return false;

                                }

                                if (prefab != null)
                                {
                                    GameObject teleporterCollider = prefab.transform.getObjRec<GadgetTeleporterNode>("Teleport Collider").gameObject;
                                    GameObject obj = GameObject.Instantiate(teleporterCollider, SceneContext.Instance.Player.transform.position, Quaternion.identity);
                                    obj.SetActive(true);
                                    obj.GetComponent<StaticTeleporterNode>()._hasDestination = true;
                                    obj.GetComponent<StaticTeleporterNode>().UpdateFX();
                                }
                                else
                                { SR2EConsole.SendError("An unknown error occured!"); return false; }
                            }
                            catch { }
                        }
                        if(SR2EConsole.isOpen)
                            SR2EConsole.Close();
                        SR2EConsole.SendMessage($"Successfully warped to the warp {name}!"); 
                        return true;
                    }
                
            
            SR2EConsole.SendError("An unknown error occured!");
            return false;

        }
        
    }
}