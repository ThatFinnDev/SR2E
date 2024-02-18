namespace SR2E.Commands
{
    public class ClearCommand : SR2CCommand
    {
        public override string ID => "clear";
        public override string Usage => "clear";
        public override string Description => "Clears the console";
        
        public override bool Execute(string[] args)
        {
            if (args != null) return SendNoArguments();

            Transform consoleContent = SR2EConsole.transform.getObjRec<Transform>("ConsoleContent");
            for (int i = 0; i < consoleContent.childCount; i++)
                Object.Destroy(consoleContent.GetChild(i).gameObject);
            
            
            SR2EConsole.SendMessage("Successfully cleared the console");
            return true;
        }
    }
}