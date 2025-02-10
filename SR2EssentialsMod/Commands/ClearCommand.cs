<<<<<<< HEAD
﻿namespace SR2E.Commands;

public class ClearCommand : SR2Command
{
    public override string ID => "clear";
    public override string Usage => "clear";
=======
﻿using SR2E.Menus;

namespace SR2E.Commands;

internal class ClearCommand : SR2ECommand
{
    public override string ID => "clear";
    public override string Usage => "clear";
    public override CommandType type => CommandType.Common;
>>>>>>> experimental

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0,0)) return SendNoArguments();

<<<<<<< HEAD
        for (int i = 0; i < SR2EConsole.consoleContent.childCount; i++) Object.Destroy(SR2EConsole.consoleContent.GetChild(i).gameObject);
        
        SendMessage(translation("cmd.clear.description"));
=======
        for (int i = 0; i < GM<SR2EConsole>().consoleContent.childCount; i++) Object.Destroy(GM<SR2EConsole>().consoleContent.GetChild(i).gameObject);

>>>>>>> experimental
        return true;
    }
}
