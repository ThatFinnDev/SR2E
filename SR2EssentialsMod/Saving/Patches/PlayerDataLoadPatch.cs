using Il2CppKinematicCharacterController;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using SR2E.Commands;
using SR2E.Saving;
using SR2E;

[HarmonyPatch(typeof(SRCharacterController), nameof(SRCharacterController.Start))]
public static class PlayerDataLoadPatch
{
    public static void Postfix(SRCharacterController __instance)
    {
        try
        {

            if (SR2ESavableData.Instance.playerSavedData.vacMode == VacModes.AUTO_VAC || SR2ESavableData.Instance.playerSavedData.vacMode == VacModes.AUTO_VAC)
            {
                SR2ESavableData.Instance.playerSavedData.vacMode = VacModes.NORMAL;
            }
            NoClipCommand.RemoteExc(SR2ESavableData.Instance.playerSavedData.noclipState);
            SpeedCommand.RemoteExc(SR2ESavableData.Instance.playerSavedData.speed);
            UtilCommand.RemoteExc_PlayerSize(SR2ESavableData.Instance.playerSavedData.size);
            UtilCommand.PlayerVacModeSet(SR2ESavableData.Instance.playerSavedData.vacMode);
            SceneContext.Instance.player.GetComponent<KinematicCharacterMotor>().BaseVelocity = Vector3Data.ConvertBack(SR2ESavableData.Instance.playerSavedData.velocity);
            SceneContext.Instance.player.GetComponent<SRCharacterController>()._gravityMagnitude = new Il2CppSystem.Nullable<float>(SR2ESavableData.Instance.playerSavedData.gravityLevel);
            if (SR2ESavableData.Instance.playerSavedData.noclipState && SR2EEntryPoint.debugLogging)
            {
                SR2EConsole.SendMessage("Load noclip state debug");
            }
        }
        catch { }
    }
}