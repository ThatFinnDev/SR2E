namespace SR2E.Commands;

public class GravityCommand : SR2Command
{
    public override string ID => "gravity";
    public override string Usage => "gravity <x> <y> <z>";

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(3,3)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();

        Vector3 gravBase;
        try
        {
            gravBase = new Vector3(-float.Parse(args[0]), -float.Parse(args[1]), -float.Parse(args[2]));
            Physics.gravity = gravBase * 9.81f;
            SendMessage(translation("cmd.gravity.success",args[0],args[1],args[2]));
            return true;
        }
        catch { return SendError(translation("cmd.error.notvalidvector3",args[0],args[1],args[2])); }
    }
}


