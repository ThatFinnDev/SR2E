namespace SR2E.Commands;

public class MoveCommand: SR2CCommand
{
    public override string ID => "move";
    public override string Usage => "move <x> <y> <z>";
    public override string Description => "Moves a thing";

    public override bool Execute(string[] args)
    {
        if (args == null)
        { SR2Console.SendMessage($"Usage: {Usage}"); return false; }
        if (args.Length != 3)
        { SR2Console.SendMessage($"Usage: {Usage}"); return false; }

        if (SceneContext.Instance == null)
        { SR2Console.SendError("Load a save first!"); return false; }
        if (SceneContext.Instance.PlayerState == null)
        { SR2Console.SendError("Load a save first!"); return false; }

        Vector3 move;
        try
        { move = new Vector3(float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2])); }
        catch (Exception e)
        { SR2Console.SendError($"The vector {args[0]} {args[1]} {args[2]} is invalid!"); return false; }
        
        if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
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
                    gameobject.GetComponentInParent<Gadget>().Model.lastPosition += move;
                }
                catch { }
                didAThing = true;
            }
            if (didAThing)
            {
                SR2Console.SendMessage("Successfully moved the thing!");
                return true;
            }
        }
        SR2Console.SendError("Not looking at a valid object!");
        return false;
    }
}