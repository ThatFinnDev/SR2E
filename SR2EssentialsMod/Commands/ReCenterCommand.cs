namespace SR2E.Commands
{
    public class ReCenterCommand : SR2Command
    {
        public override string ID => "recentercamera";
        public override string Usage => "recentercamera";
        public override bool Execute(string[] args)
        {
            if (!args.IsBetween(0,0)) return SendNoArguments();

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 10000f))
            {
                Camera.main.transform.LookAt(hit.transform);
                
                SendMessage(translation("cmd.recentercamera.success"));
                return true;
            }
            SendError(translation("cmd.recentercamera.needtolook"));
            return false;
        }
    }
}