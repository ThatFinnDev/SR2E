using System;
using System.Linq;
using System.Reflection;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.UI.Debug;
using Il2CppTMPro;
using UnityEngine.UI;

namespace SR2E.Library
{
    internal class LibraryDebug
    {
        internal static bool playerDebugUIEnabled = false;
        private static SRCharacterController cc;
        private static PlayerDebugHudUI playerDebugHudUI = null;
        public static void DebugLogButton()
        {
            var player = LibraryUtils.player.transform;
            var playercontroller = player.GetComponent<SRCharacterController>();
            SR2Console.SendMessage($"Player Position: {player.position.x} {player.position.y} {player.position.z}\nPlayer Rotation: {player.eulerAngles.y}\nPlayer velocity: {playercontroller.Velocity.x} {playercontroller.Velocity.y} {playercontroller.Velocity.z}");
        }

        internal static void TogglePlayerDebugUI()
        {
            if (playerDebugUIEnabled) DeActivatePlayerDebugUI();
            else ActivatePlayerDebugUI();
        }
        internal static void DeActivatePlayerDebugUI()
        {
            if(!playerDebugUIEnabled) return;
            if(playerDebugHudUI==null) 
                playerDebugHudUI = Get<PlayerDebugHudUI>("PlayerDebug");
            if(playerDebugHudUI==null) return;
            if(playerDebugHudUI.gameObject.activeSelf)
                playerDebugHudUI.transform.gameObject.SetActive(false);
            playerDebugUIEnabled = false;
        }
        internal static void ActivatePlayerDebugUI()
        {
            if(playerDebugUIEnabled) return;
            if(playerDebugHudUI==null) playerDebugHudUI = Get<PlayerDebugHudUI>("PlayerDebug");
            if(playerDebugHudUI==null) return;
            playerDebugHudUI.GetComponent<VerticalLayoutGroup>().spacing = -50;
            if(!playerDebugHudUI.gameObject.activeSelf)
                playerDebugHudUI.transform.gameObject.SetActive(true);
            if(player==null) return;
            cc = player.GetComponent<SRCharacterController>();
            for (int i = 0; i < playerDebugHudUI.transform.childCount; i++)
            {
                TMP_Text tmpText = playerDebugHudUI.transform.GetChild(i).GetComponent<TMP_Text>();
                if (tmpText != null)
                    tmpText.color = new Color(1, 1, 1, 1);
            }
            playerDebugUIEnabled = true;
        }
        internal static void Update()
        {
            if(!playerDebugUIEnabled) return;
            if(playerDebugHudUI==null)
            { playerDebugUIEnabled = false; return; }
            if(cc==null)
            { playerDebugUIEnabled = false; return; }
            
            playerDebugHudUI._velocity.SetText($"FPS: {(int)(1f / Time.unscaledDeltaTime)}");
            playerDebugHudUI._horizontalVelocity.SetText($"Position: {cc.Position.x} {cc.Position.y} {cc.Position.z}");
            playerDebugHudUI._slopeText.SetText($"Rotation: {player.transform.eulerAngles.y}");
            playerDebugHudUI._playerLocation.SetText($"Velocity: {cc.Velocity.x} {cc.Velocity.y} {cc.Velocity.z}");
            playerDebugHudUI._lookInput.SetText($"LookInput: {cc.LookVector.x} {cc.LookVector.y} {cc.LookVector.z}");
            playerDebugHudUI._activeAbilities.SetText($"Slope: {cc.CurrentSlopeAngle}");
        }
    }
}
