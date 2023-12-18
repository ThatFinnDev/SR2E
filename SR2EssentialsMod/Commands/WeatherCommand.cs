using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Weather;

namespace SR2E.Commands;

internal class WeatherCommand : SR2CCommand
{
    public override string ID => "forceweather";

    public override string Usage => "forceweather <state>";

    public override string Description => "Force a weather state.";

    public override string ExtendedDescription =>
        "Allows you to make any weather in the game play where you are currently.";

    public override bool Execute(string[] args)
    {
        if (args.Length != 1) return false;

        var def = Weather.getWeatherStateByName(args[0]);

        if (def == null) return false;

        var dir = Get<WeatherDirector>("WeatherVFX");

        if (dir == null) return false;

        var param = new WeatherModel.ZoneWeatherParameters
        {
            WindDirection = new Vector3(45f, 0, 30f)
        };

        dir.RunState(def.Cast<IWeatherState>(), param);
        SR2Console.SendMessage($"Successfully run weather state \"{def.name.Replace(" ", "")}\"");
        return true;
    }

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        var list = new List<string>();

        foreach (var state in Weather.states) list.Add(state.name.Replace(" ", ""));

        return list;
    }
}