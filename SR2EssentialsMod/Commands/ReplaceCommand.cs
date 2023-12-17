using Il2CppMonomiPark.SlimeRancher.Damage;
using Il2CppMonomiPark.SlimeRancher.Regions;

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
                    catch  { }

                }

                return list;
            }

            return null;
        }
        public override bool Execute(string[] args)
        {
            if (!inGame) { SR2Console.SendError("Load a save first!"); return false; }

            string objectName = "";
            string identifierTypeName = args[0];
            IdentifiableType type = SR2EEntryPoint.getIdentifiableByName(identifierTypeName);

            if (type == null)
            {
                type = SR2EEntryPoint.getIdentifiableByLocalizedName(identifierTypeName.Replace("_", ""));
                if (type == null)
                { SR2Console.SendError(args[0] + " is not a valid IdentifiableType!"); return false; }
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
                if (gameobject.GetComponent<Identifiable>())
                {
                    string oldObjectName = "";
                    try
                    {
                        string name = gameobject.GetComponent<Identifiable>().identType.LocalizedName
                            .GetLocalizedString();
                        if (name.Contains(" "))
                            objectName = "'" + name + "'";
                        else
                            objectName = name;
                    }
                    catch 
                    { oldObjectName = gameobject.GetComponent<Identifiable>().identType.name;}
                    
                    Vector3 position = gameobject.transform.position;
                    Quaternion rotation = gameobject.transform.rotation;
                    //Remove old one
                    Damage damage = new Damage { DamageSource = ScriptableObject.CreateInstance<DamageSourceDefinition>() };;
                    damage.DamageSource.hideFlags |= HideFlags.HideAndDontSave;
                    damage.Amount = 99999999;
                    DeathHandler.Kill(gameobject, damage);
                    
                    //Add new one 
                    var spawned = SRBehaviour.InstantiateActor(type.prefab, SceneContext.Instance.Player.GetComponent<RegionMember>().SceneGroup, hit.point,Quaternion.identity,true, SlimeAppearance.AppearanceSaveSet.NONE,SlimeAppearance.AppearanceSaveSet.NONE);
                    spawned.transform.position = position;
                    spawned.transform.rotation = rotation;

                    SR2Console.SendMessage($"Successfully replaced {oldObjectName} with {objectName}!");
                    return true;
                }
            }
            SR2Console.SendError("Not looking at a valid object!");
            return false;
        }
    }
}