using Il2CppMonomiPark.KFC;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using System.Linq;

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
            if (args == null) { SR2Console.SendMessage($"Usage: {Usage}"); return false; }
            if (args.Length != 1) { SR2Console.SendMessage($"Usage: {Usage}"); return false; }
            if (SceneContext.Instance == null) { SR2Console.SendError("Load a save first!"); return false; }
            if (SceneContext.Instance.PlayerState == null) { SR2Console.SendError("Load a save first!"); return false; }

            CharacterControllerParameters parameters;

            parameters = Get<SRCharacterController>("PlayerControllerKCC")._parameters;
            float speedValue = 0;
            if (!float.TryParse(args[0], out speedValue))
            { SR2Console.SendError(args[1] + " is not a valid float!"); return false; }
            try
            {
                
                parameters._maxGroundedMoveSpeed = speedValue * baseMaxGroundSpeed;
                parameters._maxAirMoveSpeed = speedValue * baseMaxAirSpeed;
                parameters._airAccelerationSpeed = speedValue * baseAccAirSpeed;

                SR2Console.SendMessage($"Speed set to {args[0]}");
                return true;
            }
            catch { SR2Console.SendError("An unknown error occured!"); return false; }
        }
    }
}