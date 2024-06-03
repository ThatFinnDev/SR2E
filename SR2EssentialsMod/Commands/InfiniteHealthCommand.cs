using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.UI;

namespace SR2E.Commands;

public class InfiniteHealthCommand : SR2Command
{
    public override string ID => "infhealth";
    public override string Usage => "infhealth";

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0,0)) return SendNoArguments();
        if (!inGame) return SendLoadASaveFirst();

        if (SR2EEntryPoint.infHealthInstalled) { SR2EConsole.SendError(translation("cmd.infhealth.dedicatedmodinstalled")); return false; }

        if (infHealth)
        {
            infHealth = false;
            healthMeter.gameObject.active = true;
            SendMessage(translation("cmd.infhealth.successnolonger"));
        }
        else
        {
            infHealth = true;
            healthMeter.gameObject.active = false;
            SendMessage(translation("cmd.infhealth.success"));
        }

        return true;
    }

    public override void OnMainMenuUILoad()
    {
        infHealth = false;
    }

    static bool infHealth = false;
    static HealthMeter _healthMeter;

    static HealthMeter healthMeter
    {
        get
        {
            if (_healthMeter == null) _healthMeter = Get<HealthMeter>("Health Meter");
            return _healthMeter;
        }
    }

    [HarmonyPatch(typeof(PlayerModel), nameof(PlayerModel.LoseHealth))]
    public class PlayerModelLoseHealthPatch
    {
        public static bool Prefix(PlayerState __instance, float health)
        {
            return !infHealth;
        }
    }

}

