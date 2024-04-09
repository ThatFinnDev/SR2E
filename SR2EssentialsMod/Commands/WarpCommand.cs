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
                foreach (KeyValuePair<string,SR2ESaveManager.Warp> pair in SR2ESaveManager.data.warps) warps.Add(pair.Key); 
                return warps;
            }
            return null;
        }
        public override bool Execute(string[] args)
        {
            if (args == null || args.Length != 1) return SendUsage();
            if (!inGame) return SendLoadASaveFirst();
            string name = args[0];
            SR2ESaveManager.Warp warp = SR2ESaveManager.WarpManager.GetWarp(name);
            if (warp==null)
            { SR2EConsole.SendError($"There is no warp with the name: {name}"); return false; }

            SR2EError error = warp.WarpPlayerThere();
            switch (error)
            {
                case SR2EError.NoError: SR2EConsole.SendMessage($"Successfully warped to the warp {name}!"); return true;
                case SR2EError.NotInGame: return SendLoadASaveFirst();
                case SR2EError.PlayerNull: return SendLoadASaveFirst();
                case SR2EError.TeleportablePlayerNull: SR2EConsole.SendError($"TeleportablePlayer is null!"); return false;
                case SR2EError.SRCharacterControllerNull: SR2EConsole.SendError($"SRCharacterController is null!"); return false;
                case SR2EError.SceneGroupNotSupported: SR2EConsole.SendError($"There sceneGroup {warp.sceneGroup} is not supported!"); return false;
                case SR2EError.DoesntExist: SR2EConsole.SendError($"There place {warp.sceneGroup} does not exist!"); return false;
            }
            TeleportablePlayer p = SceneContext.Instance.Player.GetComponent<TeleportablePlayer>();
            if(p==null)
            { SR2EConsole.SendError($"TeleportablePlayer is null!"); return false; }

            SR2EConsole.SendError("An unknown error occured!");
            return false;

        }
        
    }
}