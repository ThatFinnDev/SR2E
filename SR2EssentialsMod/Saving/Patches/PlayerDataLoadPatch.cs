using Il2CppKinematicCharacterController;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using SR2E.Commands;
using SR2E.Saving;

[HarmonyPatch(typeof(SRCharacterController), nameof(SRCharacterController.Start))]
public static class PlayerDataLoadPatch
{
    public static void Postfix(SRCharacterController __instance)
    {
        try
        {

            if (SR2ESavableDataV2.Instance.playerSavedData.vacMode == VacModes.AUTO_VAC || SR2ESavableDataV2.Instance.playerSavedData.vacMode == VacModes.AUTO_VAC)
            {
                SR2ESavableDataV2.Instance.playerSavedData.vacMode = VacModes.NORMAL;
            }
            NoClipCommand.RemoteExc(SR2ESavableDataV2.Instance.playerSavedData.noclipState);
            SpeedCommand.RemoteExc(SR2ESavableDataV2.Instance.playerSavedData.speed);
            UtilCommand.RemoteExc_PlayerSize(SR2ESavableDataV2.Instance.playerSavedData.size);
            UtilCommand.PlayerVacModeSet(SR2ESavableDataV2.Instance.playerSavedData.vacMode);
            SceneContext.Instance.player.GetComponent<KinematicCharacterMotor>().BaseVelocity = Vector3Data.ConvertBack(SR2ESavableDataV2.Instance.playerSavedData.velocity);
            SceneContext.Instance.player.GetComponent<SRCharacterController>()._gravityMagnitude = new Il2CppSystem.Nullable<float>(SR2ESavableDataV2.Instance.playerSavedData.gravityLevel);
            if (SR2ESavableDataV2.Instance.playerSavedData.noclipState && SR2EEntryPoint.debugLogging)
            {
                SR2EConsole.SendMessage("Load noclip state debug");
            }
        }
        catch { }
    }
}