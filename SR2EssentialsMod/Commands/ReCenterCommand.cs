namespace SR2E.Commands;

public class ReCenterCommand : SR2Command
{
    public override string ID => "recentercamera";
    public override string Usage => "recentercamera";

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0, 0)) return SendNoArguments();
        Camera cam = Camera.main;
        if(cam!=null)
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 10000f))
            {
                cam.transform.LookAt(hit.transform);
                SendMessage(translation("cmd.recentercamera.success"));
                return true;
            }
            else
            {
                SendError(translation("cmd.recentercamera.needtolook"));
                return false;
            }
        SendError(translation("cmd.recentercamera.nocamera"));
        return false;
    }
}