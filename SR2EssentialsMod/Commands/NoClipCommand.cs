using SR2E.Saving;

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
                //var cam = SR2EUtils.Get<GameObject>("PlayerCameraKCC");
                SceneContext.Instance.Camera.AddComponent<NoClipComponent>();
            }
        }

        public override bool Execute(string[] args)
        {
            if (args != null) return SendUsage();
            try
            {
                if (!SceneContext.Instance.Camera.RemoveComponent<NoClipComponent>())
                {
                    SceneContext.Instance.Camera.AddComponent<NoClipComponent>();
                    SR2EConsole.SendMessage("FYI: Controller input for noclip doesn't work yet!");
                    SR2ESavableData.Instance.playerSavedData.noclipState = true;
                }
                else
                    SR2ESavableData.Instance.playerSavedData.noclipState = false;
                return true;
            }
            catch { return false; }
        }
    }
}