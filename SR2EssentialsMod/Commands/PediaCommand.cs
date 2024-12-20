using System.Linq;
using Il2CppMonomiPark.SlimeRancher.Pedia;
using Il2CppMonomiPark.SlimeRancher.UI;

namespace SR2E.Commands;

public class PediaCommand : SR2ECommand
{
    public override string ID => "pedia";
    public override string Usage => "pedia <lock/unlock> <gadget> [show popup(true/false)]";
    List<string> arg0List = new List<string> { "unlock", "lock"};
    public override CommandType type => CommandType.Cheat;
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return arg0List;
        if (argIndex == 1)
        {
            var identifiableTypeGroup =
                Resources.FindObjectsOfTypeAll<PediaEntry>().Select(x => x.name);
            List<string> list = identifiableTypeGroup.ToList();
            list.Add("*");
            return list;
        }
        if (argIndex == 2)
            return new List<string> { "true","false" };
        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(2,3)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();
        if(!arg0List.Contains(args[0]))
            return SendError(translation("cmd.pedia.notvalidoption",args[0]));
        
        bool showPopup = args[1] != "*";
        if (args.Length == 3)
        {
            string boolToParse = args[2].ToLower();
            if (boolToParse != "true" && boolToParse != "false") return SendError(translation("cmd.error.notvalidbool",args[2]));
            showPopup = boolToParse == "true";
        }
        if (args[1] == "*")
        {
            bool isSilent = silent;
            foreach (PediaEntry def in Resources.FindObjectsOfTypeAll<PediaEntry>())
            {
                silent = true;
                if(args.Length==3) Execute(new []{args[0], def.name, args[2]});
                else Execute(new []{args[0], def.name});
                silent = true;
            }
            if(silent)
            {
                silent = isSilent;
                switch (args[0])
                {
                    case "lock":
                        SendMessage(translation("cmd.pedia.successalllock"));
                        break;
                    case "unlock":
                        SendMessage(translation("cmd.pedia.successallunlock"));
                        break;
                }
            }
            return true;
        }
        string identifierTypeName = args[1];
        PediaEntry id = Resources.FindObjectsOfTypeAll<PediaEntry>().FirstOrDefault(x => x.name.ToLower().Equals(identifierTypeName.ToLower()));
        if(id==null) return SendError(translation("cmd.error.notvalidpedia",identifierTypeName));
        string itemName = id.name;

        switch (args[0])
        {
            case "lock":
                if (!SceneContext.Instance.PediaDirector.IsUnlocked(id))
                    return SendError(translation("cmd.pedia.errorlock",itemName));
                SceneContext.Instance.PediaDirector.DebugReLock(id);
                SendMessage(translation("cmd.pedia.successlock",itemName)); 
                break;
            case "unlock":
                if (SceneContext.Instance.PediaDirector.IsUnlocked(id))
                    return SendError(translation("cmd.pedia.errorunlock",itemName));
                SceneContext.Instance.PediaDirector.Unlock(id,showPopup);
                SendMessage(translation("cmd.pedia.successunlock",itemName)); 
                break;
        }

        return false;
    }
}
