using Il2CppMonomiPark.SlimeRancher.Damage;
using Il2CppMonomiPark.SlimeRancher.Regions;
using Il2CppMonomiPark.SlimeRancher.World;

namespace SR2E.Commands
{
    public class ReplaceCommand : SR2CCommand
    {
        public override string ID => "replace";
        public override string Usage => "replace <object>";
        public override string Description => "Replaces the thing you're looking at";
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
                    catch  { }

                }

                return list;
            }

            return null;
        }
        public override bool Execute(string[] args)
        {
            if (!inGame) { SR2EConsole.SendError("Load a save first!"); return false; }

            string objectName = "";
            string identifierTypeName = args[0];
            IdentifiableType type = SR2EEntryPoint.getIdentifiableByName(identifierTypeName);

            if (type == null)
            {
                type = SR2EEntryPoint.getIdentifiableByLocalizedName(identifierTypeName.Replace("_", ""));
                if (type == null)
                { SR2EConsole.SendError(args[0] + " is not a valid IdentifiableType!"); return false; }
                string name = type.LocalizedName.GetLocalizedString();
                if (name.Contains(" "))
                    objectName = "'" + name + "'";
                else
                    objectName = name;
            }
            else
                objectName=type.name;
            
            
            if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
            {
                var gameobject = hit.collider.gameObject;
                string oldObjectName = "";
                bool isValid = false;
                if (gameobject.GetComponent<Identifiable>())
                {
                    isValid = true;
                    try
                    {
                        string name = gameobject.GetComponent<Identifiable>().identType.LocalizedName.GetLocalizedString();
                        if (name.Contains(" ")) objectName = "'" + name + "'";
                        else objectName = name;
                    }
                    catch 
                    { oldObjectName = gameobject.GetComponent<Identifiable>().identType.name;}
                    
                    //Remove old one
                    DeathHandler.Kill(gameobject, SR2EEntryPoint.killDamage);
                    
                }
                else if (gameobject.GetComponentInParent<Gadget>())
                {
                    isValid = true;
                    try
                    {
                        string name = gameobject.GetComponentInParent<Gadget>().identType.LocalizedName.GetLocalizedString();
                        if (name.Contains(" ")) objectName = "'" + name + "'";
                        else objectName = name;
                    }
                    catch 
                    { oldObjectName = gameobject.GetComponentInParent<Gadget>().identType.name;}
                    
                    //Remove old one
                    gameobject.GetComponentInParent<Gadget>().DestroyGadget();
                }
                if (isValid)
                {
                    //Add new one 
                    GameObject spawned = null;
                    if (type is GadgetDefinition) spawned = type.prefab.SpawnGadget(hit.point,Quaternion.identity);
                    else spawned = type.prefab.SpawnActor(hit.point, Quaternion.identity);
                     Vector3 position = gameobject.transform.position;
                    Quaternion rotation = gameobject.transform.rotation;
                    spawned.transform.position = position;
                    spawned.transform.rotation = rotation;
                    SR2EConsole.SendMessage($"Successfully replaced {oldObjectName} with {objectName}!");
                    return true;
                }
                
            }
            SR2EConsole.SendError("Not looking at a valid object!");
            return false;
        }
    }
}