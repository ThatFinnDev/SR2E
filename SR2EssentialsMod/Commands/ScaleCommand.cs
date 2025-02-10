using Il2CppMonomiPark.SlimeRancher.World;

namespace SR2E.Commands;

<<<<<<< HEAD
public class ScaleCommand: SR2Command
{
    public override string ID => "scale";
    public override string Usage => "scale <x> <y> <z>";
=======
internal class ScaleCommand: SR2ECommand
{
    public override string ID => "scale";
    public override string Usage => "scale <x> <y> <z>";
    public override CommandType type => CommandType.Miscellaneous;
>>>>>>> experimental

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0,3)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();

        Vector3 scale;
<<<<<<< HEAD
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
=======
        if (!this.TryParseVector3(args[0], args[1], args[2], out scale)) return false;
        
        Camera cam = Camera.main; if (cam == null) return SendNoCamera();
             
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit,Mathf.Infinity,defaultMask))
        {
            var gameobject = hit.collider.gameObject;
            if (gameobject.GetComponent<Identifiable>()) gameobject.transform.localScale = scale;
>>>>>>> experimental
            else if (gameobject.GetComponentInParent<Gadget>())
            {
                try { gameobject.GetComponentInParent<Gadget>().transform.localScale = scale; }
                catch { }
            }
<<<<<<< HEAD
            else
                return SendError(translation("cmd.error.notlookingatvalidobject"));
           
=======
            else if (hit.collider.gameObject.GetComponent<GordoEat>() != null)
            {
                return SendError(translation("cmd.scale.usegordocmd"));
            }
            else return SendNotLookingAtValidObject();
>>>>>>> experimental
            SendMessage(translation("cmd.scale.success"));
            return true;
            
        }
<<<<<<< HEAD
        return SendError(translation("cmd.error.notlookingatanything"));
=======
        return SendNotLookingAtAnything();
>>>>>>> experimental
    }
}