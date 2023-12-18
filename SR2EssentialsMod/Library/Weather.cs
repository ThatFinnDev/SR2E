using Il2CppMonomiPark.SlimeRancher.Weather;

namespace SR2E.Library;

public class Weather
{
    public static WeatherStateDefinition[] states => Resources.FindObjectsOfTypeAll<WeatherStateDefinition>();
    internal static WeatherStateDefinition getWeatherStateByName(string name)
    {
        foreach (WeatherStateDefinition state in states)
            try
            {
                if (state.name.ToUpper().Replace(" ", "") == name.ToUpper())
                    return state;
            }
            catch (System.Exception ignored)
            { }
        return null;
    }
}