using Il2CppMonomiPark.SlimeRancher.Damage;
using Il2CppMonomiPark.SlimeRancher.World;

namespace SR2E.Commands;

internal class KillCommand : SR2ECommand
{
    public override string ID => "kill";
    public override string Usage => "kill";
    public override CommandType type => CommandType.Cheat;
    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0,0)) return SendNoArguments();
        if (!inGame) return SendLoadASaveFirst();
        
        Camera cam = MiscEUtil.GetActiveCamera(); if (cam == null) return SendNoCamera();
        GameObject gameObject = null;
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit,Mathf.Infinity,MiscEUtil.defaultMask)) gameObject = hit.collider.gameObject;
        else return SendNotLookingAtAnything();
        if (gameObject != null)
            if (Kill(gameObject)) { SendMessage(translation("cmd.kill.success")); return true; }

        return SendNotLookingAtValidObject();

    }

    bool Kill(GameObject gameObject)
    {
        bool didAThing = false;
        if (gameObject.GetComponentInParent<Gadget>())
        {
            gameObject.GetComponentInParent<Gadget>().DestroyGadget();;
            didAThing = true;
        }
        else if (gameObject.GetComponent<Identifiable>())
        {
            DeathHandler.Kill(gameObject, killDamage);
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
                }catch { }

        }
        else if (gameObject.GetComponentInParent<LandPlot>())
        {
            gameObject.GetComponentInParent<LandPlotLocation>().Replace(gameObject.GetComponentInParent<LandPlot>(), gameContext.LookupDirector.GetPlotPrefab(LandPlot.Id.EMPTY));
            didAThing = true;
        }
        return didAThing;
    }

}
