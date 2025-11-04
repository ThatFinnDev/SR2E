using Il2CppMonomiPark.SlimeRancher.UI;

namespace SR2E.Commands;
internal class ToggleUICommand : SR2ECommand
{
    public override string ID => "toggleui";
    public override string Usage => "toggleui";
    public override CommandType type => CommandType.Miscellaneous;

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0,0)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();
        HudUI.Instance.gameObject.SetActive(!HudUI.Instance.gameObject.active);
        sceneContext.PlayerState.VacuumItem.gameObject.SetActive(HudUI.Instance.gameObject.active);
        SendMessage(translation("cmd.toggleui.success"));
        return true;
    }
}