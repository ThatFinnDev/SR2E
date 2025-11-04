using SR2E.Components;
using UnityEngine.InputSystem;

namespace SR2E.Commands;

internal class NoClipCommand : SR2ECommand
{
    public override string ID => "noclip";
    public override string Usage => "noclip";
    public override CommandType type => CommandType.Cheat;

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0, 0)) return SendNoArguments();
        try
        {
            if (!sceneContext.Camera.RemoveComponent<NoClipComponent>())
            {
                sceneContext.Camera.AddComponent<NoClipComponent>();
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
        horizontal = LookupEUtil.MainGameActions["Horizontal"];
        vertical = LookupEUtil.MainGameActions["Vertical"];
    }
}