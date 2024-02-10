using Il2CppMonomiPark.SlimeRancher.Regions;
using Il2CppMonomiPark.SlimeRancher.World;

namespace SR2E.Commands
{
    public class SpawnCommand: SR2CCommand
    {
    
        public override string ID => "spawn";
        public override string Usage => "spawn <object> [amount]";
        public override string Description => "Spawns something in front of your face";
        
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
                    if (i > 55)
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
            if (args == null)
            {
                SR2Console.SendMessage($"Usage: {Usage}");
                return false;
            }

            if (args.Length != 2&&args.Length != 1)
            {
                SR2Console.SendMessage($"Usage: {Usage}");
                return false;
            }

            
            if (!inGame) { SR2Console.SendError("Load a save first!"); return false; }


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
            int amount = 1;
            if (args.Length == 2)
            {
                if (!int.TryParse(args[1], out amount))
                {
                    SR2Console.SendError(args[1] + " is not a valid integer!");
                    return false;
                }

                if (amount <= 0)
                {
                    SR2Console.SendError(args[1] + " is not an integer above 0!");
                    return false;
                }
            }

            //if(type.ReferenceId.StartsWith("GadgetDefinition"))
            //{ SR2Console.SendError(args[0] + " is a gadget, not an item!"); return false; }
            
            for (int i = 0; i < amount; i++)
            {
                if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
                {
                    try
                    {
                        GameObject spawned = null;
                        if (type is GadgetDefinition)
                            spawned = type.prefab.SpawnGadget(hit.point,Quaternion.identity);
                        else
                            spawned = type.prefab.SpawnActor(hit.point, Quaternion.identity);
                        spawned.transform.position = hit.point+hit.normal*PhysicsUtil.CalcRad(spawned.GetComponent<Collider>());
                        var delta = -(hit.point - Camera.main.transform.position).normalized;
                        spawned.transform.rotation = Quaternion.LookRotation(delta, hit.normal);
                        if (type is GadgetDefinition) 
                            SceneContext.Instance.ActorRegistry.Register(spawned.GetComponent<Gadget>());
                    }
                    catch 
                    { }
                }
            }
            
            
            SR2Console.SendMessage($"Successfully spawned {amount} {itemName}");
            
            return true;
        }
    }

    
}