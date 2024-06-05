using Il2CppMonomiPark.SlimeRancher;

namespace SR2E.Commands;

public class FreezeCommand : SR2Command
{
    public override string ID => "freeze";
    public override string Usage => "freeze";

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0,0)) return SendNoArguments();
        if (!inGame) return SendLoadASaveFirst();

        Camera cam = Camera.main;
        if (cam == null) return SendError(translation("cmd.error.nocamera"));
            

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
                    SendMessage(translation("cmd.freeze.successfroze"));
                }
                else
                {
                    ident.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    ident.GetComponent<Vacuumable>().enabled = true;
                    if (ident.transform.getObjRec<Animator>("Appearance"))
                        ident.transform.getObjRec<Animator>("Appearance").enabled = true;
                    SendMessage(translation("cmd.freeze.successunfroze"));
                    return true;
                }
            }
        }

        return SendError(translation("cmd.error.notlookingatvalidobject"));
    }
}

