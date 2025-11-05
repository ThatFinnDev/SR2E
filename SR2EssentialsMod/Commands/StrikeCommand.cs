using Il2CppMonomiPark.SlimeRancher.World;

namespace SR2E.Commands;

internal class StrikeCommand : SR2ECommand
{
    public override string ID => "strike";
    public override string Usage => "strike [power]";
    public override CommandType type => CommandType.Fun | CommandType.Cheat;
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0)
            return new List<string> { "1", "1.5", "2" };
        return null;
    }
    static GameObject lightningPrefab;
    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0,1)) return SendUsage();
        Camera cam = MiscEUtil.GetActiveCamera(); if (cam == null) return SendNoCamera();
        
        if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out var hit,Mathf.Infinity,MiscEUtil.defaultMask))
        {
            var newPrefab = Object.Instantiate(lightningPrefab);
            newPrefab.transform.position = hit.point;
            if (args != null && args.Length == 1)
            {
                try { newPrefab.GetComponent<LightningStrike>().BlastPower = float.Parse(args[0]) * 2750f; }
                catch { return SendNotValidInt(args[0]); }
            }
            SendMessage(translation("cmd.strike.success"));
            return true;
        }
        return SendNotLookingAtAnything();
    }
    public override void OnGameContext(GameContext gameContext)
    {
        lightningPrefab = Object.Instantiate(Get<GameObject>("LightningStrike"));
        lightningPrefab.MakePrefab();
        lightningPrefab.name = "InstantLightning";
        var l = lightningPrefab.GetComponent<LightningStrike>();
        l.WarningTime = 0.5f;
        l.SpawnOptions.RemoveAt(0);
        l.SpawnOptions.RemoveAt(0);
        l._strikeTime = 8f;
        l.BlastPower = 2750f;
        l.BlastRadius = 9f;
    }
}

