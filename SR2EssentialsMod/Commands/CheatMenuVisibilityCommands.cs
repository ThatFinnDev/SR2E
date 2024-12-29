namespace SR2E.Commands;

internal class CheatMenuVisibilityCommands
{
    
    internal class OpenCommand : SR2ECommand
    {
        public override string ID => "opencheatmenu";
        public override string Usage => "opencheatmenu";
        public override bool execWhenIsOpenCheatMenu => true;
        public override CommandType type => CommandType.Menu | CommandType.Cheat;
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if (!inGame) return SendLoadASaveFirst();
            if(SR2EModMenu.isOpen)
                SR2EModMenu.Close();
            if(SR2EConsole.isOpen)
                SR2EConsole.Close();
            if(!SR2ECheatMenu.isOpen)
                SR2ECheatMenu.Open();
            return true;
        }
    }
    internal class CloseCommand : SR2ECommand
    {
        public override string ID => "closecheatmenu";
        public override string Usage => "closecheatmenu";
        public override bool execWhenIsOpenCheatMenu => true;
        public override CommandType type => CommandType.Menu | CommandType.Cheat;
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if (!inGame) return SendLoadASaveFirst();
            if(SR2EModMenu.isOpen)
                SR2EModMenu.Close();
            if(SR2EConsole.isOpen)
                SR2EConsole.Close();
            if(SR2ECheatMenu.isOpen)
                SR2ECheatMenu.Close();
            return true;
        }
    }
    internal class ToggleCommand : SR2ECommand
    {
        public override string ID => "togglecheatmenu";
        public override string Usage => "togglecheatmenu";
        public override bool execWhenIsOpenCheatMenu => true;
        public override CommandType type => CommandType.Menu | CommandType.Cheat;

        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if (!inGame) return SendLoadASaveFirst();
            if(SR2EModMenu.isOpen)
                SR2EModMenu.Close();
            if(SR2EConsole.isOpen)
                SR2EConsole.Close();
            SR2ECheatMenu.Toggle();
            return true;
        }
    }
}