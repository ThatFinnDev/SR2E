using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.UI;

namespace SR2E.Commands
{
    public class InfiniteHealthCommand : SR2Command
    {
        public override string ID => "infhealth";
        public override string Usage => "infhealth";
        public override string Description => "Makes you invincible";
        
        public override bool Execute(string[] args)
        { 
            if (args != null) return SendNoArguments();
            if (!inGame) return SendLoadASaveFirst();

            if (SR2EEntryPoint.infHealthInstalled)
            { SendError("You cannot toggle infinite health while having the infinite health mod installed!"); return false; }
            
            if (infHealth)
            {
                infHealth = false;
                healthMeter.gameObject.active = true;
                SendMessage("You're no longer invincible!");
            }
            else
            {
                infHealth = true;
                healthMeter.gameObject.active = false;
                SendMessage("You're now invincible!");
            }
            return true;
        }

        public override void OnMainMenuUILoad()
        {
            infHealth = false;
        }
        
        static bool infHealth = false;
        static HealthMeter _healthMeter;

        static HealthMeter healthMeter { get { if (_healthMeter == null) _healthMeter = Get<HealthMeter>("Health Meter"); return _healthMeter; } }
        
        [HarmonyPatch(typeof(PlayerModel), nameof(PlayerModel.LoseHealth))]
        public class PlayerModelLoseHealthPatch
        {
            public static bool Prefix(PlayerState __instance, float health) 
            { return !infHealth; }
        }
        
    }

}