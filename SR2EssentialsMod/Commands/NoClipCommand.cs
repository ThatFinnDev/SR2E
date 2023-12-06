using Il2CppKinematicCharacterController;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using SR2E.Saving;
using UnityEngine.InputSystem;

namespace SR2E.Commands
{
    public class NoClipCommand : SR2CCommand
    {

        

        public override string ID => "noclip";

        public override string Usage => "noclip";

        public override string Description => "Toggles noclip";

        public static void RemoteExc(bool n)
        {
            if (n)
            {
                SR2ESavableData.Instance.playerSavedData.noclipState = true;
                var cam = SR2EUtils.Get<GameObject>("PlayerCameraKCC");
                cam.AddComponent<NoclipComponent>();
            }
        }

        public override bool Execute(string[] args)
        {
            if (args != null)
            {
                return false;
            }
            try
            {
                var cam = SR2EUtils.Get<GameObject>("PlayerCameraKCC");
                if (!cam.RemoveComponent<NoclipComponent>())

                {
                    cam.AddComponent<NoclipComponent>();
                    SR2ESavableData.Instance.playerSavedData.noclipState = true;
                }
                return true;
            }
            catch { return false; }
        }
    }
}