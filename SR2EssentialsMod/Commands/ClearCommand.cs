﻿namespace SR2E.Commands;

internal class ClearCommand : SR2ECommand
{
    public override string ID => "clear";
    public override string Usage => "clear";
    public override CommandType type => CommandType.Common;

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0,0)) return SendNoArguments();

        for (int i = 0; i < SR2EConsole.consoleContent.childCount; i++) Object.Destroy(SR2EConsole.consoleContent.GetChild(i).gameObject);
        
        SendMessage(translation("cmd.clear.description"));
        return true;
    }
}
