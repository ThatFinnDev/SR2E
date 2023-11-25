using Il2CppMonomiPark.KFC;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using System.Linq;
using UnityEngine;

namespace SR2E.Commands
{
    public class SpeedCommand : SR2CCommand
    {
        public static T Get<T>(string name) where T : UnityEngine.Object
        {
            return Resources.FindObjectsOfTypeAll<T>().FirstOrDefault((T x) => x.name == name);
        }

        public override string ID { get; } = "speed";
        public override string Usage { get; } = "speed float";
        public override string Description { get; } = "Sets the player speed";

        private static float baseMaxAirSpeed = 10;
        private static float baseAccAirSpeed = 60;
        private static float baseMaxGroundSpeed = 10;

        public override bool Execute(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                return false;
            }

            CharacterControllerParameters parameters;

            parameters = Get<SRCharacterController>("PlayerControllerKCC")._parameters;
            try
            {

                parameters._maxGroundedMoveSpeed = float.Parse(args[0]) * baseMaxGroundSpeed;
                parameters._maxAirMoveSpeed = float.Parse(args[0]) * baseMaxAirSpeed;
                parameters._airAccelerationSpeed = float.Parse(args[0]) * baseAccAirSpeed;

                return true;
            }
            catch { return false; }
        }
    }
}