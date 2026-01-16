using Il2CppMonomiPark.SlimeRancher;

namespace SR2E.Commands;

internal class FreezeCommand : SR2ECommand
{
    public override string ID => "freeze";
    public override string Usage => "freeze";
    public override CommandType type => CommandType.Fun | CommandType.Cheat;

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0,0)) return SendNoArguments();
        if (!inGame) return SendLoadASaveFirst();

        Camera cam = MiscEUtil.GetActiveCamera(); if (cam == null) return SendNoCamera();
            

        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit,Mathf.Infinity,MiscEUtil.defaultMask))
        {
            var ident = hit.transform.GetComponent<IdentifiableActor>();
            if (ident)
            {
                if (ident.GetComponent<Rigidbody>().constraints != RigidbodyConstraints.FreezeAll)
                {
                    ident.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    ident.GetComponent<Vacuumable>().enabled = false;
                    if (ident.transform.GetObjectRecursively<Animator>("Appearance"))
                        ident.transform.GetObjectRecursively<Animator>("Appearance").enabled = false;
                    SendMessage(translation("cmd.freeze.successfroze"));
                }
                else
                {
                    ident.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    ident.GetComponent<Vacuumable>().enabled = true;
                    if (ident.transform.GetObjectRecursively<Animator>("Appearance"))
                        ident.transform.GetObjectRecursively<Animator>("Appearance").enabled = true;
                    SendMessage(translation("cmd.freeze.successthaw"));
                }
                return true;
            }
            return SendNotLookingAtValidObject();
        }
        return SendNotLookingAtAnything();
    }
}

