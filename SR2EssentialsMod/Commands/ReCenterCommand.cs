namespace SR2E.Commands;

internal class ReCenterCommand : SR2ECommand
{
    public override string ID => "recentercamera";
    public override string Usage => "recentercamera";
    public override CommandType type => CommandType.Miscellaneous;

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0, 0)) return SendNoArguments();
        Camera cam = MiscEUtil.GetActiveCamera(); if (cam == null) return SendNoCamera();
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 10000f, MiscEUtil.defaultMask))
        {
            cam.transform.LookAt(hit.transform);
            SendMessage(translation("cmd.recentercamera.success"));
            return true;
        }
        return SendNotLookingAtAnything();
    }
}
