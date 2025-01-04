using System;
using SR2E.Menus;

namespace SR2E.Commands;

internal class CheatMenuVisibilityCommands
{
    
    internal class OpenCommand : SR2ECommand
    {
        public override string ID => "opencheatmenu";
        public override string Usage => "opencheatmenu";
        public override Type[] execWhileMenuOpen => new[] { typeof(SR2ECheatMenu) };
        public override CommandType type => CommandType.Menu | CommandType.Cheat;
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if (!inGame) return SendLoadASaveFirst();
            if(GM<SR2EModMenu>().isOpen)
                GM<SR2EModMenu>().Close();
            if(GM<SR2EConsole>().isOpen)
                GM<SR2EConsole>().Close();
            if(!GM<SR2ECheatMenu>().isOpen)
                GM<SR2ECheatMenu>().Open();
            return true;
        }
    }
    internal class CloseCommand : SR2ECommand
    {
        public override string ID => "closecheatmenu";
        public override string Usage => "closecheatmenu";
        public override Type[] execWhileMenuOpen => new[] { typeof(SR2ECheatMenu) };
        public override CommandType type => CommandType.Menu | CommandType.Cheat;
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if (!inGame) return SendLoadASaveFirst();
            if(GM<SR2EModMenu>().isOpen)
                GM<SR2EModMenu>().Close();
            if(GM<SR2EConsole>().isOpen)
                GM<SR2EConsole>().Close();
            if(GM<SR2ECheatMenu>().isOpen)
                GM<SR2ECheatMenu>().Close();
            return true;
        }
    }
    internal class ToggleCommand : SR2ECommand
    {
        public override string ID => "togglecheatmenu";
        public override string Usage => "togglecheatmenu";
        public override Type[] execWhileMenuOpen => new[] { typeof(SR2ECheatMenu) };
        public override CommandType type => CommandType.Menu | CommandType.Cheat;

        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if (!inGame) return SendLoadASaveFirst();
            if(GM<SR2EModMenu>().isOpen)
                GM<SR2EModMenu>().Close();
            if(GM<SR2EConsole>().isOpen)
                GM<SR2EConsole>().Close();
            GM<SR2ECheatMenu>().Toggle();
            return true;
        }
    }
}