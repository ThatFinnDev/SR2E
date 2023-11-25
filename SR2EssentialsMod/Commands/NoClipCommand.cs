using System.Collections.Generic;
using Il2Cpp;
using Il2CppKinematicCharacterController;
using Il2CppMonomiPark.KFC;
using Il2CppMonomiPark.KFC.FirstPerson;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController.MovementAndLookTypes;
using MelonLoader;
using UnityEngine;

namespace SR2E.Commands
{
    //NOT WORKING YET
    public class NoClipCommand : SR2CCommand
    {
        public override string ID => "noclip";
        public override string Usage => "noclip";
        public override string Description => "Toggles noclip";
        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            return null;
        }

        internal static bool inNoClip = false;
        static LayerMask layerMask;
        public override bool Execute(string[] args)
        {
            if (args != null)
            {
                SR2Console.SendError($"The '<color=white>{ID}</color>' command takes no arguments");
                return false;
            }

            
            if (SceneContext.Instance == null) { SR2Console.SendError("Load a save first!"); return false; }
            if (SceneContext.Instance.PlayerState == null) { SR2Console.SendError("Load a save first!"); return false; }

            if (SR2EEntryPoint.Get<SRCharacterController>("PlayerControllerKCC") != null)
            {
                SRCharacterController Player = SR2EEntryPoint.Get<SRCharacterController>("PlayerControllerKCC");
                SR2EEntryPoint.RefreshPrefs();
                if (inNoClip)
                {
                    inNoClip = false;
                    Player.BypassGravity = false;
                    Player._motor.CollidableLayers = layerMask;
                    Player.GetComponent<KinematicCharacterMotor>().enabled = true;
                    Player._motor.SetCapsuleCollisionsActivation(true);
                    Player._motor.Capsule.enabled = true;
                    SR2Console.SendMessage("NoClip is now off!");
                    return true;
                }
                else
                {
                    inNoClip = true;
                    layerMask = Player._motor.CollidableLayers;
                    Player._motor.CollidableLayers = 0;
                    Player.BypassGravity = true;
                    Player.GetComponent<KinematicCharacterMotor>().enabled = false;
                    Player._motor.Capsule.enabled = false;
                    Player.Velocity = Vector3.zero;
                    Player._motor.SetCapsuleCollisionsActivation(false);
                    SR2Console.SendMessage("NoClip is now on!");
                    return true;
                }


            }
            SR2Console.SendError("An unknown error occured!");
            return false;
        }
    }
}