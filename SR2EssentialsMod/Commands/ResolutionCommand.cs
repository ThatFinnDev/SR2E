namespace SR2E.Commands;

internal class ResolutionCommand : SR2ECommand
{
    public override string ID => "resolution";
    public override string Usage => "resolution <x> <y> [fullscreen(true/false)]";
    public override CommandType type => CommandType.Common;
    
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0) return new List<string> {Screen.currentResolution.width.ToString() };
        if (argIndex == 1) return new List<string> {Screen.currentResolution.height.ToString() };
        if (argIndex == 2) return new List<string> {"true","false" };
        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(2,3)) return SendUsage();
        
        int x = 1;
        if(!TryParseInt(args[0], out x, 0,false,16000)) return false;
        
        int y = 1;
        if(!TryParseInt(args[1], out y, 0,false,16000)) return false;

        bool fullscreen = true;
        if (args.Length == 3) if (!TryParseBool(args[2], out fullscreen)) return false;
        Screen.SetResolution(x,y,fullscreen);
        SendMessage(translation("cmd.resolution.success",x,y));
        return true;
    }
}