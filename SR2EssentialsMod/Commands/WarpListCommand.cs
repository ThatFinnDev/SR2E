namespace SR2E.Commands
{
    public class WarpListCommand: SR2Command
    {
        public override string ID => "warplist";
        public override string Usage => "warplist";
        public override string Description => "Shows all saved warps and their stats";
        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            return null;
        }
        public override bool Execute(string[] args)
        {
            if (args != null) return SendNoArguments();

            if (SR2ESaveManager.data.warps.Count == 0)
            { SendError("There aren't warps yet!"); return false; }
            
            SendMessage("<color=blue>List of all Warps:</color>");
            foreach (KeyValuePair<string, SR2ESaveManager.Warp> pair in SR2ESaveManager.data.warps)
                SendMessage($"'{pair.Key}' in '{pair.Value.sceneGroup}' at '{pair.Value.x} {pair.Value.y} {pair.Value.z}'");

            return true;
        }
    }
}