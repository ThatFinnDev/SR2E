
namespace SR2E.Commands;

internal class SpawnCommand : SR2ECommand
{

    public override string ID => "spawn";
    public override string Usage => "spawn <object> [amount]";
    public override CommandType type => CommandType.Cheat;

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return getIdentListByPartialName(args == null ? null : args[0], true, true,true);
        if (argIndex == 1)
            return new List<string> { "1", "5", "10", "20", "30", "50" };

        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,2)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();


        string identifierTypeName = args[0];
        IdentifiableType type = getIdentByName(identifierTypeName);
        if (type == null) return SendError(translation("cmd.error.notvalididenttype", identifierTypeName));
        
        if (type.isGadget()) return SendError(translation("cmd.give.isgadgetnotitem",type.getName()));
        
        Camera cam = Camera.main;
        if (cam == null) return SendError(translation("cmd.error.nocamera"));
        
        int amount = 1;
        if (args.Length == 2)
        {
            if (!int.TryParse(args[1], out amount)) return SendError(translation("cmd.error.notvalidint",args[1]));

            if (amount <= 0) return SendError(translation("cmd.error.notintabove",args[1],0));
            
        }


        for (int i = 0; i < amount; i++)
        {
            if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit))
            {
                try
                {
                    GameObject spawned = null;
                    if (type is GadgetDefinition gadgetDefinition) spawned = gadgetDefinition.SpawnGadget(hit.point,Quaternion.identity);
                    else spawned = type.SpawnActor(hit.point, Quaternion.identity);

                    spawned.transform.position =
                        hit.point + hit.normal * PhysicsUtil.CalcRad(spawned.GetComponent<Collider>());
                    var delta = -(hit.point - Camera.main.transform.position).normalized;
                    spawned.transform.rotation = Quaternion.LookRotation(delta, hit.normal);
                }
                catch
                {
                }
            }
        }


        SendMessage(translation("cmd.spawn.success",amount,type.getName()));

        return true;
    }
}