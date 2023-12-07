using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
namespace SR2E.Commands;
public class PartyCommand : SR2CCommand
{
    public override string ID => "party";
    public override string Usage => "party";
    public override string Description => "Enable Party Mode";
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        return null;
    }
    internal static Volume defaultVolume = null;
    public override bool Execute(string[] args)
    {
        if (args != null)
        { SR2Console.SendError($"The '<color=white>{ID}</color>' command takes no arguments"); return false; }
        if (defaultVolume != null)
        {
            if (defaultVolume.profile.Has<ColorAdjustments>())
                if (defaultVolume.profile.TryGet(out ColorAdjustments adjustments))
                {
                    adjustments.hueShift.value = 0;
                    adjustments.hueShift.overrideState = false;
                    defaultVolume = null;
                    SR2Console.SendMessage("Successfully disabled party mode!");
                    return true;
                }
        }

        Volume volume = SR2EUtils.Get<Volume>("Default Volume");
        if (volume != null)
            if (volume.isGlobal)
            {
                defaultVolume = volume;
                SR2Console.SendMessage("Successfully enabled party mode!");
                return true;
            }

        SR2Console.SendError("An unknown error occured!");
        return false;
    }

    public override void Update()
    {            
        if (defaultVolume != null)
        {
            try
            {
                ColorAdjustments adjustments = null;
                if (defaultVolume.profile.Has<ColorAdjustments>())
                {
                    defaultVolume.profile.TryGet(out adjustments);
                }
                else
                    adjustments = defaultVolume.profile.Add<ColorAdjustments>();
                    
                float currentValue = adjustments.hueShift.GetValue<float>();
                currentValue += 80f * Time.deltaTime;
                if (currentValue > 180)
                    currentValue = -180;
                adjustments.hueShift.overrideState = true;
                adjustments.hueShift.m_Value = currentValue;
                adjustments.hueShift.value = currentValue;
            }
            catch {}
        }
    }
}