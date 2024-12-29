namespace SR2E.Commands;

internal class ModMenuVisibilityCommands
{
    
    internal class OpenCommand : SR2ECommand
    {
        public override string ID => "openmodmenu";
        public override string Usage => "openmodmenu";
        public override bool execWhenIsOpenModMenu => true;
        public override CommandType type => CommandType.Menu;
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if(SR2EConsole.isOpen)
                SR2EConsole.Close();
            if(SR2ECheatMenu.isOpen)
                SR2ECheatMenu.Close();
            if(!SR2EModMenu.isOpen)
                SR2EModMenu.Open();
            return true;
        }
    }
    internal class CloseCommand : SR2ECommand
    {
        public override string ID => "closemodmenu";
        public override string Usage => "closemodmenu";
        public override bool execWhenIsOpenModMenu => true;
        public override CommandType type => CommandType.Menu;
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if(SR2EConsole.isOpen)
                SR2EConsole.Close();
            if(SR2ECheatMenu.isOpen)
                SR2ECheatMenu.Close();
            if(SR2EModMenu.isOpen)
                SR2EModMenu.Close();
            return true;
        }
    }
    internal class ToggleCommand : SR2ECommand
    {
        public override string ID => "togglemodmenu";
        public override string Usage => "togglemodmenu";
        public override bool execWhenIsOpenModMenu => true;
        public override CommandType type => CommandType.Menu;
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if(SR2EConsole.isOpen)
                SR2EConsole.Close();
            if(SR2ECheatMenu.isOpen)
                SR2ECheatMenu.Close();
            SR2EModMenu.Toggle();
            return true;
        }
    }
}