using System;
using Il2CppMonomiPark.SlimeRancher.Persist;
using Il2CppMonomiPark.World;

namespace SR2E.Commands;

internal class RanchCommand : SR2ECommand
{
    internal static RanchV02 ranch = null;
    public override string ID => "ranch";
    public override string Usage => "ranch <lock/unlock> <door>";
    List<string> arg0List = new List<string> { "unlock", "lock"};
    public override CommandType type => CommandType.Cheat;
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        //return new List<string>(){"InMaintenance"};
        if (argIndex == 0) return arg0List;
        if (argIndex == 1) return doors;
        return null;
    }

    public override void OnMainMenuUIInitialize()
    {
        doors = new List<string>();
    }

    internal static void LoadAutoComplete(RanchV02 ranchV02)
    { 
        ranch = ranchV02;
        doors = new List<string>();
        foreach (var door in ranch.AccessDoorStates) doors.Add(door.Key);
        doors.Add("*");
    }
    public static List<string> doors = new List<string>();
    public override bool Execute(string[] args)
    {
        //return SendCommandMaintenance();
        if (!args.IsBetween(2,2)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();
        if (!arg0List.Contains(args[0])) return SendNotValidOption(args[0]);
        if(!doors.Contains(args[1])) return SendNotValidOption(args[1]);
        if (args[1] == "*")
        {
            bool isSilent = silent;
            foreach (string door in doors)
            {
                if(door=="*") continue;
                silent = true;
                Execute(new []{args[0], door});
                silent = true;
            }
            silent = isSilent;
            switch (args[0])
            { 
                case "lock": SendMessage(translation("cmd.ranch.successalllock")); break;
                case "unlock": SendMessage(translation("cmd.ranch.successallunlock")); break;
            }
            return true;
        }

        var accessDoors = GetAll<AccessDoor>();
        accessDoors.RemoveAll(item => item == null);
        accessDoors.RemoveAll(item => !doors.Contains(item._id));
        switch (args[0])
        {
            case "lock":
                if (ranch.AccessDoorStates[args[1]] == AccessDoor.State.LOCKED)
                    return SendError(translation("cmd.ranch.errorlock",args[1]));
                ranch.AccessDoorStates[args[1]] = AccessDoor.State.LOCKED;
                foreach (AccessDoor accessDoor in accessDoors) if (accessDoor.Id == args[1]) { accessDoor.CurrState=AccessDoor.State.LOCKED; break; }
                SendMessage(translation("cmd.ranch.successlock",args[1])); 
                break;
            case "unlock":
                if (ranch.AccessDoorStates[args[1]] == AccessDoor.State.OPEN)
                    return SendError(translation("cmd.ranch.errorunlock",args[1]));
                ranch.AccessDoorStates[args[1]] = AccessDoor.State.OPEN;
                foreach (AccessDoor accessDoor in accessDoors) if (accessDoor.Id == args[1]) { accessDoor.CurrState=AccessDoor.State.OPEN; break; }
                SendMessage(translation("cmd.ranch.successunlock",args[1])); 
                break;
        }

        return false;
    }
    public override void OnMainMenuUILoad()
    {
        doors = new List<string>();
    }
}
