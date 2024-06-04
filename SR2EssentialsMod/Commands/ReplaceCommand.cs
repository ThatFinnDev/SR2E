using Il2CppMonomiPark.SlimeRancher.Damage;
using Il2CppMonomiPark.SlimeRancher.Regions;
using Il2CppMonomiPark.SlimeRancher.World;

namespace SR2E.Commands;

public class ReplaceCommand : SR2Command
{
    public override string ID => "replace";
    public override string Usage => "replace <object>";

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return getIdentListByPartialName(args == null ? null : args[0], true, false);

        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,1)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();

        
        string identifierTypeName = args[0];
        IdentifiableType type = getIdentByName(identifierTypeName);
        if (type == null) return SendError(translation("cmd.error.notvalididenttype", identifierTypeName));

        if (type.isGadget()) return SendError(translation("cmd.give.isgadgetnotitem",type.getName()));
        
        Camera cam = Camera.main;
        if (cam == null) return SendError(translation("cmd.error.nocamera"));
        
        

        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit))
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

            if (isValid)
            {
                //Add new one 
                GameObject spawned = null;
                //if (type is GadgetDefinition) spawned = type.prefab.SpawnGadget(hit.point,Quaternion.identity);
                //else 
                spawned = type.prefab.SpawnActor(hit.point, Quaternion.identity);
                Vector3 position = gameobject.transform.position;
                Quaternion rotation = gameobject.transform.rotation;
                spawned.transform.position = position;
                spawned.transform.rotation = rotation;
                SendMessage(translation("cmd.replace.success",oldObjectName,type.getName()));
                return true;
            }

        }

        return SendError(translation("cmd.error.notlookingatanything"));
    }
}