using Il2CppMonomiPark.SlimeRancher.UI;

namespace SR2E.Commands;


public class ToggleUICommand : SR2Command
{
    public override string ID => "toggleui";
    public override string Usage => "toggleui";

    public override bool Execute(string[] args)
    {
        HudUI.Instance.gameObject.SetActive(!HudUI.Instance.gameObject.active);
        SceneContext.Instance.PlayerState.VacuumItem.gameObject.SetActive(HudUI.Instance.gameObject.active);
        return true;
    }
}