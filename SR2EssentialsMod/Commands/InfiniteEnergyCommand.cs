using Il2CppMonomiPark.SlimeRancher.Player.CharacterController.Abilities;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppMonomiPark.UnitPropertySystem;

namespace SR2E.Commands;

public class InfiniteEnergyCommand : SR2Command
{
    public override string ID => "infenergy";
    public override string Usage => "infenergy [should disable height limit(true/false)]";
    public override string Description => "Removes energy from the game";

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return new List<string> { "true", "false" };
        return null;
    }

    public override bool Execute(string[] args)
    {
        if (args != null && args.Length != 1) return SendNoArguments();
        if (!inGame) return SendLoadASaveFirst();
        
        bool shouldDisableThrusterHeight = false;
        if (args != null)
            if (args.Length != 1)
            {
                SendMessage($"Usage: {Usage}");
                return false;
            }
            else
                shouldDisableThrusterHeight = (args[0].ToLower() == "true");


        if (infEnergy)
        {
            infEnergy = false;
            if (energyMeter == null)
                energyMeter = Get<EnergyMeter>("Energy Meter");
            energyMeter.gameObject.active = true;

            if (jetpackAbilityData == null)
                jetpackAbilityData = Get<JetpackAbilityData>("Jetpack");
            jetpackAbilityData._hoverHeight = normalHoverHeight;
            jetpackAbilityData._maxUpwardThrustForce = normalMaxUpwardThrustForce;
            jetpackAbilityData._upwardThrustForceIncrement = normalUpwardThrustForceIncrement;

            energyMeter.maxEnergy = new NullableFloatProperty(normalEnergy);
            SceneContext.Instance.PlayerState.SetEnergy(0);
            SendMessage("Energy is no longer infinite");
        }
        else
        {
            infEnergy = true;
            if (energyMeter == null)
                energyMeter = Get<EnergyMeter>("Energy Meter");
            energyMeter.gameObject.active = false;

            if (jetpackAbilityData == null)
                jetpackAbilityData = Get<JetpackAbilityData>("Jetpack");
            normalHoverHeight = jetpackAbilityData._hoverHeight;
            normalMaxUpwardThrustForce = jetpackAbilityData._maxUpwardThrustForce;
            normalUpwardThrustForceIncrement = jetpackAbilityData._upwardThrustForceIncrement;
            if (shouldDisableThrusterHeight)
            {
                jetpackAbilityData._hoverHeight = float.MaxValue;
                jetpackAbilityData._maxUpwardThrustForce = 5f;
                jetpackAbilityData._upwardThrustForceIncrement = 5f;
            }

            SceneContext.Instance.PlayerState.SetEnergy(int.MaxValue);
            normalEnergy = energyMeter.maxEnergy;
            energyMeter.maxEnergy = new NullableFloatProperty(2.14748365E+09f);
            SendMessage("Energy is now infinite");
        }


        return true;
    }

    public override void Update()
    {
        if (infEnergy)
            if (SceneContext.Instance != null)
                if (SceneContext.Instance.PlayerState != null)
                    SceneContext.Instance.PlayerState.SetEnergy(int.MaxValue);
    }

    public override void OnMainMenuUILoad()
    {
        infEnergy = false;
    }

    public override void OnPlayerCoreLoad()
    {
        jetpackAbilityData = Get<JetpackAbilityData>("Jetpack");
    }

    public override void OnUICoreLoad()
    {
        energyMeter = Get<EnergyMeter>("Energy Meter");
    }

    static bool infEnergy = false;
    static float normalEnergy = 100;
    static float normalHoverHeight = 0;
    static float normalMaxUpwardThrustForce = 0;
    static float normalUpwardThrustForceIncrement = 0;
    static EnergyMeter energyMeter;
    static JetpackAbilityData jetpackAbilityData;
}
