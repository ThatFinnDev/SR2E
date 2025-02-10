<<<<<<< HEAD
﻿using UnityEngine.InputSystem;
=======
﻿using SR2E.Components;
using UnityEngine.InputSystem;
>>>>>>> experimental

namespace SR2E.Commands;

internal class NoClipCommand : SR2ECommand
{
<<<<<<< HEAD
    public class NoClipCommand : SR2Command
    {
        public override string ID => "noclip";
        public override string Usage => "noclip";
        public static void RemoteExc(bool n)
        {
            if (n)
            {
                //SR2ESavableDataV2.Instance.playerSavedData.noclipState = true;
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
                    //SR2ESavableDataV2.Instance.playerSavedData.noclipState = true;
                    SendMessage(translation("cmd.noclip.success"));
                }
                else
                {
                    
                    //SR2ESavableDataV2.Instance.playerSavedData.noclipState = false;
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
=======
    public override string ID => "noclip";
    public override string Usage => "noclip";
    public override CommandType type => CommandType.Cheat;

    public static void RemoteExc(bool n)
    {
        if (n)
        {
            //SR2ESavableDataV2.Instance.playerSavedData.noclipState = true;
            //var cam = SR2EUtils.Get<GameObject>("PlayerCameraKCC");
            SceneContext.Instance.Camera.AddComponent<NoClipComponent>();
        }
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0, 0)) return SendNoArguments();
        try
        {
            if (!SceneContext.Instance.Camera.RemoveComponent<NoClipComponent>())
            {
                SceneContext.Instance.Camera.AddComponent<NoClipComponent>();
                //SR2ESavableDataV2.Instance.playerSavedData.noclipState = true;
                SendMessage(translation("cmd.noclip.success"));
            }
            else
            {
                //SR2ESavableDataV2.Instance.playerSavedData.noclipState = false;
                SendMessage(translation("cmd.noclip.success2"));
            }
            return true;
        }
        catch { return SendError("cmd.noclip.error"); }
    }

    public static InputAction horizontal;
    public static InputAction vertical;

    public override void OnMainMenuUILoad()
    {
        horizontal = MainGameActions["Horizontal"];
        vertical = MainGameActions["Vertical"];
>>>>>>> experimental
    }
}