using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.Persist;
using Il2CppMonomiPark.SlimeRancher.Weather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Commands
{
    internal class WeatherCommand : SR2CCommand
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

        public override string ID => "forceweather";

        public override string Usage => "forceweather <state>";

        public override string Description => "Force a weather state.";

        public override string ExtendedDescription => "Allows you to make any weather in the game play where you are currently.";

        public override bool Execute(string[] args)
        {
            if (args.Length != 1) return false;
            
            WeatherStateDefinition def = getWeatherStateByName(args[0]);
            
            if (def == null) return false;

            var dir = SR2EUtils.Get<WeatherDirector>("WeatherVFX");

            if (dir == null) return false;

            var param = new Il2CppMonomiPark.SlimeRancher.DataModel.WeatherModel.ZoneWeatherParameters()
            {
                WindDirection = new Vector3(45f, 0, 30f)
            };

            dir.RunState(def.Cast<IWeatherState>(), param);
            return true;
        }

        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            var list = new List<string>();

            foreach (var state in states)
            {
                list.Add(state.name.Replace(" ", ""));
            }
            return list;
        }
    }
}
