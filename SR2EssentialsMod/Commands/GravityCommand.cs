namespace SR2E.Commands;

public class GravityCommand : SR2Command
{
    public override string ID => "gravity";
    public override string Usage => "gravity <x> <y> <z>";
    public override string Description => "Sets the gravity";

    public override bool Execute(string[] args)
    {
        if (args == null || args.Length != 3) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();

        Vector3 gravBase;
        try
        {
            gravBase = new Vector3(-float.Parse(args[0]), -float.Parse(args[1]), -float.Parse(args[2]));
            Physics.gravity = gravBase * 9.81f;
            SendMessage($"Successfully changed the gravity to {args[0]} {args[1]} {args[2]}!");
            return true;
        }
        catch
        {
            SendError($"The vector {args[0]} {args[1]} {args[2]} is invalid!");
            return false;
        }

        return false;
    }
}


