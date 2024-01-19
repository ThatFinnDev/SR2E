using Il2CppMonomiPark.SlimeRancher.World;
using SR2E.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace SR2E.Commands.Secret
{
    internal class StrikeCommand : SR2CCommand
    {
        public override string ID => "strike";

        public override string Usage => "strike [power]";

        public override string Description => "Spawns a lightning strike";

        public static bool setup = false;

        public static GameObject lightningPrefab;

        public override void Update()
        {
            if (SceneManager.GetSceneByName("GameCore").isLoaded && !setup)
            {
                setup = true;
                lightningPrefab = Object.Instantiate(Get("LightningStrike"));
                lightningPrefab.MakePrefab();
                lightningPrefab.name = "InstantLightning";
                var l = lightningPrefab.GetComponent<LightningStrike>();
                l.WarningTime = 0.5f;
                l.SpawnOptions.RemoveAt(0);
                l.SpawnOptions.RemoveAt(0);
                l._strikeTime = 8f;
                l.BlastPower = 2750f;
                l.BlastRadius = 9f;

            }
        }

        public override bool Execute(string[] args)
        {
            if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
            {
                var newPrefab = Object.Instantiate(lightningPrefab);
                newPrefab.transform.position = hit.point;
                if (args != null && args.Length == 1)
                {
                    try
                    {
                        newPrefab.GetComponent<LightningStrike>().BlastPower = float.Parse(args[0]) * 2750f;
                    }
                    catch
                    {
                        SR2Console.SendError("Invalid number!");
                        return false;
                    }
                }
                SR2Console.SendMessage("Summoned lightning!");
                return true;
            }
            return false;
        }
    }
}
