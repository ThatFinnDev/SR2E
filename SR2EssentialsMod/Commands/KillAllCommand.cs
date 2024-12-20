namespace SR2E.Commands;

public class KillAllCommand : SR2ECommand
{
    public override string ID => "killall";
    public override string Usage => "killall [id]";
    public override CommandType type => CommandType.Cheat;

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return getIdentListByPartialName(args == null ? null : args[0], true, false,true);
        return null;
    }
    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0,01)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();
        
        if (args == null)
        {
            foreach (var ident in Resources.FindObjectsOfTypeAll<IdentifiableActor>())
            {
                if (ident.hasStarted)
                {
                    var id = ident._model.actorId;
                    if (ident.identType.name != "Player")
                    {
                        Object.Destroy(ident.gameObject);
                        SceneContext.Instance.GameModel.identifiables.Remove(id);
                    }
                }
            }
            SendMessage(translation("cmd.killall.success"));
            return true;
        }
        if (args.Length == 1)
        {
            
            string identifierTypeName = args[0];
            IdentifiableType type = getIdentByName(identifierTypeName);
            if (type == null) return SendError(translation("cmd.error.notvalididenttype", identifierTypeName));
    
            if (type.isGadget()) return SendError(translation("cmd.give.isgadgetnotitem",type.getName()));
                
            foreach (var ident in Resources.FindObjectsOfTypeAll<IdentifiableActor>())
                if (ident.hasStarted)
                    if (ident.identType == type)
                    {
                        var id = ident._model.actorId;
                        Object.Destroy(ident.gameObject);
                        SceneContext.Instance.GameModel.identifiables.Remove(id);
                    }
                
            
            SendMessage(translation("cmd.killall.successspecific",type.getName()));
            return true;
        }
        return false;
    }
}
