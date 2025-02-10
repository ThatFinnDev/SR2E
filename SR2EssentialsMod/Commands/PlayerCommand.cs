using System;
using System.Linq;
using Il2CppKinematicCharacterController;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.Player.PlayerItems;
using SR2E.Components;
using SR2E.Enums;

namespace SR2E.Commands;

internal class PlayerCommand : SR2ECommand
{
    public override string ID => "player";
    public override string Usage => "player <action> [value]";
    public override CommandType type => CommandType.Cheat | CommandType.Fun;

    List<string> arg0List = new List<string> { "size", "gravity", "vacmode"};
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        if (argIndex == 0) return arg0List;
        if (argIndex == 1)
            switch (args[0])
            {
                case "size": return new List<string> { "0.25", "0.5", "1", "1.5", "2", "5" };
                case "gravity": return new List<string> { "-2", "-1", "1", "2", "5" };
                case "vacmode": return Enum.GetNames(typeof(VacModes)).ToList();
            }
        return null;
    }
    const float playerColliderHeightBase = 2f;
    const float playerColliderRadBase = 0.6f;
    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(1,2)) return SendUsage();
        if (!inGame) return SendLoadASaveFirst();
        if (!arg0List.Contains(args[0])) return SendNotValidOption(args[0]);
         
        switch (args[0])
        {
            case "size":
                if (args.Length == 1) { SendMessage(translation("cmd.player.size.show",SceneContext.Instance.player.transform.localScale.x)); return true; }
                float size;
                if (!this.TryParseFloat(args[1], out size, 0, false)) return false;
                KinematicCharacterMotor KCC = null;
                try { KCC = SceneContext.Instance.player.GetComponent<KinematicCharacterMotor>(); }
                catch { return SendNullKinematicCharacterMotor();}
                SceneContext.Instance.player.transform.localScale = Vector3.one * size;
                KCC.CapsuleHeight = playerColliderHeightBase * size;
                KCC.CapsuleRadius = playerColliderRadBase * size;
                SendMessage(translation("cmd.player.size.edit",size));
                return true;
            case "gravity":
                SRCharacterController SRCC = null;
                try { SRCC = SceneContext.Instance.player.GetComponent<SRCharacterController>(); }
                catch { return SendNullSRCharacterController();}
                if (args.Length == 1) { SendMessage(translation("cmd.player.gravity.show",SRCC._gravityMagnitude.Value)); return true; }
                float level;
                if (!this.TryParseFloat(args[1], out level)) return false;
                SRCC._gravityMagnitude = new Il2CppSystem.Nullable<float>(level);
                SendMessage(translation("cmd.gravity.edit",level));
                return true;
            case "vacmode":
                if (args.Length == 1) { SendMessage(translation("cmd.player.vacmode.show",currVacMode.ToString().Replace("VacModes",""))); return true; }
                VacModes mode;
                try { mode = Enum.Parse<VacModes>(args[1]); }
                catch { return SendNotValidVacMode(args[1]); }
                SceneContext.Instance.Camera.RemoveComponent<FlingMode>();
                SceneContext.Instance.Camera.RemoveComponent<IdentifiableObjectDragger>();
                var vacuumItem = SceneContext.Instance.PlayerState.VacuumItem;
                vacuumItem.gameObject.SetActive(true);
                switch (mode)
                {
                    case VacModes.NONE: vacuumItem._vacMode = VacuumItem.VacMode.NONE; break;
                    case VacModes.AUTO_VAC: vacuumItem._vacMode = VacuumItem.VacMode.VAC;; break;
                    case VacModes.AUTO_SHOOT: vacuumItem._vacMode = VacuumItem.VacMode.SHOOT; break;
                    /*case VacModes.DRAG: 
                        vacuumItem._vacMode = VacuumItem.VacMode.NONE;
                        ExecuteInSeconds((() => { vacuumItem.gameObject.SetActive(false); SceneContext.Instance.Camera.AddComponent<IdentifiableObjectDragger>(); }), 1.5f); break;
                    case VacModes.LAUNCH: 
                        vacuumItem._vacMode = VacuumItem.VacMode.NONE; 
                        ExecuteInSeconds((() => { vacuumItem.gameObject.SetActive(false); SceneContext.Instance.Camera.AddComponent<FlingMode>(); }), 1.5f);break;*/
                }
                currVacMode = mode;
                SendMessage(translation("cmd.player.vacmode.success",mode.ToString().Replace("VacModes","")));
                return true;
        }
        return SendUnknown();
    }
    static VacModes currVacMode;
    public override void OnPlayerCoreLoad()
    {
        switch (SceneContext.Instance.PlayerState.VacuumItem._vacMode)
        {
            case VacuumItem.VacMode.NONE:
                currVacMode = VacModes.NONE;
                break;
            case VacuumItem.VacMode.SHOOT:
                currVacMode = VacModes.AUTO_SHOOT;
                break;
            case VacuumItem.VacMode.VAC:
                currVacMode = VacModes.AUTO_VAC;
                break;
        }
        
    }
}