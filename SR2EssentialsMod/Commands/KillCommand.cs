using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.Damage;
using UnityEngine;

namespace SR2E.Commands
{
    public class KillCommand : SR2CCommand
    {
        public override string ID => "kill";
        public override string Usage => "kill";
        public override string Description => "Kills what you're looking at";
        public override bool Execute(string[] args)
        {
            if (SceneContext.Instance == null) { SR2Console.SendError("Load a save first!"); return false; }
            if (SceneContext.Instance.PlayerState == null) { SR2Console.SendError("Load a save first!"); return false; }

            if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
            {
                var gameobject = hit.collider.gameObject;
                if (gameobject.GetComponent<Identifiable>())
                {
                    Damage damage = new Damage { DamageSource = ScriptableObject.CreateInstance<DamageSourceDefinition>() };;
                    damage.DamageSource.hideFlags |= HideFlags.HideAndDontSave;
                    damage.Amount = 99999999;
                    DeathHandler.Kill(gameobject, damage);
                    
                }
                else if (gameobject.GetComponentInParent<Gadget>())
                {
                    gameobject.GetComponentInParent<Gadget>().RequestDestroy("KillCommand.Execute");
                }
                else if (gameobject.GetComponentInParent<LandPlot>())
                {
                    gameobject.GetComponentInParent<LandPlotLocation>().Replace(gameobject.GetComponentInParent<LandPlot>(), GameContext.Instance.LookupDirector.GetPlotPrefab(LandPlot.Id.EMPTY));
                }
                SR2Console.SendMessage("Successfully killed the thing!");
                return true;
            }
            SR2Console.SendError("Not looking at a valid object!");
            return false;
        }
    }
}