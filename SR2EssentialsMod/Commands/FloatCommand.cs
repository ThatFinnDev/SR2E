using System.Collections;

namespace SR2E.Commands;

public class FloatCommand : SR2CCommand
{
    public override string ID => "floaty";
    public override string Usage => "floaty <duration>";
    public override string Description => "Temporarily disables gravity for the selected object for the specified duration.";

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return new List<string> { "10", "20", "50", "100"};
        return null;
    }

    public override bool Execute(string[] args)
    {
        if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
        {
            if (hit.rigidbody == null)
            { SR2EConsole.SendError($"Object {hit} has no rigidbody"); return false; }
            MelonCoroutines.Start(TimeGravity(hit, args[0]));
        }
        else
        { SR2EConsole.SendWarning("Not looking at a valid object!"); return false; }
        return true;
    }

    private IEnumerator TimeGravity(RaycastHit hit, string duration)
    {
        hit.rigidbody.useGravity = false;
        yield return new WaitForSecondsRealtime(float.Parse(duration));
        hit.rigidbody.useGravity = true;
    }
}