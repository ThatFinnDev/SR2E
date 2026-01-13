using Il2CppMonomiPark.SlimeRancher.World;

namespace SR2E.Commands;

internal class RotationCommand : SR2ECommand
{
    public override string ID => "rotation";
    public override string Usage => "rotation <x> <y> <z> [absolute(true/false)]";
    public override CommandType type => CommandType.Miscellaneous | CommandType.Cheat;

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(3,3)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();

        Vector3 rotation;
        if (!TryParseVector3(args[0], args[1], args[2], out rotation)) return false;
        bool absolute = false;
        if (args.Length==4) if (!TryParseBool(args[3], out absolute)) return false;
        Camera cam = MiscEUtil.GetActiveCamera(); if (cam == null) return SendNoCamera();
        
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit,Mathf.Infinity,MiscEUtil.defaultMask))
        {
            var gameobject = hit.collider.gameObject;
            if (gameobject.GetComponent<Identifiable>()) gameobject.transform.Rotate(rotation);
            else if (gameobject.GetComponentInParent<Gadget>())
            {
                if (absolute)
                {
                    gameobject.GetComponentInParent<Gadget>().transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
                    gameobject.GetComponentInParent<Gadget>()._model.eulerRotation = new Vector3(rotation.x, rotation.y, rotation.z);
                }
                else
                {
                    gameobject.GetComponentInParent<Gadget>().transform.Rotate(new Vector3(rotation.x, rotation.y, rotation.z));
                    gameobject.GetComponentInParent<Gadget>()._model.eulerRotation += new Vector3(rotation.x, rotation.y, rotation.z);
                }
            }
            else return SendNotLookingAtValidObject();
            SendMessage(translation("cmd.rotate.success"));
            return true;
            
        }
        return SendNotLookingAtAnything();
        
    }
}