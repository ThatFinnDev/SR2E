using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Library
{
    internal class LibraryDebug
    {
        public static void DebugLogButton()
        {
            var player = LibraryUtils.player.transform;
            var playercontroller = player.GetComponent<SRCharacterController>();
            SR2Console.SendMessage($"Player Position: {player.position.x} {player.position.y} {player.position.z}\nPlayer Rotation (Euler): {player.eulerAngles.x} {player.eulerAngles.y} {player.eulerAngles.z}\nPlayer Rotation (True): {player.rotation.x} {player.rotation.y} {player.rotation.z} {player.rotation.w}\nPlayer velocity: {playercontroller.Velocity.x} {playercontroller.Velocity.y} {playercontroller.Velocity.z}");
        }
    }
}
