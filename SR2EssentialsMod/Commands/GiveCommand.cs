﻿using Il2Cpp;

namespace SR2E.Commands
{
    public class GiveCommand : SR2CCommand
    {
        public override string ID => "give";
        public override string Usage => "give <item> <amount>";
        public override string Description => "Gives you items";
        
        public override bool Execute(string[] args)
        {
            if (args == null)
            { SR2Console.SendMessage($"Usage: {Usage}"); return false; }

            if (args.Length != 2)
            { SR2Console.SendMessage($"Usage: {Usage}"); return false; }

            
            if (SceneContext.Instance == null) { SR2Console.SendError("Load a save first!"); return false; }
            if (SceneContext.Instance.PlayerState == null) { SR2Console.SendError("Load a save first!"); return false; }


            string itemName = "";
            string identifierTypeName = args[0];
            IdentifiableType type = SR2EMain.getVaccableByName(identifierTypeName);

            if (type == null)
            {
                type = SR2EMain.getVaccableByLocalizedName(identifierTypeName.Replace("_", ""));
                if (type == null)
                { SR2Console.SendError(args[0] + " is not a valid IdentifiableType!"); return false; }
                string name = type.LocalizedName.GetLocalizedString();
                if (name.Contains(" "))
                    itemName = "'" + name + "'";
                else
                    itemName = name;
            }
            else
                itemName=type.name;
            int amount = 0;
            try
            { amount = int.Parse(args[1]); }
            catch
            { SR2Console.SendError(args[1] + " is not a valid integer!"); return false; }

            if (amount<=0)
            { SR2Console.SendError(args[1] + " is not an integer above 0!"); return false; }
                
            for (int i = 0; i < amount; i++)
                SceneContext.Instance.PlayerState.Ammo.MaybeAddToSlot(type,null); 
            
            
            SR2Console.SendMessage($"Successfully added {amount} {itemName}");
            
            return true;
        }
    }
}