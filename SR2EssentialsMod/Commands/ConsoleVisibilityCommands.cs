namespace SR2E.Commands;

public class ConsoleVisibilityCommands
{
    public class OpenConsoleCommand : SR2CCommand
    {
        public override string ID => "openconsole";
        public override string Usage => "openconsole";
        public override string Description => "Opens the console";
        public override bool executeWhenConsoleIsOpen => true;
        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            return null;
        }
        public override bool Execute(string[] args)
        {
            if (args != null)
            { SR2Console.SendError($"The '<color=white>{ID}</color>' command takes no arguments"); return false; }
            return SilentExecute(args);
        }
        public override bool SilentExecute(string[] args)
        {
            if(!SR2Console.isOpen)
                SR2Console.Open();
            return true;
        }
    }
    public class CloseConsoleCommand : SR2CCommand
    {
        public override string ID => "closeconsole";
        public override string Usage => "closeconsole";
        public override string Description => "Closes the console";
        public override bool executeWhenConsoleIsOpen => true;
        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            return null;
        }
        public override bool Execute(string[] args)
        {
            if (args != null)
            { SR2Console.SendError($"The '<color=white>{ID}</color>' command takes no arguments"); return false; }
            return SilentExecute(args);
        }
        public override bool SilentExecute(string[] args)
        {
            if(SR2Console.isOpen)
                SR2Console.Close();
            return true;
        }
    }
    public class ToggleConsoleCommand : SR2CCommand
    {
        public override string ID => "toggleconsole";
        public override string Usage => "toggleconsole";
        public override string Description => "Toggles the console";
        public override bool executeWhenConsoleIsOpen => true;
        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            return null;
        }
        public override bool Execute(string[] args)
        {
            if (args != null)
            { SR2Console.SendError($"The '<color=white>{ID}</color>' command takes no arguments"); return false; }
            return SilentExecute(args);
        }
        public override bool SilentExecute(string[] args)
        {
            SR2Console.Toggle();
            return true;
        }
    }
}