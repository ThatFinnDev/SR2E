using Il2Cpp;

namespace SR2E.Commands
{
    public class GiveCommand : SR2CCommand
    {
        public override string ID { get; } = "give";
        public override string Usage { get; } = "give <item> <amount>";
        public override string Description { get; } = "Gives you items";
        
        public override bool Execute(string[] args)
        {
            if (args == null)
            {
                SR2Console.SendMessage($"Usage: {Usage}");
                return false;
            }

            if (args.Length != 2)
            {
                SR2Console.SendMessage($"Usage: {Usage}");
                return false;
            }

            
            if (SceneContext.Instance == null)
            { SR2Console.SendError("Load a save first!"); return false; }
                
            if (SceneContext.Instance.PlayerState == null) 
            { SR2Console.SendError("Load a save first!"); return false; }
            

            string identifierTypeName = args[0];
            IdentifiableType type = SR2EMain.getVaccableByName(identifierTypeName);
                
            if (type == null)
            { SR2Console.SendError(args[0] + " is not a valid IdentifiableType!"); return false; }

            int amount = 0;
            try
            { amount = int.Parse(args[1]); }
            catch
            { SR2Console.SendError(args[1] + " is not a valid integer!"); return false; }

            if (amount<=0)
            { SR2Console.SendError(args[1] + " is not an integer above 0!"); return false; }
                
            for (int i = 0; i < amount; i++)
            {
                SceneContext.Instance.PlayerState.Ammo.MaybeAddToSlot(type,null); 
            }
            
            SR2Console.SendMessage($"Successfully added {amount} {type.name}");
            
            return true;
        }
    }
}