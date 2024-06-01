using Il2CppMonomiPark.SlimeRancher;

namespace SR2E.Commands;

public class FreezeCommand : SR2Command
{
    public override string ID => "freeze";

    public override string Usage => "freeze";

    public override string Description => "Freezes a object in time";

    public override string ExtendedDescription => "Freezes a identifiable in time, cannot be moved";

    public override bool Execute(string[] args)
    {
        if (!inGame) return SendLoadASaveFirst();
        if (args != null) return SendUsage();

        Camera cam = Camera.main;
        if (cam == null)
        {
            SendError("Couldn't find player camera!");
            return false;
        }

        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit))
        {
            var ident = hit.transform.GetComponent<IdentifiableActor>();
            if (ident)
            {
                if (ident.GetComponent<Rigidbody>().constraints != RigidbodyConstraints.FreezeAll)
                {
                    ident.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    ident.GetComponent<Vacuumable>().enabled = false;
                    if (ident.transform.getObjRec<Animator>("Appearance"))
                        ident.transform.getObjRec<Animator>("Appearance").enabled = false;
                    SendMessage("Successfully froze the actor!");
                }
                else
                {
                    ident.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    ident.GetComponent<Vacuumable>().enabled = true;
                    if (ident.transform.getObjRec<Animator>("Appearance"))
                        ident.transform.getObjRec<Animator>("Appearance").enabled = true;
                    SendMessage("Successfully unfroze the actor!");
                    return true;
                }
            }
        }

        SendWarning("Not looking at a valid object!");
        return false;
    }
}

