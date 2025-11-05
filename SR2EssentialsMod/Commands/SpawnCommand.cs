namespace SR2E.Commands;

internal class SpawnCommand : SR2ECommand
{
    public override string ID => "spawn";
    public override string Usage => "spawn <object> [amount]";
    public override CommandType type => CommandType.Cheat;

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0) return LookupEUtil.GetFilteredIdentifiableTypeStringListByPartialName(args == null ? null : args[0], true, MAX_AUTOCOMPLETE.Get());
        if (argIndex == 1) return new List<string> { "1", "5", "10", "20", "30", "50" };
        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,2)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();

        string identifierTypeName = args[0];
        IdentifiableType type = LookupEUtil.GetIdentifiableTypeByName(identifierTypeName);
        if (type == null) return SendNotValidIdentType(identifierTypeName);
        //if (type.isGadget()) return SendIsGadgetNotItem(type.GetName());
        Camera cam = MiscEUtil.GetActiveCamera(); if (cam == null) return SendNoCamera();
        int amount = 1;
        if (args.Length == 2) if(!TryParseInt(args[1], out amount,1, true)) return false;

        for (int i = 0; i < amount; i++)
        {
            if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit,Mathf.Infinity,MiscEUtil.defaultMask))
            {
                try
                {
                    GameObject spawned = null;
                    if (type.TryCast<GadgetDefinition>()!=null) spawned = type.TryCast<GadgetDefinition>().SpawnGadget(hit.point,Quaternion.identity).GetGameObject();
                    else spawned = type.SpawnActor(hit.point, Quaternion.identity);
                    spawned.transform.position = hit.point + hit.normal * PhysicsUtil.CalcRad(spawned.GetComponent<Collider>());
                    var delta = -(hit.point - cam.transform.position).normalized;
                    spawned.transform.rotation = Quaternion.LookRotation(delta, hit.normal);
                }catch { }
            }
        }
        SendMessage(translation("cmd.spawn.success",amount,type.GetName()));
        return true;
    }
}