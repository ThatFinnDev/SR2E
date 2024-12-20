namespace SR2E.Commands;

public class ConsoleVisibilityCommands
{
    public class OpenCommand : SR2Command
    {
        public override string ID => "openconsole";
        public override string Usage => "openconsole";
        public override bool execWhenIsOpenConsole => true;
        public override CommandType type => CommandType.Menu;
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if(SR2EModMenu.isOpen)
                SR2EModMenu.Close();
            if(SR2ECheatMenu.isOpen)
                SR2ECheatMenu.Close();
            if(!SR2EConsole.isOpen)
                SR2EConsole.Open();
            return true;
        }
    }
    public class CloseCommand : SR2Command
    {
        public override string ID => "closeconsole";
        public override string Usage => "closeconsole";
        public override CommandType type => CommandType.Menu;
        public override bool execWhenIsOpenConsole => true;
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if(SR2EModMenu.isOpen)
                SR2EModMenu.Close();
            if(SR2ECheatMenu.isOpen)
                SR2ECheatMenu.Close();
            if(SR2EConsole.isOpen)
                SR2EConsole.Close();
            return true;
        }
    }
    public class ToggleCommand : SR2Command
    {
        public override string ID => "toggleconsole";
        public override string Usage => "toggleconsole";
        public override CommandType type => CommandType.Menu;
        public override bool execWhenIsOpenConsole => true;
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if(SR2EModMenu.isOpen)
                SR2EModMenu.Close();
            if(SR2ECheatMenu.isOpen)
                SR2ECheatMenu.Close();
            SR2EConsole.Toggle();
            return true;
        }
    }
}