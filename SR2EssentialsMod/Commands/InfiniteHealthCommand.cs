using Il2CppMonomiPark.SlimeRancher.UI;

namespace SR2E.Commands
{
    public class InfiniteHealthCommand : SR2CCommand
    {
        public override string ID => "infhealth";
        public override string Usage => "infhealth";
        public override string Description => "Makes you invincible";
        
        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            return null;
        }
        public override bool Execute(string[] args)
        { 
            return Code(args, false);
        }
        public override bool SilentExecute(string[] args)
        {
            Code(args, true);
            return true;
        }

        public bool Code(string[] args, bool silent)
        {
            if (args != null) return SendUsage();
            if (!inGame) return SendLoadASaveFirst();

            if (infHealth)
            {
                infHealth = false;
                if (healthMeter == null)
                    healthMeter = Get<HealthMeter>("Health Meter");
                healthMeter.gameObject.active = true;
                
                SceneContext.Instance.PlayerState._model.maxHealth = normalHealth;
                SceneContext.Instance.PlayerState.SetHealth(normalHealth); 
                if(!silent) SR2EConsole.SendMessage("You're no longer invincible!");
            }
            else
            {
                infHealth = true;
                if (healthMeter == null)
                    healthMeter = Get<HealthMeter>("Health Meter");
                healthMeter.gameObject.active = false;
                
                
                normalHealth = SceneContext.Instance.PlayerState._model.maxHealth;
                
                SceneContext.Instance.PlayerState.SetHealth(int.MaxValue); 
                SceneContext.Instance.PlayerState._model.maxHealth = int.MaxValue;
                if(!silent) SR2EConsole.SendMessage("You're now invincible!");
            }
            return true;
        }

        public override void OnMainMenuUILoad()
        {
            infHealth = false;
        }

        public override void Update()
        {
            if(infHealth)
                if (SceneContext.Instance != null)
                    if (SceneContext.Instance.PlayerState != null)
                        SceneContext.Instance.PlayerState.SetHealth(int.MaxValue);
        }

        public static int normalHealth = 100;
        static bool infHealth = false;
        static HealthMeter healthMeter;
        
        [HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.SaveGameAndFlush))]
        public static class InvincibleCorrectionPatch
        {
            public static void Prefix(AutoSaveDirector __instance)
            {
                if(infHealth)
                    if (SceneContext.Instance != null)
                        if (SceneContext.Instance.PlayerState != null)
                        {
                            
                            infHealth = false;
                            if (healthMeter == null)
                                healthMeter = Get<HealthMeter>("Health Meter");
                            healthMeter.gameObject.active = true;
                
                            SceneContext.Instance.PlayerState._model.maxHealth = normalHealth;
                            SceneContext.Instance.PlayerState.SetHealth(normalHealth); 
                        }
            }
        }
    }

}