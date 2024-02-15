namespace SR2E.Commands
{
    public class GiveBlueprintCommand : SR2CCommand
    {
        public override string ID => "giveblueprint";
        public override string Usage => "giveblueprint <blueprint>";
        public override string Description => "Gives you a blueprint";
        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            if (argIndex == 0)
            {
                string firstArg = "";
                if (args != null)
                    firstArg = args[0];
                List<string> list = new List<string>();
                int i = -1;
                GadgetDefinition[] ids = Resources.FindObjectsOfTypeAll<GadgetDefinition>();
                foreach (GadgetDefinition id in ids)
                {
                    if (i > 20)
                        break;
                    try
                    {
                        if (id.LocalizedName != null)
                        {
                            string localizedString = id.LocalizedName.GetLocalizedString();
                            if (localizedString.ToLower().Replace(" ", "").StartsWith(firstArg.ToLower()))
                            {
                                i++;
                                list.Add(localizedString.Replace(" ", ""));
                            }
                        }
                    }
                    catch  { }

                }

                return list;
            }

            return null;
        }
        public override bool Execute(string[] args)
        {
            if (args == null || args.Length != 1) return SendUsage();
            if (!inGame) return SendLoadASaveFirstMessage();


            string bluePrintName = "";
            GadgetDefinition[] ids = Resources.FindObjectsOfTypeAll<GadgetDefinition>();

            GadgetDefinition foundType = null;
            
            foreach (GadgetDefinition id in ids)
                if (id.name.ToUpper() == args[0].ToUpper())
                { foundType = id; break; }

            if (foundType == null)
            {
                foreach (GadgetDefinition id in ids)
                    try
                    {
                        if (id.LocalizedName.GetLocalizedString().ToUpper().Replace(" ","") == args[0].Replace("_", "").ToUpper())
                        { foundType = id; break; }
                    } catch {}
            }
            if (foundType == null)
            { SR2EConsole.SendError(args[0] + " is not a valid IdentifiableType/Gadget!"); return false; }
            
            try { bluePrintName = foundType.LocalizedName.GetLocalizedString().Replace(" ", ""); }
            catch { bluePrintName = foundType.name; }


            if (SceneContext.Instance.GadgetDirector.HasBlueprint(foundType ))
            { SR2EConsole.SendError("You already have this blueprint!"); return false; }

            SceneContext.Instance.GadgetDirector.AddBlueprint(foundType);;
            SR2EConsole.SendMessage($"Successfully added {bluePrintName}");

            return true;
        }
    }
    
}