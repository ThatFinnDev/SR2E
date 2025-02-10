namespace SR2E.Commands;

<<<<<<< HEAD
public class ResolutionCommand : SR2Command
{
    public override string ID => "resolution";
    public override string Usage => "resolution <x> <y> [fullscreen(true/false)]";
    

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return new List<string> {Screen.currentResolution.width.ToString() };
        if (argIndex == 1)
            return new List<string> {Screen.currentResolution.height.ToString() };
        if (argIndex == 2)
            return new List<string> {"true","false" };
=======
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
>>>>>>> experimental
        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(2,3)) return SendUsage();
        
        int x = 1;
<<<<<<< HEAD
        try { x = int.Parse(args[0]); }
        catch { return SendError(translation("cmd.error.notvalidint",args[0])); }
        if (x <= 0) return SendError(translation("cmd.error.notintabove",args[0],0));
        if (x >= 16000) return SendError(translation("cmd.error.notintbelow",args[0],16000));
        
        int y = 1;
        try { y = int.Parse(args[1]); }
        catch { return SendError(translation("cmd.error.notvalidint",args[1])); }
        if (y <= 0) return SendError(translation("cmd.error.notintabove",args[1],0));
        if (y >= 16000) return SendError(translation("cmd.error.notintbelow",args[1],16000));

        bool fullscreen = true;
        if (args.Length == 3)
        {
            string boolToParse = args[2].ToLower();
            if (boolToParse != "true" && boolToParse != "false") return SendError(translation("cmd.error.notvalidbool",args[2]));
            if (boolToParse == "false") fullscreen = false;
        }
=======
        if(!this.TryParseInt(args[0], out x, 0,false,16000)) return false;
        
        int y = 1;
        if(!this.TryParseInt(args[1], out y, 0,false,16000)) return false;

        bool fullscreen = true;
        if (args.Length == 3) if (!this.TryParseBool(args[2], out fullscreen)) return false;
>>>>>>> experimental
        Screen.SetResolution(x,y,fullscreen);
        SendMessage(translation("cmd.resolution.success",x,y));
        return true;
    }
}