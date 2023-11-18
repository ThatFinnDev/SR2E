using Il2Cpp;

namespace SR2E.Commands
{
    public class GiveBlueprintCommand : SR2CCommand
    {
        public override string ID => "giveblueprint";
        public override string Usage => "giveblueprint <blueprint>";
        public override string Description => "Gives you a blueprint";

        public override bool Execute(string[] args)
        {
            if (args == null)
            { SR2Console.SendMessage($"Usage: {Usage}"); return false; }

            if (args.Length != 1)
            { SR2Console.SendMessage($"Usage: {Usage}"); return false; }


            if (SceneContext.Instance == null) { SR2Console.SendError("Load a save first!"); return false; }
            if (SceneContext.Instance.PlayerState == null) { SR2Console.SendError("Load a save first!"); return false; }


            string bluePrintName = "";
            string identifierTypeName = args[0];
            IdentifiableType type =  SR2EMain.getVaccableByName(identifierTypeName);

            if (type == null)
            {
                type =  SR2EMain.getVaccableByLocalizedName(identifierTypeName.Replace("_", ""));
                if (type == null)
                {
                    SR2Console.SendError(args[0] + " is not a valid IdentifiableType!");
                    return false;
                }

                string name = type.LocalizedName.GetLocalizedString();
                if (name.Contains(" "))
                    bluePrintName = "'" + name + "'";
                else
                    bluePrintName = name;
            }
            else
                bluePrintName = type.name;

            if (!(type is GadgetDefinition))
            { SR2Console.SendError(bluePrintName + " is not a valid Gadget!"); return false; }



            if (SceneContext.Instance.GadgetDirector.HasBlueprint(type as GadgetDefinition))
            { SR2Console.SendError("You already have this blueprint!"); return false; }

            SceneContext.Instance.GadgetDirector.AddBlueprint(type as GadgetDefinition);;
            SR2Console.SendMessage($"Successfully added {bluePrintName}");

            return true;
        }
    }
    
}