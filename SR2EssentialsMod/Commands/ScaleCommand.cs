using Il2CppMonomiPark.SlimeRancher.World;

namespace SR2E.Commands;

internal class ScaleCommand: SR2ECommand
{
    public override string ID => "scale";
    public override string Usage => "scale <x> <y> <z>";
    public override CommandType type => CommandType.Miscellaneous;

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0,3)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();

        Vector3 scale;
        if (!this.TryParseVector3(args[0], args[1], args[2], out scale)) return false;
        
        Camera cam = Camera.main; if (cam == null) return SendNoCamera();
             
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit))
        {
            var gameobject = hit.collider.gameObject;
            if (gameobject.GetComponent<Identifiable>()) gameobject.transform.localScale = scale;
            else if (gameobject.GetComponentInParent<Gadget>())
            {
                try { gameobject.GetComponentInParent<Gadget>().transform.localScale = scale; }
                catch { }
            }
            else return SendNotLookingAtValidObject();
            SendMessage(translation("cmd.scale.success"));
            return true;
            
        }
        return SendNotLookingAtAnything();
    }
}