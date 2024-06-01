using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Weather;

namespace SR2E.Commands;

public class WeatherCommand : SR2Command
{
    public override string ID => "weather";
    public override string Usage => "weather <action> <action> <action>";
    public override string Description => "Manage the weather.";
    public override string ExtendedDescription =>
        "Allows you to start/stop weathers as well as to view every weather.";
    
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0) return new List<string> { "list", "change" };
        if (argIndex == 1)
        {
            if (args[0] == "list") return new List<string> { "all", "running" };
            if (args[0] == "change")
            {
                var list = new List<string>();
                foreach (var state in weatherStateDefinitions) list.Add(state.name.Replace(" ", ""));
                return list;
            }
        }

        if (argIndex == 2) if (args[0] == "change")
                return new List<string> { "start", "stop" };
        
        return null;
    }
    
    public override bool Execute(string[] args)
    {
        if (args == null) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();
        
        WeatherDirector weatherDirector = Get<WeatherDirector>("WeatherVFX");
        if (weatherDirector == null) { SendError("An error occured, cannot find WeatherDirector!"); return false; }

        switch (args.Length)
        {
            case 1:
                if (args[0] == "list")
                {
                    var stateNames = "";
                    foreach (var state in weatherStateDefinitions) stateNames += $"\n{state.GetName()}";
                    SendMessage($"All weathers are:${stateNames}");
                    return true;
                }

                if (args[0] == "modify")
                {
                    SendError("'modify' requires more arguments!");
                    return false;
                }
                SendError(args[0] +" is not a valid argument!");
                return false;
            case 2:
                if (args[0] == "list")
                {
                    if (args[1] == "running")
                    {
                        
                        var states = weatherDirector._runningStates;
                        var stateNames = "";
                        foreach (var state in states) stateNames += $"\n{state.GetName()}";
                        SendMessage($"Running weathers are:${stateNames}");
                        return true;
                    }
                    if (args[1] == "change")
                    {
                        var stateNames = "";
                        foreach (var state in weatherStateDefinitions) stateNames += $"\n{state.GetName()}";
                        SendMessage($"All weathers are:${stateNames}");
                        return true;
                    }
                    SendError(args[1] +" is not a valid argument!");
                    return false;
                }
                if (args[0] == "modify")
                {
                    
                    WeatherStateDefinition def = getWeatherStateByName(args[1]);
                    if (def == null) { SendError($"The weather {args[1]} doesn't exist!"); return false; }

                    bool isRunning = weatherDirector._runningStates.Contains(def.Cast<IWeatherState>());
                    if(isRunning) SendMessage($"The weather \"{def.name.Replace(" ", "")}\" is currently running!");
                    else SendMessage($"The weather \"{def.name.Replace(" ", "")}\" is currently not running!");
                    
                    return true;
                }
                SendError(args[0] +" is not a valid argument!");
                return false;
            case 3:
                if (args[0] == "list")
                {
                    SendError("'list' requires less arguments!");
                    return false;
                }
                if (args[0] == "modify")
                {
                    
                    WeatherStateDefinition def = getWeatherStateByName(args[1]);
                    if (def == null) { SendError($"The weather {args[1]} doesn't exist!"); return false; }

                    bool isRunning = weatherDirector._runningStates.Contains(def.Cast<IWeatherState>());
                    
                    if (args[2] == "start")
                    {
                        if (isRunning)
                        {
                            SendMessage($"The weather \"{def.name.Replace(" ", "")}\" is already running!");
                            return true;
                        }
                        
                        var param = new WeatherModel.ZoneWeatherParameters() { WindDirection = new Vector3(45f, 0, 30f) };
                        weatherDirector.RunState(def.Cast<IWeatherState>(), param);
                        SendMessage($"Successfully started weather \"{def.name.Replace(" ", "")}\"");
                        return true;
                    }
                    if (args[2] == "stop")
                    {
                        if (!isRunning)
                        {
                            SendMessage($"The weather \"{def.name.Replace(" ", "")}\" is not running!");
                            return true;
                        }
                        var param = new WeatherModel.ZoneWeatherParameters() { WindDirection = new Vector3(45f, 0, 30f) };
                        weatherDirector.StopState(def.Cast<IWeatherState>(), param);
                        SendMessage($"Successfully stopped weather \"{def.name.Replace(" ", "")}\"");
                        return true;
                    }
                    if (args[2] == "toggle")
                    {
                        var param = new WeatherModel.ZoneWeatherParameters() { WindDirection = new Vector3(45f, 0, 30f) };
                        if (isRunning)
                        {
                            weatherDirector.StopState(def.Cast<IWeatherState>(), param);
                            SendMessage($"Successfully stopped weather \"{def.name.Replace(" ", "")}\"");
                        }
                        else
                        {
                            weatherDirector.RunState(def.Cast<IWeatherState>(), param);
                            SendMessage($"Successfully started weather \"{def.name.Replace(" ", "")}\"");
                        }
                        return true;
                    }
                    
                    SendError(args[2] +" is not a valid argument!");
                    return false;
                }
                SendError(args[0] +" is not a valid argument!");
                return false;
            default:
                return SendUsage();
        }
    }

}