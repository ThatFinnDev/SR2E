using System.Collections;

namespace SR2E.Commands;

public class FloatCommand : SR2Command
{
    public override string ID => "floaty";
    public override string Usage => "floaty <duration>";
    public override string Description => "Temporarily disables gravity for the selected object for the specified duration in seconds.";

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return new List<string> { "10", "20", "50", "100"};
        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!inGame) return SendLoadASaveFirst();
        if (args == null || args.Length != 1) return SendUsage();
        
        Camera cam = Camera.main;
        if (cam == null)
        {
            SendError("Couldn't find player camera!");
            return false;
        }
        
        float duration = 0;
        try { duration = float.Parse(args[0]); }
        catch { SendError(args[0] + " is not a valid float!"); return false; }
        if (duration<=0) { SendError(args[0] + " is not a float above 0!"); return false; }
        
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit))
        {
            if (hit.rigidbody == null)
            { SendError("You're not looking at a valid object!"); return false; }
            MelonCoroutines.Start(TimeGravity(hit, duration));
            SendMessage($"The object will float for {duration} seconds!");
        }
        else
        { SendWarning("You're not looking at a valid object!"); return false; }
        return true;
    }

    private IEnumerator TimeGravity(RaycastHit hit, float duration)
    {
        hit.rigidbody.useGravity = false;
        yield return new WaitForSecondsRealtime(duration);
        hit.rigidbody.useGravity = true;
    }
}