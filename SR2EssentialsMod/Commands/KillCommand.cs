using Il2CppMonomiPark.SlimeRancher.Damage;
using Il2CppMonomiPark.SlimeRancher.World;

namespace SR2E.Commands;

public class KillCommand : SR2Command
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
        if (args != null) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();
        
        GameObject gameObject = ShootRaycast();
        if (gameObject != null)
            if (Kill(gameObject))
            { SendMessage("Successfully killed the thing!"); return true; }
        
        SendError("Not looking at a valid object!");
        return false;
    }

    GameObject ShootRaycast()
    {
        GameObject obj = null;
        if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
            obj = hit.collider.gameObject;
        return obj;
    }

    bool Kill(GameObject gameObject)
    {
        bool didAThing = false;
        if (gameObject.GetComponentInParent<Gadget>())
        {
            gameObject.GetComponentInParent<Gadget>().DestroyGadget(null,null);;
            didAThing = true;
        }
        else if (gameObject.GetComponent<Identifiable>())
        {
            DeathHandler.Kill(gameObject, SR2EEntryPoint.killDamage);
            didAThing = true;
        }
        else if (gameObject.GetComponent<GordoEat>())
        {
            GordoEat gordoEat = gameObject.GetComponent<GordoEat>();
            if (gordoEat.isActiveAndEnabled && gordoEat.CanEat())
                try
                {
                    gordoEat.ImmediateReachedTarget();
                    didAThing = true;
                }
                catch
                {
                }

        }
        else if (gameObject.GetComponentInParent<LandPlot>())
        {
            gameObject.GetComponentInParent<LandPlotLocation>().Replace(gameObject.GetComponentInParent<LandPlot>(), GameContext.Instance.LookupDirector.GetPlotPrefab(LandPlot.Id.EMPTY));
            didAThing = true;
        }
        return didAThing;
    }

}
