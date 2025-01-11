using System.Collections;

namespace SR2E.Commands;

internal class FloatCommand : SR2ECommand
{
    public override string ID => "floaty";
    public override string Usage => "floaty <duration>";
    public override CommandType type => CommandType.Fun;

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return new List<string> { "10", "20", "50", "100"};
        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,1)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();
        
        Camera cam = Camera.main; if (cam == null) return SendNoCamera();
        
        float duration = 0;
        if(!this.TryParseFloat(args[0], out duration, 0, false)) return false;
        
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit))
        {
            if (hit.rigidbody == null) return SendNotLookingAtValidObject();
            MelonCoroutines.Start(TimeGravity(hit, duration));
            SendMessage(translation("cmd.float.success",duration));
        }
        return SendNotLookingAtAnything();
    }

    private IEnumerator TimeGravity(RaycastHit hit, float duration)
    {
        hit.rigidbody.useGravity = false;
        yield return new WaitForSecondsRealtime(duration);
        hit.rigidbody.useGravity = true;
    }
}