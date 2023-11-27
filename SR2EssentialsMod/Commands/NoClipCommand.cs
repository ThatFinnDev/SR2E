using Il2CppKinematicCharacterController;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using System.Linq;
using UnityEngine.InputSystem;

namespace SR2E.Commands
{
    public class NoClipCommand : SR2CCommand
    {
        public static T Get<T>(string name) where T : UnityEngine.Object
        {
            return Resources.FindObjectsOfTypeAll<T>().FirstOrDefault(x => x.name == name);
        }

        public class NoclipComponent : MonoBehaviour
        {
            public static T Get<T>(string name) where T : UnityEngine.Object
            {
                return Resources.FindObjectsOfTypeAll<T>().FirstOrDefault((T x) => x.name == name);
            }

            public static float baseSpeed = 15f;
            public static float speedAdjust = 235f;
            public float speed = 15f;
            public Transform player;
            public KCCSettings settings;
            private Vector2 lastMousePos;

            public void OnDestroy()
            {
                player.gameObject.GetComponent<KinematicCharacterMotor>().enabled = true;
                settings.AutoSimulation = true;
                player.GetComponent<SRCharacterController>().Position = player.position;
            }

            public void Awake()
            {
                player = Get<Transform>("PlayerControllerKCC");
                player.gameObject.GetComponent<KinematicCharacterMotor>().enabled = false;
                settings = Get<KCCSettings>("");
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
                var cam = Get<GameObject>("PlayerCameraKCC");
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
                var cam = Get<GameObject>("PlayerCameraKCC");
                if (cam.GetComponent<NoclipComponent>() == null)
                {
                    cam.AddComponent<NoclipComponent>();
                }
                else
                {
                    UnityEngine.Object.Destroy(cam.GetComponent<NoclipComponent>());
                }
                return true;
            }
            catch { return false; }
        }
    }
}