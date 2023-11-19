using Il2Cpp;
using UnityEngine;

namespace SR2E.Commands
{
    public class GiveGadgetCommand : SR2CCommand
    {
        public override string ID => "givegadget";
        public override string Usage => "givegadget <gadget> <amount>";
        public override string Description => "Gives you gadgets";

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
            
            
            if (SceneContext.Instance == null) { SR2Console.SendError("Load a save first!"); return false; }
            if (SceneContext.Instance.PlayerState == null) { SR2Console.SendError("Load a save first!"); return false; }


            string itemName = "";
            GadgetDefinition[] ids = Resources.FindObjectsOfTypeAll<GadgetDefinition>();

            GadgetDefinition foundType = null;
            
            foreach (GadgetDefinition id in ids)
                if (id.name.ToUpper() == args[0].ToUpper())
                {
                    foundType = id;
                    break;
                }

            if (foundType == null)
            {
                foreach (GadgetDefinition id in ids)
                    try
                    {
                        if (id.LocalizedName.GetLocalizedString().ToUpper().Replace(" ","") == args[0].Replace("_", "").ToUpper())
                        {
                            foundType = id;
                            break;
                        }
                    }
                    catch (System.Exception ignored)
                    {}
            }
            if (foundType == null)
            { SR2Console.SendError(args[0] + " is not a valid IdentifiableType/Gadget!"); return false; }
            
            try
            { itemName = foundType.LocalizedName.GetLocalizedString().Replace(" ", ""); }
            catch (System.Exception ignored)
            { itemName = foundType.name; }


            int amount = 0;
            if (!int.TryParse(args[1], out amount))
            { SR2Console.SendError(args[1] + " is not a valid integer!"); return false; }

            if (amount <= 0)
            { SR2Console.SendError(args[1] + " is not an integer above 0!"); return false; }


            SceneContext.Instance.GadgetDirector.AddItem(foundType, amount);
            SR2Console.SendMessage($"Successfully added {amount} {itemName}");

            return true;
        }
    }
}