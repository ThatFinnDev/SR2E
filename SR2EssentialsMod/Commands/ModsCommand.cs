<<<<<<< HEAD
﻿namespace SR2E.Commands
{
    public class ModsCommand : SR2Command
    {
        public override string ID => "mods";
        public override string Usage => "mods";
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();

            SendMessage(translation("cmd.mods.success"));

            foreach (MelonBase melonBase in MelonBase.RegisteredMelons)
                SendMessage(translation("cmd.mods.successdesc",melonBase.Info.Name,melonBase.Info.Author));
=======
﻿namespace SR2E.Commands;

internal class ModsCommand : SR2ECommand
{
    public override string ID => "mods";
    public override string Usage => "mods";
    public override CommandType type => CommandType.Common;

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0, 0)) return SendNoArguments();

        SendMessage(translation("cmd.mods.success"));

        foreach (MelonBase melonBase in MelonBase.RegisteredMelons)
            SendMessage(translation("cmd.mods.successdesc", melonBase.Info.Name, melonBase.Info.Author));
>>>>>>> experimental

        return true;
    }
}