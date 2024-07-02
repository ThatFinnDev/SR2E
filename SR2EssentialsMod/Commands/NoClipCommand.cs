using SR2E.Saving;
using UnityEngine.InputSystem;

namespace SR2E.Commands
{
    public class NoClipCommand : SR2Command
    {
        public override string ID => "noclip";
        public override string Usage => "noclip";
        public static void RemoteExc(bool n)
        {
            if (n)
            {
                SR2ESavableDataV2.Instance.playerSavedData.noclipState = true;
                //var cam = SR2EUtils.Get<GameObject>("PlayerCameraKCC");
                SceneContext.Instance.Camera.AddComponent<NoClipComponent>();
            }
        }

        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();
            try
            {
                if (!SceneContext.Instance.Camera.RemoveComponent<NoClipComponent>())
                {
                    SceneContext.Instance.Camera.AddComponent<NoClipComponent>();
                    SR2ESavableDataV2.Instance.playerSavedData.noclipState = true;
                    SR2EConsole.SendWarning(translation("cmd.noclip.info"));
                    SendMessage(translation("cmd.noclip.success"));
                }
                else
                {
                    
                    SR2ESavableDataV2.Instance.playerSavedData.noclipState = false;
                    SendMessage(translation("cmd.noclip.success2"));
                }
                return true;
            }
            catch { return false; }
        }

        public static InputAction horizontal;
        public static InputAction vertical;
        public override void OnMainMenuUILoad()
        {
            horizontal = MainGameActions["Horizontal"];
            vertical = MainGameActions["Vertical"];
        }
    }
}