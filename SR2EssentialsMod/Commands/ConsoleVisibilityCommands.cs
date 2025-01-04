using SR2E.Menus;

namespace SR2E.Commands;

internal class ConsoleVisibilityCommands
{
    internal class OpenCommand : SR2ECommand
    {
        public override string ID => "openconsole";
        public override string Usage => "openconsole";
        public override bool execWhenIsOpenConsole => true;
        public override CommandType type => CommandType.Menu;
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if(GM<SR2EModMenu>().isOpen)
                GM<SR2EModMenu>().Close();
            if(GM<SR2ECheatMenu>().isOpen)
                GM<SR2ECheatMenu>().Close();
            if(!GM<SR2EConsole>().isOpen)
                GM<SR2EConsole>().Open();
            return true;
        }
    }
    internal class CloseCommand : SR2ECommand
    {
        public override string ID => "closeconsole";
        public override string Usage => "closeconsole";
        public override CommandType type => CommandType.Menu;
        public override bool execWhenIsOpenConsole => true;
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if(GM<SR2EModMenu>().isOpen)
                GM<SR2EModMenu>().Close();
            if(GM<SR2ECheatMenu>().isOpen)
                GM<SR2ECheatMenu>().Close();
            if(GM<SR2EConsole>().isOpen)
                GM<SR2EConsole>().Close();
            return true;
        }
    }
    internal class ToggleCommand : SR2ECommand
    {
        public override string ID => "toggleconsole";
        public override string Usage => "toggleconsole";
        public override CommandType type => CommandType.Menu;
        public override bool execWhenIsOpenConsole => true;
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if(GM<SR2EModMenu>().isOpen)
                GM<SR2EModMenu>().Close();
            if(GM<SR2ECheatMenu>().isOpen)
                GM<SR2ECheatMenu>().Close();
            GM<SR2EConsole>().Toggle();
            return true;
        }
    }
}