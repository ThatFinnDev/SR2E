using System.Linq;
using Il2Cpp;
using UnityEngine;

namespace SR2E.Commands
{
    public class InfiniteEnergyCommand: SR2CCommand
    {
        public override string ID { get; } = "infenergy";
        public override string Usage { get; } = "infenergy";
        public override string Description { get; } = "Removes energy from the game";
        
        public override bool Execute(string[] args)
        {
            if (args != null)
            {
                SR2Console.SendError($"The '<color=white>{ID}</color>' command takes no arguments");
                return false;
            }
                
            
                
            if (SceneContext.Instance == null)
            { SR2Console.SendError("Load a save first!"); return false; }
                
            if (SceneContext.Instance.PlayerState == null) 
            { SR2Console.SendError("Load a save first!"); return false; }

            if (SR2EMain.infEnergy)
            {
                SR2EMain.infEnergy = false;
                Get<GameObject>("Energy Meter").active = true;
                SceneContext.Instance.PlayerState.SetEnergy(0);
                SceneContext.Instance.PlayerState._model.maxEnergy.CachedValue = normalEnergy;
                SR2Console.SendMessage("Energy is no longer infinite");
            }
            else
            {
                SR2EMain.infEnergy = true;
                Get<GameObject>("Energy Meter").active = false;
                SceneContext.Instance.PlayerState.SetEnergy(int.MaxValue);
                normalEnergy = SceneContext.Instance.PlayerState._model.maxEnergy.CachedValue;
                SceneContext.Instance.PlayerState._model.maxEnergy.CachedValue = 2.14748365E+09f;
                SR2Console.SendMessage("Energy is now infinite");
            }

           
            return true;
        }

        static float normalEnergy = 100;
        static T Get<T>(string name) where T : UnityEngine.Object
        {
            return Resources.FindObjectsOfTypeAll<T>().FirstOrDefault((T x) => x.name == name);
        }
    }
}