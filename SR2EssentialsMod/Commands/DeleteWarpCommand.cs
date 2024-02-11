namespace SR2E.Commands
{
    public class DeleteWarpCommand : SR2CCommand
    {
        public override string ID => "delwarp";
        public override string Usage => "delwarp <name>";
        public override string Description => "Deletes a warp";
        public override string ExtendedDescription => "Deletes a warp from the <u>warp</u> command.";
        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            if (argIndex != 0) return null;
            List<string> warps = new List<string>();
            foreach (KeyValuePair<string,Warp> pair in SR2EWarps.warps) warps.Add(pair.Key); 
            return warps;
        }
        public override bool Execute(string[] args)
        {
            if (args == null || args.Length != 1) return SendUsage();
            
            string name = args[0];
            if (SR2EWarps.warps.ContainsKey(name))
            {
                SR2EWarps.warps.Remove(name);
                SR2EWarps.SaveWarps();
                SR2EConsole.SendMessage($"Successfully deleted the Warp: {name}");
                return true;
            }
            SR2EConsole.SendError($"There is no warp with the name: {name}"); 
            return false;
        }
    }
}