using Il2Cpp;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.UI;
using UnityEngine;

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
            if (SceneContext.Instance == null) { SR2Console.SendError("Load a save first!"); return false; }
            if (SceneContext.Instance.PlayerState == null) { SR2Console.SendError("Load a save first!"); return false; }

            if (SR2EMain.infHealth)
            {
                SR2EMain.infHealth = false;
                if (healthMeter == null)
                    healthMeter = Get<HealthMeter>("Health Meter");
                healthMeter.gameObject.active = true;
                
                SceneContext.Instance.PlayerState._model.maxHealth = normalHealth;
                SceneContext.Instance.PlayerState.SetHealth(normalHealth); 
                SR2Console.SendMessage("You're no longer invincible!");
            }
            else
            {
                SR2EMain.infHealth = true;
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
        internal static int normalHealth = 100;
        
        internal static HealthMeter healthMeter;
        static T Get<T>(string name) where T : UnityEngine.Object
        {
            return Resources.FindObjectsOfTypeAll<T>().FirstOrDefault((T x) => x.name == name);
        }
    }
}