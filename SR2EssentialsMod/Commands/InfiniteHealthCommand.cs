using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.UI;

namespace SR2E.Commands;

internal class InfiniteHealthCommand : SR2ECommand
{
    public override string ID => "infhealth";
    public override string Usage => "infhealth";
    public override CommandType type => CommandType.Cheat;

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0,0)) return SendNoArguments();
        if (!inGame) return SendLoadASaveFirst();

        if (infHealth)
        {
            infHealth = false;
            if (healthMeter == null) healthMeter = GetInScene<HealthMeter>("Health Meter");
            healthMeter.gameObject.active = true;
            SendMessage(translation("cmd.infhealth.successnolonger"));
        }
        else
        {
            infHealth = true;;
            if (healthMeter == null) healthMeter = GetInScene<HealthMeter>("Health Meter");
            healthMeter.gameObject.active = false;
            SendMessage(translation("cmd.infhealth.success"));
        }

        return true;
    }

    public override void OnMainMenuUILoad() => infHealth = false;
    public static bool infHealth = false;
    private static HealthMeter healthMeter;
    public override void OnUICoreLoad() => healthMeter = GetInScene<HealthMeter>("Health Meter");
    [HarmonyPatch(typeof(PlayerModel), nameof(PlayerModel.LoseHealth))]
    internal class PlayerModelLoseHealthPatch
    {
        public static bool Prefix(PlayerState __instance, float health) => !infHealth;
    }

}

