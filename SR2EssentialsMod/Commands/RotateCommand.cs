using Il2CppMonomiPark.SlimeRancher.World;

namespace SR2E.Commands;

public class RotateCommand : SR2Command
{
    public override string ID => "rotate";
    public override string Usage => "rotate <x> <y> <z>";
    public override CommandType type => CommandType.Miscellaneous;

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(3,3)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();

        Vector3 rotation;
        try
        { rotation = new Vector3(float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2])); }
        catch 
        { return SendError(translation("cmd.error.notvalidvector3",args[0],args[1],args[2])); }
        
        Camera cam = Camera.main;
        if (cam == null)  return SendError(translation("cmd.error.nocamera"));
             
        
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit))
        {
            var gameobject = hit.collider.gameObject;
            if (gameobject.GetComponent<Identifiable>())
            {
                gameobject.transform.Rotate(rotation);
            }
            else if (gameobject.GetComponentInParent<Gadget>())
            {
                gameobject.GetComponentInParent<Gadget>().transform.Rotate(new Vector3(rotation.x,rotation.y,rotation.z));
                gameobject.GetComponentInParent<Gadget>()._model.eulerRotation += new Vector3(rotation.x, rotation.y, rotation.z);
            }
            else
                return SendError(translation("cmd.error.notlookingatvalidobject"));
            
            SendMessage(translation("cmd.rotate.success"));
            return true;
            
        }
        return SendError(translation("cmd.error.notlookingatanything"));
        
    }
}