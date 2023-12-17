using Il2CppMonomiPark.SlimeRancher.Weather;

namespace SR2E.Commands;

public class ListWeathersCommand : SR2CCommand
{
    public override string ID => "listweather";
    public override string Usage => "listweather";
    public override string Description => "Lists all Running Weather States";

    public override bool Execute(string[] args)
    {
        var states = Get<WeatherDirector>("WeatherVFX")._runningStates;
        string stateNames = "";
        foreach (var state in states)
        {
            stateNames += $"\n{state.GetName()}";
        }
        SR2Console.SendMessage($"Running States are:${stateNames}");
        return true;
    }
}