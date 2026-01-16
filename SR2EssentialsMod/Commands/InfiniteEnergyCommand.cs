using Il2CppMonomiPark.SlimeRancher.Player.CharacterController.Abilities;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.UnitPropertySystem;

namespace SR2E.Commands;

internal class InfiniteEnergyCommand : SR2ECommand
{
    public override string ID => "infenergy";
    public override string Usage => "infenergy [should disable height limit(true/false)]";
    public override CommandType type => CommandType.Cheat;

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0) return new List<string> { "true", "false" };
        return null;
    }

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0,1)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();

        bool shouldDisableThrusterHeight = false;
        if (args != null) if (!TryParseBool(args[0], out shouldDisableThrusterHeight)) return false;

        if (infEnergy)
        {
            infEnergy = false;
            if (energyMeter == null) energyMeter = GetInScene<EnergyMeter>("Energy Meter");
            energyMeter.transform.GetChild(0).gameObject.SetActive(true);

            if (jetpackAbilityData == null) jetpackAbilityData = Get<JetpackAbilityData>("Jetpack");
            jetpackAbilityData._hoverHeight = normalHoverHeight;
            jetpackAbilityData._maxUpwardThrustForce = normalMaxUpwardThrustForce;
            jetpackAbilityData._upwardThrustForceIncrement = normalUpwardThrustForceIncrement;

            energyMeter.maxEnergy = new NullableFloatProperty(normalEnergy);
            sceneContext.PlayerState.SetEnergy(0);
            SendMessage(translation("cmd.infenergy.successnolonger"));
        }
        else
        {
            infEnergy = true;
            if (energyMeter == null) energyMeter = GetInScene<EnergyMeter>("Energy Meter");
            energyMeter.transform.GetChild(0).gameObject.SetActive(false);

            if (jetpackAbilityData == null) jetpackAbilityData = Get<JetpackAbilityData>("Jetpack");
            normalHoverHeight = jetpackAbilityData._hoverHeight;
            normalMaxUpwardThrustForce = jetpackAbilityData._maxUpwardThrustForce;
            normalUpwardThrustForceIncrement = jetpackAbilityData._upwardThrustForceIncrement;
            if (shouldDisableThrusterHeight)
            {
                jetpackAbilityData._hoverHeight = float.MaxValue;
                jetpackAbilityData._maxUpwardThrustForce = 5f;
                jetpackAbilityData._upwardThrustForceIncrement = 5f;
            }

            try { sceneContext.PlayerState.SetEnergy(int.MaxValue); }catch { }
            normalEnergy = energyMeter.maxEnergy;
            energyMeter.maxEnergy = new NullableFloatProperty(2.14748365E+09f);
            SendMessage(translation("cmd.infenergy.success"));
        }
        return true;
    }

    public override void Update()
    {
        try
        {
            if (infEnergy)
                if (sceneContext != null)
                    if (sceneContext.PlayerState != null)
                        sceneContext.PlayerState.SetEnergy(int.MaxValue);
        }
        catch { }
    }

    public override void OnMainMenuUILoad() => infEnergy = false;
    public override void OnPlayerCoreLoad() => jetpackAbilityData = Get<JetpackAbilityData>("Jetpack");
    public override void OnUICoreLoad() => energyMeter = GetInScene<EnergyMeter>("Energy Meter");
    

    public static bool infEnergy = false;
    static float normalEnergy = 100;
    static float normalHoverHeight = 0;
    static float normalMaxUpwardThrustForce = 0;
    static float normalUpwardThrustForceIncrement = 0;
    static EnergyMeter energyMeter;
    static JetpackAbilityData jetpackAbilityData;
}
