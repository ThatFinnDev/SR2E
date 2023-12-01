using Il2CppKinematicCharacterController;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using System.Linq;
using UnityEngine.InputSystem;

namespace SR2E.Commands
{
    public class NoClipCommand : SR2CCommand
    {
        internal bool inNoclip;
        public class NoclipComponent : MonoBehaviour
        {

            public static float baseSpeed = 15f;
            public static float speedAdjust = 235f;
            public float speed = 15f;
            public Transform player;
            public KCCSettings settings;

            public void OnDestroy()
            {
                player.gameObject.GetComponent<KinematicCharacterMotor>().enabled = true;
                settings.AutoSimulation = true;
                player.GetComponent<SRCharacterController>().Position = player.position;
            }

            public void Awake()
            {
                player = SR2EUtils.Get<Transform>("PlayerControllerKCC");
                player.gameObject.GetComponent<KinematicCharacterMotor>().enabled = false;
                settings = SR2EUtils.Get<KCCSettings>("");
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


                    if (SceneContext.Instance == null) { SR2Console.SendError("Load a save first!"); return false; }
                if (SceneContext.Instance.PlayerState == null) { SR2Console.SendError("Load a save first!"); return false; }

                if (SR2EUtils.Get<SRCharacterController>("PlayerControllerKCC") != null)
                {
                    SRCharacterController Player = SR2EUtils.Get<SRCharacterController>("PlayerControllerKCC");
                    SR2EEntryPoint.RefreshPrefs();
                    if (inNoclip)
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
                return false;
            }
            catch { return false; }
        }
    }
}