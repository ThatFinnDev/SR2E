using Il2CppMonomiPark.SlimeRancher.Weather;

namespace SR2E.Commands;

public class StopWeatherCommand : SR2CCommand
{
    
    public override string ID => "stopweather";
    public override string Usage => "stopweather <state>";
    public override string Description => "Stops a weather state.";


    public override bool Execute(string[] args)
    {
        if (args.Length != 1) return false;
        WeatherStateDefinition def = Weather.getWeatherStateByName(args[0]);
        if (def == null) return false;
        var dir = Get<WeatherDirector>("WeatherVFX");
        if (dir == null) return false;
        if (!dir._runningStates.Contains(def.Cast<IWeatherState>()))
        {
            SR2Console.SendError($"State \"{def.name}\" is not running");
            return false;
        }
        var param = new Il2CppMonomiPark.SlimeRancher.DataModel.WeatherModel.ZoneWeatherParameters()
        {
            WindDirection = new Vector3(45f, 0, 30f)
        };
        dir.StopState(def.Cast<IWeatherState>(), param);
        SR2Console.SendMessage($"Successfully stopped weather state \"{def.name.Replace(" ", "")}\"");
        return true;
    }

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        var list = new List<string>();

        foreach (var state in Weather.states)
        {
            list.Add(state.name.Replace(" ", ""));
        }

        return list;
    }
}