using Il2CppMonomiPark.KFC;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using SR2E.Saving;

namespace SR2E.Commands
{
    public class SpeedCommand : SR2CCommand
    {
        public override string ID { get; } = "speed";
        public override string Usage { get; } = "speed <speed>";
        public override string Description { get; } = "Sets the player speed";

        private static float baseMaxAirSpeed = 10;
        private static float baseAccAirSpeed = 60;
        private static float baseMaxGroundSpeed = 10;

        public static void RemoteExc(float val)
        {
            var parameters = SR2EUtils.Get<SRCharacterController>("PlayerControllerKCC")._parameters;

            parameters._maxGroundedMoveSpeed = val * baseMaxGroundSpeed;
            parameters._maxAirMoveSpeed = val * baseMaxAirSpeed;
            parameters._airAccelerationSpeed = val * baseAccAirSpeed;
        }
        public override bool Execute(string[] args)
        {
            if (args == null) { SR2Console.SendMessage($"Usage: {Usage}"); return false; }
            if (args.Length != 1) { SR2Console.SendMessage($"Usage: {Usage}"); return false; }
            if (!SR2EUtils.inGame) { SR2Console.SendError("Load a save first!"); return false; }

            CharacterControllerParameters parameters;

            parameters = SR2EUtils.Get<SRCharacterController>("PlayerControllerKCC")._parameters;
            float speedValue = 0;
            if (!float.TryParse(args[0], out speedValue))
            { SR2Console.SendError(args[1] + " is not a valid float!"); return false; }
            try
            {
                
                parameters._maxGroundedMoveSpeed = speedValue * baseMaxGroundSpeed;
                parameters._maxAirMoveSpeed = speedValue * baseMaxAirSpeed;
                parameters._airAccelerationSpeed = speedValue * baseAccAirSpeed;

                SR2ESavableData.Instance.playerSavedData.speed = speedValue;

                SR2Console.SendMessage($"Speed set to {args[0]}");
                return true;
            }
            catch { SR2Console.SendError("An unknown error occured!"); return false; }
        }
    }
}