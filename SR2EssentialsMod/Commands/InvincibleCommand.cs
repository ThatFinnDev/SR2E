using System.Linq;
using Il2CppMonomiPark.SlimeRancher.UI;

namespace SR2E.Commands
{
    public class InvincibleCommand : SR2CCommand
    {
        public override string ID => "invincible";
        public override string Usage => "invincible";
        public override string Description => "Makes you invincible";
        
        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            return null;
        }
        public override bool Execute(string[] args)
        {
            if (args != null)
            {
                SR2Console.SendError($"The '<color=white>{ID}</color>' command takes no arguments");
                return false;
            }
            if (!inGame) { SR2Console.SendError("Load a save first!"); return false; }

            if (infHealth)
            {
                infHealth = false;
                if (healthMeter == null)
                    healthMeter = Get<HealthMeter>("Health Meter");
                healthMeter.gameObject.active = true;
                
                SceneContext.Instance.PlayerState._model.maxHealth = normalHealth;
                SceneContext.Instance.PlayerState.SetHealth(normalHealth); 
                SR2Console.SendMessage("You're no longer invincible!");
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
                SR2Console.SendMessage("You're now invincible!");
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

        static int normalHealth = 100;
        static bool infHealth = false;
        static HealthMeter healthMeter;
        
    }
}