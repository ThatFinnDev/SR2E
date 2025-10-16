using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Weather;
namespace SR2E.Commands;

internal class WeatherCommand : SR2ECommand
{
    public override string ID => "weather";
    public override string Usage => "weather <action> <action> <action>";
    public override CommandType type => CommandType.Cheat;
    
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0) return new List<string> { "list", "modify" };
        if (argIndex == 1)
        {
            if (args[0] == "list") return new List<string> { "all", "running" };
            if (args[0] == "modify")
            {
                var list = new List<string>();
                foreach (var state in LookupEUtil.weatherStateDefinitions) list.Add(state.name.Replace(" ", ""));
                return list;
            }
        }

        if (argIndex == 2) if (args[0] == "modify")
                return new List<string> { "start", "stop", "toggle" };

        switch (argIndex)
        {
            case 0:
                return new List<string> { "list", "modify" };
            case 1:
                switch (args[0])
                {
                    case "list":
                        return new List<string> { "all", "running" };
                    case "modify":
                        var list = new List<string>();
                        foreach (var state in LookupEUtil.weatherStateDefinitions) list.Add(state.name.Replace(" ", ""));
                        return list;
                }
                break;
            case 2:
                if (args[0] == "modify")
                    return new List<string> { "start", "stop", "toggle" };
                break;
        }
        
        return null;
    }
    
    public override bool Execute(string[] args)
    {
        //return SendCommandMaintenance();
        if (!args.IsBetween(0,3)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();
        
        WeatherDirector weatherDirector = Get<WeatherDirector>("WeatherVFX");
        if (weatherDirector == null) return SendError(translation("cmd.weather.nodirector")); 

        switch (args.Length)
        {
            case 1:
                if (args[0] == "list")
                {
                    var stateNames = "";
                    foreach (var state in LookupEUtil.weatherStateDefinitions) stateNames += $"\n{state.GetName()}";
                    SendMessage(translation("cmd.weather.successlist",stateNames));
                    return true;
                }

                if (args[0] == "modify") return SendError(translation("cmd.weather.requiresmore",args[0]));
                
                return SendError(translation("cmd.weather.notvalidargument",args[0]));
            case 2:
                if (args[0] == "list")
                {
                    if (args[1] == "running")
                    {
                        
                        var states = weatherDirector._runningStates;
                        var stateNames = "";
                        foreach (var state in states) stateNames += $"\n{state.GetName()}";
                        SendMessage(translation("cmd.weather.successlistrunning",stateNames));
                        return true;
                    }
                    if (args[1] == "all")
                    {
                        var stateNames = "";
                        foreach (var state in LookupEUtil.weatherStateDefinitions) stateNames += $"\n{state.GetName()}";
                        SendMessage(translation("cmd.weather.successlist",stateNames));
                        return true;
                    }
                    return SendError(translation("cmd.weather.notvalidargument",args[1]));
                }
                if (args[0] == "modify")
                {
                    
                    WeatherStateDefinition def = LookupEUtil.GetWeatherStateDefinitionByName(args[1]);
                    if (def == null) return SendNotValidWeather(args[1]); 

                    bool isRunning = weatherDirector._runningStates.Contains(def.Cast<IWeatherState>());
                    if(isRunning) SendMessage( translation("cmd.weather.currentlyrunning",$"\"{def.name.Replace(" ", "")}\""));
                    else SendMessage( translation("cmd.weather.currentlynotrunning",$"\"{def.name.Replace(" ", "")}\""));
                    
                    return true;
                }
                return SendError(translation("cmd.weather.notvalidargument",args[0]));
            case 3:
                if (args[0] == "list") return SendError(translation("cmd.weather.requiresless",args[0]));
                if (args[0] == "modify")
                {
                    
                    WeatherStateDefinition def = LookupEUtil.GetWeatherStateDefinitionByName(args[1]);
                    if (def == null) return SendNotValidWeather(args[1]); 

                    bool isRunning = weatherDirector._runningStates.Contains(def.Cast<IWeatherState>());
                    
                    if (args[2] == "start")
                    {
                        if (isRunning)
                        {
                            SendMessage( translation("cmd.weather.alreadyrunning",$"\"{def.name.Replace(" ", "")}\""));
                            return true;
                        }
                        
                        var param = new WeatherModel.ZoneWeatherParameters() { WindDirection = new Vector3(45f, 0, 30f) };
                        weatherDirector.RunState(def.Cast<IWeatherState>(), param);
                        SendMessage( translation("cmd.weather.successstart",$"\"{def.name.Replace(" ", "")}\""));
                        return true;
                    }
                    if (args[2] == "stop")
                    {
                        if (!isRunning)
                        {
                            SendMessage( translation("cmd.weather.alreadynotrunning",$"\"{def.name.Replace(" ", "")}\""));
                            return true;
                        }
                        var param = new WeatherModel.ZoneWeatherParameters() { WindDirection = new Vector3(45f, 0, 30f) };
                        weatherDirector.StopState(def.Cast<IWeatherState>(), param);
                        SendMessage( translation("cmd.weather.successstop",$"\"{def.name.Replace(" ", "")}\""));
                        return true;
                    }
                    if (args[2] == "toggle")
                    {
                        var param = new WeatherModel.ZoneWeatherParameters() { WindDirection = new Vector3(45f, 0, 30f) };
                        if (isRunning)
                        {
                            weatherDirector.StopState(def.Cast<IWeatherState>(), param);
                            SendMessage( translation("cmd.weather.successstop",$"\"{def.name.Replace(" ", "")}\""));
                        }
                        else
                        {
                            weatherDirector.RunState(def.Cast<IWeatherState>(), param);
                            SendMessage( translation("cmd.weather.successstart",$"\"{def.name.Replace(" ", "")}\""));
                        }
                        return true;
                    }
                    
                    return SendError(translation("cmd.weather.notvalidargument",args[2]));
                }
                return SendError(translation("cmd.weather.notvalidargument",args[0]));
            default:
                return SendUsage();
        }
    }

}