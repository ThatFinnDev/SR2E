﻿using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.UI;

namespace SR2E.Commands;

<<<<<<< HEAD
public class InfiniteHealthCommand : SR2Command
{
    public override string ID => "infhealth";
    public override string Usage => "infhealth";
=======
internal class InfiniteHealthCommand : SR2ECommand
{
    public override string ID => "infhealth";
    public override string Usage => "infhealth";
    public override CommandType type => CommandType.Cheat;
>>>>>>> experimental

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0,0)) return SendNoArguments();
        if (!inGame) return SendLoadASaveFirst();

<<<<<<< HEAD
        if (SR2EEntryPoint.infHealthInstalled) { SR2EConsole.SendError(translation("cmd.infhealth.dedicatedmodinstalled")); return false; }

        if (infHealth)
        {
            infHealth = false;
            if (healthMeter == null)
                healthMeter = Get<HealthMeter>("Health Meter");
=======
        if (infHealth)
        {
            infHealth = false;
            if (healthMeter == null) healthMeter = Get<HealthMeter>("Health Meter");
>>>>>>> experimental
            healthMeter.gameObject.active = true;
            SendMessage(translation("cmd.infhealth.successnolonger"));
        }
        else
        {
            infHealth = true;;
<<<<<<< HEAD
            if (healthMeter == null)
                healthMeter = Get<HealthMeter>("Health Meter");
=======
            if (healthMeter == null) healthMeter = Get<HealthMeter>("Health Meter");
>>>>>>> experimental
            healthMeter.gameObject.active = false;
            SendMessage(translation("cmd.infhealth.success"));
        }

        return true;
    }

<<<<<<< HEAD
    public override void OnMainMenuUILoad()
    {
        infHealth = false;
    }

    public static bool infHealth = false;

    private static HealthMeter healthMeter;
    public override void OnUICoreLoad()
    {
        healthMeter = Get<HealthMeter>("Health Meter");
    }
    [HarmonyPatch(typeof(PlayerModel), nameof(PlayerModel.LoseHealth))]
    public class PlayerModelLoseHealthPatch
    {
        public static bool Prefix(PlayerState __instance, float health)
        {
            return !infHealth;
        }
=======
    public override void OnMainMenuUILoad() => infHealth = false;
    public static bool infHealth = false;
    private static HealthMeter healthMeter;
    public override void OnUICoreLoad() => healthMeter = Get<HealthMeter>("Health Meter");
    [HarmonyPatch(typeof(PlayerModel), nameof(PlayerModel.LoseHealth))]
    internal class PlayerModelLoseHealthPatch
    {
        public static bool Prefix(PlayerState __instance, float health) => !infHealth;
>>>>>>> experimental
    }

}

