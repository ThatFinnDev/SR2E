using Il2CppKinematicCharacterController;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using SR2E.Saving;
using UnityEngine.InputSystem;

namespace SR2E.Commands
{
    public class NoClipCommand : SR2CCommand
    {

        public class NoclipComponent : MonoBehaviour
        {
            public static float speedAdjust => SR2EEntryPoint.noclipAdjustSpeed;
            public float speed = 15f;
            public Transform player;
            public KCCSettings settings;

            public void OnDestroy()
            {
                try
                {
                    player.gameObject.GetComponent<KinematicCharacterMotor>().enabled = true;
                    settings.AutoSimulation = true;
                    player.GetComponent<SRCharacterController>().Position = player.position;
                }
                catch
                {
                    // ignore error
                }
            }

            public void Awake()
            {
                player = SceneContext.Instance.player.transform;
                player.gameObject.GetComponent<KinematicCharacterMotor>().enabled = false;
                settings = Object.FindFirstObjectByType<KCCSettings>();
                settings.AutoSimulation = false;
            }

            public void Update()
            {
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                {
                    player.position += -transform.right * (speed * Time.deltaTime);
                }

                if (Keyboard.current.shiftKey.isPressed)
                {
                    speed = baseSpeed * 2;
                }
                else
                {
                    speed = baseSpeed;
                }

                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                {
                    player.position += transform.right * (speed * Time.deltaTime);
                }

                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                {
                    player.position += transform.forward * (speed * Time.deltaTime);
                }

                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                {
                    player.position += -transform.forward * (speed * Time.deltaTime);
                }

                if (Mouse.current.scroll.ReadValue().y > 0)
                {
                    baseSpeed += (speedAdjust * Time.deltaTime);
                }
                if (Mouse.current.scroll.ReadValue().y < 0)
                {
                    baseSpeed -= (speedAdjust * Time.deltaTime);
                }
                if (baseSpeed < 1)
                {
                    baseSpeed = 1.01f;
                }
            }
        }

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
                if (cam.GetComponent<NoclipComponent>() == null)

                {
                    cam.AddComponent<NoclipComponent>();
                    SR2ESavableData.Instance.playerSavedData.noclipState = true;
                }
                else
                {
                    UnityEngine.Object.Destroy(cam.GetComponent<NoclipComponent>());
                    SR2ESavableData.Instance.playerSavedData.noclipState = false;
                }
                return true;
            }
            catch { return false; }
        }
    }
}