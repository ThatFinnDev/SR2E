using Il2CppMonomiPark.SlimeRancher.Damage;
<<<<<<< HEAD
using Il2CppMonomiPark.SlimeRancher.Regions;
=======
>>>>>>> experimental
using Il2CppMonomiPark.SlimeRancher.World;

namespace SR2E.Commands;

<<<<<<< HEAD
public class ReplaceCommand : SR2Command
{
    public override string ID => "replace";
    public override string Usage => "replace <object>";

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return getIdentListByPartialName(args == null ? null : args[0], true, false,true);

=======
internal class ReplaceCommand : SR2ECommand
{
    public override string ID => "replace";
    public override string Usage => "replace <object>";
    public override CommandType type => CommandType.Cheat;

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0) return getIdentListByPartialName(args == null ? null : args[0], true, true,true);
>>>>>>> experimental
        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,1)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();
<<<<<<< HEAD

        
        string identifierTypeName = args[0];
        IdentifiableType type = getIdentByName(identifierTypeName);
        if (type == null) return SendError(translation("cmd.error.notvalididenttype", identifierTypeName));

        if (type.isGadget()) return SendError(translation("cmd.give.isgadgetnotitem",type.getName()));
        
        Camera cam = Camera.main;
        if (cam == null) return SendError(translation("cmd.error.nocamera"));
        
        

        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit))
=======
        
        string identifierTypeName = args[0];
        IdentifiableType type = getIdentByName(identifierTypeName);
        if (type == null) return SendNotValidIdentType(identifierTypeName);
        if (type.isGadget()) return SendIsGadgetNotItem(type.getName());
        Camera cam = Camera.main; if (cam == null) return SendNoCamera();

        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit,Mathf.Infinity,defaultMask))
>>>>>>> experimental
        {
            var gameobject = hit.collider.gameObject;
            string oldObjectName = "";
            bool isValid = false;
            if (gameobject.GetComponent<Identifiable>())
            {
                isValid = true;
<<<<<<< HEAD
                try
                {
                    string name = gameobject.GetComponent<Identifiable>().identType.LocalizedName.GetLocalizedString();
                    if (name.Contains(" ")) oldObjectName = "'" + name + "'";
                    else oldObjectName = name;
                }
                catch
                {
                    oldObjectName = gameobject.GetComponent<Identifiable>().identType.name;
                }

                //Remove old one
                DeathHandler.Kill(gameobject, SR2EEntryPoint.killDamage);

            } /*
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
            }*/

=======
                oldObjectName = gameobject.GetComponent<Identifiable>().identType.getName();
                //Remove old one
                DeathHandler.Kill(gameobject, killDamage);

            } 
            else if (gameobject.GetComponentInParent<Gadget>())
            {
                isValid = true;
                oldObjectName = gameobject.GetComponentInParent<Gadget>().identType.getName();
                //Remove old one
                gameobject.GetComponentInParent<Gadget>().DestroyGadget();
            }
>>>>>>> experimental
            if (isValid)
            {
                //Add new one 
                GameObject spawned = null;
<<<<<<< HEAD
                //if (type is GadgetDefinition) spawned = type.prefab.SpawnGadget(hit.point,Quaternion.identity);
                //else 
                spawned = type.prefab.SpawnActor(hit.point, Quaternion.identity);
=======
                if (type is GadgetDefinition gadgetDefinition) spawned = gadgetDefinition.SpawnGadget(hit.point,Quaternion.identity);
                else spawned = type.SpawnActor(hit.point, Quaternion.identity);
>>>>>>> experimental
                Vector3 position = gameobject.transform.position;
                Quaternion rotation = gameobject.transform.rotation;
                spawned.transform.position = position;
                spawned.transform.rotation = rotation;
                SendMessage(translation("cmd.replace.success",oldObjectName,type.getName()));
                return true;
            }
<<<<<<< HEAD

        }

        return SendError(translation("cmd.error.notlookingatanything"));
=======
            return SendNotLookingAtValidObject();
        }
        return SendNotLookingAtAnything();
>>>>>>> experimental
    }
}