using Il2CppMonomiPark.SlimeRancher.UI;

namespace SR2E.Commands;


public class ToggleUICommand : SR2Command
{
    public override string ID => "toggleui";
    public override string Usage => "toggleui";

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0,0)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();
        HudUI.Instance.gameObject.SetActive(!HudUI.Instance.gameObject.active);
        SceneContext.Instance.PlayerState.VacuumItem.gameObject.SetActive(HudUI.Instance.gameObject.active);
        SendMessage(translation("cmd.toggleui.success"));
        return true;
    }
}