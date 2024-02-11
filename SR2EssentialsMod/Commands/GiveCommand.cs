namespace SR2E.Commands
{
    public class GiveCommand : SR2CCommand
    {
        public override string ID => "give";
        public override string Usage => "give <item> [amount]";
        public override string Description => "Gives you items";
        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            if (argIndex == 0)
            {
                string firstArg = "";
                if (args != null) firstArg = args[0];
                List<string> list = new List<string>();
                int i = -1;
                foreach (IdentifiableType type in SR2EEntryPoint.identifiableTypes)
                {
                    if(type.ReferenceId.StartsWith("GadgetDefinition")) continue;
                    if (i > 20) break;
                    try
                    {
                        if (type.LocalizedName != null)
                        {
                            string localizedString = type.LocalizedName.GetLocalizedString();
                            if (localizedString.ToLower().Replace(" ", "").StartsWith(firstArg.ToLower()))
                            { i++; list.Add(localizedString.Replace(" ", "")); }
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
            string identifierTypeName = args[0];
            IdentifiableType type = SR2EEntryPoint.getIdentifiableByName(identifierTypeName);

            if (type == null)
            {
                type = SR2EEntryPoint.getIdentifiableByLocalizedName(identifierTypeName.Replace("_", ""));
                if (type == null)
                { SR2EConsole.SendError(args[0] + " is not a valid IdentifiableType!"); return false; }
                string name = type.LocalizedName.GetLocalizedString();
                if (name.Contains(" ")) itemName = "'" + name + "'";
                else itemName = name;
            }
            else itemName=type.name;
            
            if(type.ReferenceId.StartsWith("GadgetDefinition"))
            { SR2EConsole.SendError(args[0] + " is a gadget, not an item!"); return false; }
            
            int amount = 1;
            if (args.Length == 2)
            {
                try { amount = int.Parse(args[1]); }
                catch { SR2EConsole.SendError(args[1] + " is not a valid integer!"); return false; }

                if (amount<=0) { SR2EConsole.SendError(args[1] + " is not an integer above 0!"); return false; }
            }
            
                
            for (int i = 0; i < amount; i++)
                SceneContext.Instance.PlayerState.Ammo.MaybeAddToSlot(type,null); 
            
            
            SR2EConsole.SendMessage($"Successfully added {amount} {itemName}");
            return true;
        }
    }
}