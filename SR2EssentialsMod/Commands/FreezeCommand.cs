using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Commands
{
    internal class FreezeCommand : SR2CCommand
    {
        public override string ID => "freeze";

        public override string Usage => "freeze";

        public override string Description => "Freezes a object in time";

        public override string ExtendedDescription => "Freezes a identifiable in time, cannot be moved";

        public override bool Execute(string[] args)
        {
            if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
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
                    }
                    else
                    {
                        ident.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                        ident.GetComponent<Vacuumable>().enabled = true;
                        if (ident.transform.getObjRec<Animator>("Appearance"))
                            ident.transform.getObjRec<Animator>("Appearance").enabled = true;
                    }
                }
            }
            return true;
        }
    }
}
