using System.Collections.Generic;
using Il2Cpp;

namespace SR2E.Commands
{
    public class DeleteWarpCommand : SR2CCommand
    {
        public override string ID => "delwarp";
        public override string Usage => "delwarp <name>";
        public override string Description => "Deletes a warp";
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
            if (args == null)
            {
                SR2Console.SendError($"Usage: {Usage}");
                return false;
            }
            if (args.Length != 1)
            { SR2Console.SendMessage($"Usage: {Usage}"); return false; }
            
            string name = args[0];
            if (SR2Warps.warps.ContainsKey(name))
            {
                SR2Warps.warps.Remove(name);
                SR2Warps.SaveWarps();
                SR2Console.SendMessage($"Successfully deleted the Warp: {name}");
                return true;
            }
            SR2Console.SendError($"There is no warp with the name: {name}"); 
            return false;
        }
    }
}