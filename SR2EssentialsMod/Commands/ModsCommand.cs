namespace SR2E.Commands
{
    public class ModsCommand : SR2CCommand
    {
        public override string ID => "mods";
        public override string Usage => "mods";
        public override string Description => "Displays all mods loaded";
        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            return null;
        }
        public override bool Execute(string[] args)
        {
            if (args != null) return SendUsage();

            SR2EConsole.SendMessage("<color=blue>List of Mods Loaded:</color>");

            foreach (MelonBase melonBase in MelonBase.RegisteredMelons)
            {
                SR2EConsole.SendMessage(melonBase.Info.Name+" by: "+melonBase.Info.Author);
            }

            return true;
        }
    }
}