namespace SR2E.Commands
{
    public class GiveCommand : SR2CCommand
    {
        public override string ID => "give";
        public override string Usage => "give <item> <amount>";
        public override string Description => "Gives you items";
        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            if (argIndex == 0)
            {
                string firstArg = "";
                if (args != null)
                    firstArg = args[0];
                List<string> list = new List<string>();
                int i = -1;
                foreach (IdentifiableType type in SR2EEntryPoint.identifiableTypes)
                {
                    if(type.ReferenceId.StartsWith("GadgetDefinition"))
                        continue;
                    if (i > 20)
                        break;
                    try
                    {
                        if (type.LocalizedName != null)
                        {
                            string localizedString = type.LocalizedName.GetLocalizedString();
                            if (localizedString.ToLower().Replace(" ", "").StartsWith(firstArg.ToLower()))
                            {
                                i++;
                                list.Add(localizedString.Replace(" ", ""));
                            }
                        }
                    }
                    catch (Exception ignored) { }

                }

                return list;
            }
            if (argIndex == 1)
            {
                
                List<string> list = new List<string>();
                list.Add("1");
                list.Add("5");
                list.Add("10");
                list.Add("20");
                list.Add("30");
                list.Add("50");
                return list;
            }

            return null;
        }
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
            IdentifiableType type = SR2EEntryPoint.getIdentifiableByName(identifierTypeName);

            if (type == null)
            {
                type = SR2EEntryPoint.getIdentifiableByLocalizedName(identifierTypeName.Replace("_", ""));
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
            
            if(type.ReferenceId.StartsWith("GadgetDefinition"))
            { SR2Console.SendError(args[0] + " is a gadget, not an item!"); return false; }
            
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