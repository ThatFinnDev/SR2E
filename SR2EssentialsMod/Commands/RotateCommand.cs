using Il2CppMonomiPark.SlimeRancher.World;

namespace SR2E.Commands;

public class RotateCommand : SR2Command
{
    public override string ID => "rotate";
    public override string Usage => "rotate <x> <y> <z>";
    public override string Description => "Rotates a thing";

    public override bool Execute(string[] args)
    {
        if (args == null || args.Length !=3) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();

        Vector3 rotation;
        try
        { rotation = new Vector3(float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2])); }
        catch 
        { SendError($"The vector {args[0]} {args[1]} {args[2]} is invalid!"); return false; }
        
        Camera cam = Camera.main;
        if (cam == null)
        {
            SendError("Couldn't find player camera!");
            return false;
        }
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit))
        {
            bool didAThing = false;
            var gameobject = hit.collider.gameObject;
            if (gameobject.GetComponent<Identifiable>())
            {
                gameobject.transform.Rotate(rotation);
                didAThing = true;
            }
            else if (gameobject.GetComponentInParent<Gadget>())
            {
                gameobject.GetComponentInParent<Gadget>().transform.Rotate(new Vector3(rotation.x,rotation.y,rotation.z));
                gameobject.GetComponentInParent<Gadget>()._model.eulerRotation += new Vector3(rotation.x, rotation.y, rotation.z);
                didAThing = true;
            }
            if (didAThing)
            {
                SendMessage("Successfully rotated the thing!");
                return true;
            }
        }
        SendError("Not looking at a valid object!");
        return false;
    }
}