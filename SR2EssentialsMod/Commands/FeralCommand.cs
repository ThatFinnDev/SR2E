/*using Il2CppMonomiPark.SlimeRancher;

namespace SR2E.Commands;

internal class FeralCommand : SR2ECommand
{
    public override string ID => "feral";
    public override string Usage => "feral";
    public override CommandType type => CommandType.Miscellaneous | CommandType.Cheat;

    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0,0)) return SendNoArguments();
        if (!inGame) return SendLoadASaveFirst();

        Camera cam = MiscEUtil.GetActiveCamera(); if (cam == null) return SendNoCamera();
            

        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit,Mathf.Infinity,MiscEUtil.defaultMask))
        {
            var feral = hit.transform.GetComponent<SlimeFeral>();
            if (feral!=null)
            {
                var obj = hit.transform.gameObject;
                bool isFeral = feral.IsFeral();

                if (isFeral)
                {
                    feral.MakeNotFeral(true);
                    SendMessage(translation("cmd.feral.successoff"));
                }
                else
                {
                    feral.SetFeral();
                    SendMessage(translation("cmd.feral.successon"));
                }
                
                return true;

            }
            return SendNotLookingAtValidObject();
        }
        return SendNotLookingAtAnything();
    }
}*/