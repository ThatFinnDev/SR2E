using System;
using SR2E.Menus;

namespace SR2E.Commands;

internal class ModMenuVisibilityCommands
{
    
    internal class OpenCommand : SR2ECommand
    {
        public override string ID => "openmodmenu";
        public override string Usage => "openmodmenu";
        public override Type[] execWhileMenuOpen => new[] { typeof(SR2EModMenu) };
        public override CommandType type => CommandType.Menu;
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if(GM<SR2EConsole>().isOpen)
                GM<SR2EConsole>().Close();
            if(GM<SR2ECheatMenu>().isOpen)
                GM<SR2ECheatMenu>().Close();
            if(!GM<SR2EModMenu>().isOpen)
                GM<SR2EModMenu>().Open();
            return true;
        }
    }
    internal class CloseCommand : SR2ECommand
    {
        public override string ID => "closemodmenu";
        public override string Usage => "closemodmenu";
        public override Type[] execWhileMenuOpen => new[] { typeof(SR2EModMenu) };
        public override CommandType type => CommandType.Menu;
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if(GM<SR2EConsole>().isOpen)
                GM<SR2EConsole>().Close();
            if(GM<SR2ECheatMenu>().isOpen)
                GM<SR2ECheatMenu>().Close();
            if(GM<SR2EModMenu>().isOpen)
                GM<SR2EModMenu>().Close();
            return true;
        }
    }
    internal class ToggleCommand : SR2ECommand
    {
        public override string ID => "togglemodmenu";
        public override string Usage => "togglemodmenu";
        public override Type[] execWhileMenuOpen => new[] { typeof(SR2EModMenu) };
        public override CommandType type => CommandType.Menu;
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if(GM<SR2EConsole>().isOpen)
                GM<SR2EConsole>().Close();
            if(GM<SR2ECheatMenu>().isOpen)
                GM<SR2ECheatMenu>().Close();
            GM<SR2EModMenu>().Toggle();
            return true;
        }
    }
}