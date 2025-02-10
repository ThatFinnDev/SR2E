using Il2CppMonomiPark.SlimeRancher.Damage;
using Il2CppMonomiPark.SlimeRancher.World;

namespace SR2E.Commands;

<<<<<<< HEAD
public class KillCommand : SR2Command
{
    public override string ID => "kill";
    public override string Usage => "kill";

    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        return null;
    }
=======
internal class KillCommand : SR2ECommand
{
    public override string ID => "kill";
    public override string Usage => "kill";
    public override CommandType type => CommandType.Cheat;
>>>>>>> experimental
    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0,0)) return SendNoArguments();
        if (!inGame) return SendLoadASaveFirst();
        
<<<<<<< HEAD
        Camera cam = Camera.main;
        if (cam == null) return SendError(translation("cmd.error.nocamera"));
        GameObject gameObject = null;
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit))
            gameObject = hit.collider.gameObject;
        else
            return SendError(translation("cmd.error.notlookingatanything"));
        if (gameObject != null)
            if (Kill(gameObject))
            { SendMessage(translation("cmd.kill.success")); return true; }
        
        return SendError(translation("cmd.error.notlookingatvalidobject"));
        
=======
        Camera cam = Camera.main; if (cam == null) return SendNoCamera();
        GameObject gameObject = null;
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit,Mathf.Infinity,defaultMask)) gameObject = hit.collider.gameObject;
        else return SendNotLookingAtAnything();
        if (gameObject != null)
            if (Kill(gameObject)) { SendMessage(translation("cmd.kill.success")); return true; }

        return SendNotLookingAtValidObject();

>>>>>>> experimental
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
<<<<<<< HEAD
            DeathHandler.Kill(gameObject, SR2EEntryPoint.killDamage);
=======
            DeathHandler.Kill(gameObject, killDamage);
>>>>>>> experimental
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
<<<<<<< HEAD
                }
                catch
                {
                }
=======
                }catch { }
>>>>>>> experimental

        }
        else if (gameObject.GetComponentInParent<LandPlot>())
        {
            gameObject.GetComponentInParent<LandPlotLocation>().Replace(gameObject.GetComponentInParent<LandPlot>(), GameContext.Instance.LookupDirector.GetPlotPrefab(LandPlot.Id.EMPTY));
            didAThing = true;
        }
        return didAThing;
    }

}
