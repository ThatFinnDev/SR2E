namespace SR2E.Commands;

public class FlingCommand : SR2Command
{
    public override string ID => "fling";
    public override string Usage => "fling <strength>";
    public override string Description => "Flings any object you are looking at.";

    public override bool Execute(string[] args)
    {
        if (!inGame) return SendLoadASaveFirst();
        if (args == null || args.Length != 1) return SendUsage();
        
        Camera cam = Camera.main; if (cam == null) { SendError("Couldn't find player camera!"); return false; }
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit))
        {
            Transform transform = hit.transform;
            Rigidbody rb = hit.rigidbody;
            
            float strength = 0;
            try { strength = float.Parse(args[0]); }
            catch { SendError(args[0] + " is not a valid float!"); return false; }

            Vector3 cameraPosition = cam.transform.position;
            Vector3 moveDirection = transform.position - cameraPosition;
            moveDirection.Normalize();
            rb.velocity += (moveDirection * strength) + Vector3.up;
            SendMessage($"Successfully flinged the object with a string of {strength}!");
            return true;
        }
        SendError("You're not looking at a valid object!");
        return false;
    }
}

