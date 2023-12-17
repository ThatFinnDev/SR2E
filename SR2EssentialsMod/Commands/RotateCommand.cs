namespace SR2E.Commands;

public class RotateCommand : SR2CCommand
{
    public override string ID => "rotate";
    public override string Usage => "rotate <x> <y> <z>";
    public override string Description => "Rotates a thing";

    public override bool Execute(string[] args)
    {
        if (args == null)
        { SR2Console.SendMessage($"Usage: {Usage}"); return false; }
        if (args.Length != 3)
        { SR2Console.SendMessage($"Usage: {Usage}"); return false; }

        if (!inGame) { SR2Console.SendError("Load a save first!"); return false; }

        Vector3 rotation;
        try
        { rotation = new Vector3(float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2])); }
        catch 
        { SR2Console.SendError($"The vector {args[0]} {args[1]} {args[2]} is invalid!"); return false; }
        
        if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
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
                gameobject.GetComponentInParent<Gadget>().transform.Rotate(new Vector3(rotation.x,0,rotation.z));
                gameobject.GetComponentInParent<Gadget>().AddRotation(rotation.y);
                didAThing = true;
            }
            if (didAThing)
            {
                SR2Console.SendMessage("Successfully rotated the thing!");
                return true;
            }
        }
        SR2Console.SendError("Not looking at a valid object!");
        return false;
    }
}