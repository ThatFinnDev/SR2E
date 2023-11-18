using Il2Cpp;

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
            string identifierTypeName = args[0];
            IdentifiableType type = SR2EMain.getVaccableByName(identifierTypeName);

            if (type == null)
            {
                type = SR2EMain.getVaccableByLocalizedName(identifierTypeName.Replace("_", ""));
                if (type == null)
                {
                    SR2Console.SendError(args[0] + " is not a valid IdentifiableType!");
                    return false;
                }

                string name = type.LocalizedName.GetLocalizedString();
                if (name.Contains(" "))
                    itemName = "'" + name + "'";
                else
                    itemName = name;
            }
            else
                itemName = type.name;

            if (!(type is GadgetDefinition))
            {
                SR2Console.SendError(itemName + " is not a valid Gadget!");
                return false;
            }

            int amount = 0;
            if (!int.TryParse(args[1], out amount))
            { SR2Console.SendError(args[1] + " is not a valid integer!"); return false; }

            if (amount <= 0)
            {
                SR2Console.SendError(args[1] + " is not an integer above 0!");
                return false;
            }


            SceneContext.Instance.GadgetDirector.AddItem(type, amount);
            SR2Console.SendMessage($"Successfully added {amount} {itemName}");

            return true;
        }
    }
}