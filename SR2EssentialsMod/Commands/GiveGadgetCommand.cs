namespace SR2E.Commands
{
    public class GiveGadgetCommand : SR2CCommand
    {
        public override string ID => "givegadget";
        public override string Usage => "givegadget <gadget> [amount]";
        public override string Description => "Gives you gadgets";
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
            if (argIndex == 1)
                return new List<string> {"1","5","10","20","30","50"};

            return null;
        }
        public override bool Execute(string[] args)
        {
            if (args == null || args.Length > 2) return SendUsage();
            if (!inGame) return SendLoadASaveFirstMessage();


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
                    catch {}
            }
            if (foundType == null)
            { SR2EConsole.SendError(args[0] + " is not a valid IdentifiableType/Gadget!"); return false; }
            
            try
            { itemName = foundType.LocalizedName.GetLocalizedString().Replace(" ", ""); }
            catch 
            { itemName = foundType.name; }

            int amount = 1;
            if (args.Length == 2)
            {
                if (!int.TryParse(args[1], out amount))
                {
                    SR2EConsole.SendError(args[1] + " is not a valid integer!");
                    return false;
                }

                if (amount <= 0)
                {
                    SR2EConsole.SendError(args[1] + " is not an integer above 0!");
                    return false;
                }
            }

            SceneContext.Instance.GadgetDirector.AddItem(foundType, amount);
            SR2EConsole.SendMessage($"Successfully added {amount} {itemName}");

            return true;
        }
    }
}