namespace SR2E.Commands;

public class KillAllCommand : SR2Command
{
    public override string ID => "killall";

    public override string Usage => "killall [id]";

    public override string Description => "Kills everything or only everything with a specified id.";
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
        {
            string firstArg = "";
            if (args != null)
                firstArg = args[0];
            List<string> list = new List<string>();
            int i = -1;
            foreach (IdentifiableType type in SR2EEntryPoint.identifiableTypes)
            {
                if (i > 55)
                    break;
                try
                {
                    if (type.LocalizedName != null)
                    {
                        string localizedString = type.LocalizedName.GetLocalizedString();
                        if (localizedString.ToLower().Replace(" ", "").StartsWith(firstArg.ToLower()))
                        {
                            i++;
                            list.Add(localizedString.Replace(" ", ""));
                        }
                    }
                }
                catch { }

            }

            return list;
        }

        return null;
    }
    public override bool Execute(string[] args)
    {
        if (args != null && args.Length != 1) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();
        
        if (args == null)
        {
            foreach (var ident in Resources.FindObjectsOfTypeAll<IdentifiableActor>())
            {
                if (ident.hasStarted)
                {
                    var id = ident.model.actorId;
                    if (ident.identType.name != "Player")
                    {
                        Object.Destroy(ident.gameObject);
                        SceneContext.Instance.GameModel.identifiables.Remove(id);
                    }
                }
            }
            SendMessage("Successfully killed all actors!");
            return true;
        }
        if (args.Length == 1)
        {
            IdentifiableType type = SR2EEntryPoint.getIdentifiableByLocalizedName(args[0]);
            if (type == null) type = SR2EEntryPoint.getIdentifiableByName(args[0]);
            if (type == null) { SendError(args[0]+" is not a valid ID!"); return false; }
            
            foreach (var ident in Resources.FindObjectsOfTypeAll<IdentifiableActor>())
            {
                if (ident.hasStarted)
                {
                    if (ident.identType == type)
                    {
                        var id = ident.model.actorId;
                        Object.Destroy(ident.gameObject);
                        SceneContext.Instance.GameModel.identifiables.Remove(id);
                    }
                }
            }
            SendMessage($"Successfully killed all actors with type {type.name}!");
            return true;
        }
        return false;
    }
}
