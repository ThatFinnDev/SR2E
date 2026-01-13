using Il2CppMonomiPark.SlimeRancher.World;

namespace SR2E.Commands;

internal class PositionCommand: SR2ECommand
{
    public override string ID => "position";
    public override string Usage => "position <x> <y> <z>";
    public override CommandType type => CommandType.Miscellaneous | CommandType.Cheat;

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0,3)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();

        Vector3 move;
        if (!TryParseVector3(args[0], args[1], args[2], out move)) return false;
        bool absolute = false;
        if (args.Length==4) if (!TryParseBool(args[3], out absolute)) return false;
        
        Camera cam = MiscEUtil.GetActiveCamera(); if (cam == null) return SendNoCamera();
             
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit,Mathf.Infinity,MiscEUtil.defaultMask))
        {
            var gameobject = hit.collider.gameObject;
            if (gameobject.GetComponent<Identifiable>()) 
                if(absolute) gameobject.transform.position = move;
                else gameobject.transform.position += move;
            else if (gameobject.GetComponentInParent<Gadget>())
            {
                try
                {
                    if (absolute)
                    {
                        gameobject.GetComponentInParent<Gadget>().transform.position = move;
                        gameobject.GetComponentInParent<Gadget>()._model.lastPosition = move;
                    }
                    else
                    {
                        gameobject.GetComponentInParent<Gadget>().transform.position += move;
                        gameobject.GetComponentInParent<Gadget>()._model.lastPosition += move;
                    }
                } catch { }
            }
            else return SendNotLookingAtValidObject();
           
            SendMessage(translation("cmd.move.success"));
            return true;
            
        }

        return SendNotLookingAtAnything();
    }
}