using Il2CppMonomiPark.SlimeRancher.World;

namespace SR2E.Commands;

public class ScaleCommand: SR2Command
{
    public override string ID => "scale";
    public override string Usage => "scale <x> <y> <z>";

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0,3)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();

        Vector3 scale;
        try
        { scale = new Vector3(float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2])); }
        catch 
        {return SendError(translation("cmd.error.notvalidvector3",args[0],args[1],args[2])); }
        
        Camera cam = Camera.main;
        if (cam == null) return SendError(translation("cmd.error.nocamera"));
             
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit))
        {
            var gameobject = hit.collider.gameObject;
            if (gameobject.GetComponent<Identifiable>())
            {
                gameobject.transform.localScale = scale;
            }
            else if (gameobject.GetComponentInParent<Gadget>())
            {
                try { gameobject.GetComponentInParent<Gadget>().transform.localScale = scale; }
                catch { }
            }
            else
                return SendError(translation("cmd.error.notlookingatvalidobject"));
           
            SendMessage(translation("cmd.scale.success"));
            return true;
            
        }
        return SendError(translation("cmd.error.notlookingatanything"));
    }
}