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
    internal static ColorAdjustments myAdjustments = null;
    public override bool Execute(string[] args)
    {
        if (args != null)
        { SR2Console.SendError($"The '<color=white>{ID}</color>' command takes no arguments"); return false; }
        if (defaultVolume != null)
        {
            if (myAdjustments != null)
            { 
                myAdjustments.hueShift.value = 0;
                myAdjustments.hueShift.overrideState = false;
                defaultVolume = null;
                SR2Console.SendMessage("Successfully disabled party mode!"); 
                return true;
            }
        }

        Volume volume = Get<Volume>("Default Volume");
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
                if (myAdjustments == null)
                {
                    if (defaultVolume.profile.Has<ColorAdjustments>())
                        defaultVolume.profile.Remove<ColorAdjustments>();
                    myAdjustments = defaultVolume.profile.Add<ColorAdjustments>();
                }
                
                float currentValue = myAdjustments.hueShift.GetValue<float>();
                currentValue += 80f * Time.deltaTime;
                if (currentValue > 180)
                    currentValue = -180;
                myAdjustments.hueShift.overrideState = true;
                myAdjustments.hueShift.m_Value = currentValue;
                myAdjustments.hueShift.value = currentValue;
            }
            catch {}
        }
    }
}