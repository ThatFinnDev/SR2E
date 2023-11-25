namespace SR2E.Commands
{
    public class ClearCommand : SR2CCommand
    {
        public override string ID => "clear";
        public override string Usage => "clear";
        public override string Description => "Clears the console";
        
        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            return null;
        }
        public override bool Execute(string[] args)
        {
            if (args != null)
            {
                SR2Console.SendError($"The '<color=white>{ID}</color>' command takes no arguments");
                return false;
            }

            Transform consoleContent = SR2Console.getObjRec<Transform>(SR2Console.transform, "ConsoleContent");
            for (int i = 0; i < consoleContent.childCount; i++)
                Object.Destroy(consoleContent.GetChild(i).gameObject);
            
            
            SR2Console.SendMessage("Successfully cleared the console");
            return true;
        }
    }
}