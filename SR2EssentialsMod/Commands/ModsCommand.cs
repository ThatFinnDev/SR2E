namespace SR2E.Commands
{
    public class ModsCommand : SR2Command
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
            if (args != null) return SendNoArguments();

            SendMessage("<color=blue>List of Mods Loaded:</color>");

            foreach (MelonBase melonBase in MelonBase.RegisteredMelons)
            {
                SendMessage(melonBase.Info.Name+" by: "+melonBase.Info.Author);
            }

            return true;
        }
    }
}