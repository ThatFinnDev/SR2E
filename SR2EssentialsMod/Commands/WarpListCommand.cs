namespace SR2E.Commands
{
    public class WarpListCommand: SR2CCommand
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
            if (args != null)
            {
                SR2Console.SendError($"The '<color=white>{ID}</color>' command takes no arguments");
                return false;
            }

            if (SR2Warps.warps.Count == 0)
            {
                SR2Console.SendError("There are no warps yet!");
                return false;
            }
            SR2Console.SendMessage("<color=blue>List of all Warps:</color>");
            foreach (KeyValuePair<string, Warp> pair in SR2Warps.warps)
            {
                SR2Console.SendMessage($"'{pair.Key}' in '{pair.Value.sceneGroup}' at '{pair.Value.x} {pair.Value.y} {pair.Value.z}'");
            }

            return true;
        }
    }
}