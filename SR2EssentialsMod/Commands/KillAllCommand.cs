namespace SR2E.Commands;

<<<<<<< HEAD
public class KillAllCommand : SR2Command
{
    public override string ID => "killall";
    public override string Usage => "killall [id]";
=======
internal class KillAllCommand : SR2ECommand
{
    public override string ID => "killall";
    public override string Usage => "killall [id]";
    public override CommandType type => CommandType.Cheat;
>>>>>>> experimental

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return getIdentListByPartialName(args == null ? null : args[0], true, false,true);
        return null;
    }
    public override bool Execute(string[] args)
    {
<<<<<<< HEAD
        if (!args.IsBetween(0,01)) return SendUsage();
=======
        if (!args.IsBetween(0,1)) return SendUsage();
>>>>>>> experimental
        if (!inGame) return SendLoadASaveFirst();
        
        if (args == null)
        {
            foreach (var ident in Resources.FindObjectsOfTypeAll<IdentifiableActor>())
<<<<<<< HEAD
            {
=======
>>>>>>> experimental
                if (ident.hasStarted)
                {
                    var id = ident._model.actorId;
                    if (ident.identType.name != "Player")
                    {
                        Object.Destroy(ident.gameObject);
                        SceneContext.Instance.GameModel.identifiables.Remove(id);
                    }
                }
<<<<<<< HEAD
            }
=======
>>>>>>> experimental
            SendMessage(translation("cmd.killall.success"));
            return true;
        }
        if (args.Length == 1)
        {
            
            string identifierTypeName = args[0];
            IdentifiableType type = getIdentByName(identifierTypeName);
<<<<<<< HEAD
            if (type == null) return SendError(translation("cmd.error.notvalididenttype", identifierTypeName));
    
            if (type.isGadget()) return SendError(translation("cmd.give.isgadgetnotitem",type.getName()));
=======
            if (type == null) return SendNotValidIdentType(identifierTypeName);
            if (type.isGadget()) return SendIsGadgetNotItem(type.getName());
>>>>>>> experimental
                
            foreach (var ident in Resources.FindObjectsOfTypeAll<IdentifiableActor>())
                if (ident.hasStarted)
                    if (ident.identType == type)
                    {
                        var id = ident._model.actorId;
                        Object.Destroy(ident.gameObject);
                        SceneContext.Instance.GameModel.identifiables.Remove(id);
                    }
<<<<<<< HEAD
                
            
=======
>>>>>>> experimental
            SendMessage(translation("cmd.killall.successspecific",type.getName()));
            return true;
        }
        return false;
    }
}
