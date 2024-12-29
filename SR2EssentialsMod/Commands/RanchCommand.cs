using System.Linq;
using Il2CppMonomiPark.SlimeRancher.Pedia;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.World;

namespace SR2E.Commands;

internal class RanchCommand : SR2ECommand
{
    public override string ID => "ranch";
    public override string Usage => "ranch <lock/unlock> <door>";
    List<string> arg0List = new List<string> { "unlock", "lock"};
    public override CommandType type => CommandType.Cheat | CommandType.Experimental;
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return arg0List;
        if (argIndex == 1)
            return translations.Values.ToList();
        return null;
    }
    public Dictionary<string, string> translations = new Dictionary<string, string>()
    {
        { "Gully","door1733849867" },
        { "Den","door0010140679"  },
        { "Archway","door0749608168" },
        { "Tidepoles","door0129604684" },
        { "Digest","door1356553442" },
        { "*","*"}
    };
    public override bool Execute(string[] args)
    {
        
        if (!args.IsBetween(2,2)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();
        if(!arg0List.Contains(args[0]))
            return SendError(translation("cmd.ranch.notvalidoption",args[0]));
        if(!translations.Values.ToList().Contains(args[1]))
            return SendError(translation("cmd.ranch.notvalidoption",args[1]));
        
        bool showPopup = args[1] != "*";
        if (args[1] == "*")
        {
            bool isSilent = silent;
            foreach (KeyValuePair<string, string> kvp in translations)
            {
                silent = true;
                Execute(new []{args[0], kvp.Key});
                silent = true;
            }
            if(silent)
            {
                silent = isSilent;
                switch (args[0])
                {
                    case "lock":
                        SendMessage(translation("cmd.ranch.successalllock"));
                        break;
                    case "unlock":
                        SendMessage(translation("cmd.ranch.successallunlock"));
                        break;
                }
            }
            return true;
        }

        string itemName = args[1];
        string door = translations[args[1]];

        var ranch = GameContext.Instance.AutoSaveDirector.SavedGame.GameState.Ranch;
        switch (args[0])
        {
            case "lock":
                if (ranch.AccessDoorStates[door] == AccessDoor.State.LOCKED)
                    return SendError(translation("cmd.ranch.errorlock",itemName));
                ranch.AccessDoorStates[door] = AccessDoor.State.LOCKED;
                SendMessage(translation("cmd.ranch.successlock",itemName)); 
                break;
            case "unlock":
                if (ranch.AccessDoorStates[door] == AccessDoor.State.OPEN)
                    return SendError(translation("cmd.pedia.errorunlock",itemName));
                ranch.AccessDoorStates[door] = AccessDoor.State.OPEN;
                SendMessage(translation("cmd.pedia.successunlock",itemName)); 
                break;
        }

        return false;
    }
}
