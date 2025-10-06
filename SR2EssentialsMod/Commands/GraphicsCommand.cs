using SR2E.Managers;

namespace SR2E.Commands;

internal class GraphicsCommand : SR2ECommand
{
    public override string ID => "graphics";
    public override string Usage => "graphics <mode>";
    public override CommandType type => CommandType.Fun;
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    { 
        //return new List<string>(){"InMaintenance"};
        if (argIndex == 0)
        {
            var list = new List<string> { "NORMAL" };
            list.AddRange(SR2EVolumeProfileManager.presets.Keys);
            return list;
        }
        return null;
    }


    internal static GameObject rangeLightInstance;
    internal static bool activated = false;
    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,1)) return SendUsage();
        
        if (args[0] == "NORMAL")
        {
            SR2EVolumeProfileManager.DisableProfile();
            SendMessage(translation("cmd.graphics.success","NORMAL"));
            return true;
        }

        foreach (var preset in SR2EVolumeProfileManager.presets.Keys)
            if (preset == args[0])
            {
                SR2EVolumeProfileManager.DisableProfile();
                SR2EVolumeProfileManager.EnableProfile(preset);
                SendMessage(translation("cmd.graphics.success",preset));
                return true;
            }
        
        SR2EVolumeProfileManager.DisableProfile();
        SendMessage(translation("cmd.graphics.success","NORMAL"));
        return true;
    }
}