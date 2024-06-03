namespace SR2E.Commands;

public class FlingCommand : SR2Command
{
    public override string ID => "fling";
    public override string Usage => "fling <strength>";

    public override bool Execute(string[] args)
    {
        if (!inGame) return SendLoadASaveFirst();
        if (!args.IsBetween(1,1)) return SendUsage();
        
        Camera cam = Camera.main; if (cam == null) return SendError(translation("cmd.error.nocamera")); 
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit))
        {
            Rigidbody rb = hit.rigidbody;
            if (rb == null) return SendError(translation("cmd.error.notlookingatvalidobject")); 
            Transform transform = hit.transform;
            
            float strength = 0;
            try { strength = float.Parse(args[0]); }
            catch { return SendError(translation("cmd.error.notvalidfloat",args[0]));  }

            Vector3 cameraPosition = cam.transform.position;
            Vector3 moveDirection = transform.position - cameraPosition;
            moveDirection.Normalize();
            rb.velocity += (moveDirection * strength) + Vector3.up;
            SendMessage(translation("cmd.fling.success",strength));
            return true;
        }
        SendError(translation("cmd.error.notlookingatvalidobject"));
        return false;
    }
}

