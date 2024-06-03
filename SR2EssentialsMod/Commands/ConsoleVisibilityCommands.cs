namespace SR2E.Commands;

public class ConsoleVisibilityCommands
{
    public class OpenConsoleCommand : SR2Command
    {
        public override string ID => "openconsole";
        public override string Usage => "openconsole";
        public override bool executeWhenConsoleIsOpen => true;
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if(!SR2EConsole.isOpen)
                SR2EConsole.Open();
            return true;
        }
    }
    public class CloseConsoleCommand : SR2Command
    {
        public override string ID => "closeconsole";
        public override string Usage => "closeconsole";
        public override bool executeWhenConsoleIsOpen => true;
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            if(SR2EConsole.isOpen)
                SR2EConsole.Close();
            return true;
        }
    }
    public class ToggleConsoleCommand : SR2Command
    {
        public override string ID => "toggleconsole";
        public override string Usage => "toggleconsole";
        public override bool executeWhenConsoleIsOpen => true;
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            SR2EConsole.Toggle();
            return true;
        }
    }
}