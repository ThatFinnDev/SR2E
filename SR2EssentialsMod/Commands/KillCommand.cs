using Il2CppMonomiPark.SlimeRancher.Damage;

namespace SR2E.Commands
{
    public class KillCommand : SR2CCommand
    {
        public override string ID => "kill";
        public override string Usage => "kill";
        public override string Description => "Kills what you're looking at";
        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            return null;
        }
        public override bool Execute(string[] args)
        {
            if (SceneContext.Instance == null) { SR2Console.SendError("Load a save first!"); return false; }
            if (SceneContext.Instance.PlayerState == null) { SR2Console.SendError("Load a save first!"); return false; }

            if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
            {
                bool didAThing = false;
                var gameobject = hit.collider.gameObject;
                if (gameobject.GetComponent<Identifiable>())
                {
                    Damage damage = new Damage { Amount = 99999999,DamageSource = ScriptableObject.CreateInstance<DamageSourceDefinition>() };;
                    damage.DamageSource.hideFlags |= HideFlags.HideAndDontSave;
                    damage.Amount = 99999999;
                    DeathHandler.Kill(gameobject, damage);
                    didAThing = true;
                }
                else if (gameobject.GetComponentInParent<Gadget>())
                {
                    gameobject.transform.position=new Vector3(-100000,-100000,-100000);
                    gameobject.GetComponentInParent<Gadget>().gameObject.hideFlags |= HideFlags.HideAndDontSave;
                    //GameContext.Instance.AutoSaveDirector.SavedGame.Pull(gameobject.GetComponentInParent<Gadget>().Model);
                    //gameobject.GetComponentInParent<Identifiable>().
                    //SRBehaviour.RequestDestroy(gameobject.transform.parent.gameObject,"KillCommand.Execute");
                    //gameobject.GetComponentInParent<Gadget>().GetDestroyRequestHandler().OnDestroyRequest(gameobject.GetComponentInParent<Gadget>().gameObject);
                    gameobject.GetComponentInParent<Gadget>().RequestDestroy("ok");
                    didAThing = true;
                }
                else if (gameobject.GetComponentInParent<LandPlot>())
                {
                    gameobject.GetComponentInParent<LandPlotLocation>().Replace(gameobject.GetComponentInParent<LandPlot>(), GameContext.Instance.LookupDirector.GetPlotPrefab(LandPlot.Id.EMPTY));
                    didAThing = true;
                }
                if(didAThing)
                { SR2Console.SendMessage("Successfully killed the thing!"); return true; }
            }
            SR2Console.SendError("Not looking at a valid object!");
            return false;
        }
    }
}