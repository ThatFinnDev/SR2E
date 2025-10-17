using System.Linq;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.Weather;

namespace CottonLibrary;

public static partial class Library
{
    public static WeatherStateDefinition? GetWeatherStateByName(string name)
    {
        return autoSaveDirector._configuration.WeatherStates.items._items.FirstOrDefault(x =>
            name.ToUpper().Replace(" ", "") == x.name.Replace(" ", "").ToUpper());
    }

}