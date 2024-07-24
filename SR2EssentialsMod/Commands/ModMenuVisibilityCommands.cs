namespace SR2E.Commands;

public class ModMenuVisibilityCommands
{
    
    public class OpenCommand : SR2Command
    {
        public override string ID => "openmodmenu";
        public override string Usage => "openmodmenu";
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if (!inGame) return SendLoadASaveFirst();
            if(!SR2EModMenu.isOpen)
                SR2EModMenu.Open();
            return true;
        }
    }
    public class CloseCommand : SR2Command
    {
        public override string ID => "closemodmenu";
        public override string Usage => "closemodmenu";
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if (!inGame) return SendLoadASaveFirst();
            if(SR2EModMenu.isOpen)
                SR2EModMenu.Close();
            return true;
        }
    }
    public class ToggleCommand : SR2Command
    {
        public override string ID => "togglemodmenu";
        public override string Usage => "togglemodmenu";
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if (!inGame) return SendLoadASaveFirst();
            SR2EModMenu.Toggle();
            return true;
        }
    }
}