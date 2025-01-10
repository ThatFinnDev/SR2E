using Il2CppMonomiPark.SlimeRancher.Damage;
using Il2CppMonomiPark.SlimeRancher.Regions;
using Il2CppMonomiPark.SlimeRancher.World;

namespace SR2E.Commands;

internal class ReplaceCommand : SR2ECommand
{
    public override string ID => "replace";
    public override string Usage => "replace <object>";
    public override CommandType type => CommandType.Cheat;

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return getIdentListByPartialName(args == null ? null : args[0], true, true,true);

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
            if (isValid)
            {
                //Add new one 
                GameObject spawned = null;
                if (type is GadgetDefinition gadgetDefinition) spawned = gadgetDefinition.SpawnGadget(hit.point,Quaternion.identity);
                else spawned = type.SpawnActor(hit.point, Quaternion.identity);
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