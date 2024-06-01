using Il2CppMonomiPark.SlimeRancher.World;

namespace SR2E.Commands;

public class MoveCommand: SR2Command
{
    public override string ID => "move";
    public override string Usage => "move <x> <y> <z>";
    public override string Description => "Moves a thing";

    public override bool Execute(string[] args)
    {
        if (args == null || args.Length != 3) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();

        Vector3 move;
        try
        { move = new Vector3(float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2])); }
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
                gameobject.transform.position += move;
                didAThing = true;
            }
            else if (gameobject.GetComponentInParent<Gadget>())
            {
                try
                {
                    gameobject.GetComponentInParent<Gadget>().transform.position += move;
                    gameobject.GetComponentInParent<Gadget>()._model.lastPosition += move;
                }
                catch { }
                didAThing = true;
            }
            if (didAThing)
            {
                SendMessage("Successfully moved the thing!");
                return true;
            }
        }
        SendError("Not looking at a valid object!");
        return false;
    }
}