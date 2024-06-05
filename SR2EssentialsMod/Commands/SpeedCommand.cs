﻿using Il2CppMonomiPark.KFC;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using SR2E.Saving;

namespace SR2E.Commands;

public class SpeedCommand : SR2Command
{
    public override string ID { get; } = "speed";
    public override string Usage { get; } = "speed <speed>";

    private static float baseMaxAirSpeed = 10;
    private static float baseAccAirSpeed = 60;
    private static float baseMaxGroundSpeed = 10;
    private static CharacterControllerParameters parameters;

    public static void RemoteExc(float val)
    {
        if (parameters == null)
            parameters = Get<SRCharacterController>("PlayerControllerKCC")._parameters;
        if (parameters == null) return;
        parameters._maxGroundedMoveSpeed = val * baseMaxGroundSpeed;
        parameters._maxAirMoveSpeed = val * baseMaxAirSpeed;
        parameters._airAccelerationSpeed = val * baseAccAirSpeed;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,1)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();

        if (parameters == null)
            parameters = Get<SRCharacterController>("PlayerControllerKCC")._parameters;
        if (parameters == null)
            return SendError("cmd.error.srccnull");
        float speedValue = 0;
        if (!float.TryParse(args[0], out speedValue)) return SendError(translation("cmd.error.notvalidfloat",args[0]));
        try
        {

            parameters._maxGroundedMoveSpeed = speedValue * baseMaxGroundSpeed;
            parameters._maxAirMoveSpeed = speedValue * baseMaxAirSpeed;
            parameters._airAccelerationSpeed = speedValue * baseAccAirSpeed;

            SR2ESavableDataV2.Instance.playerSavedData.speed = speedValue;

            SendMessage(translation("cmd.speed.success",args[0]));
            return true;
        }
        catch
        {
            return SendError(translation("cmd.speed.unknownerror"));
        }
    }
}
