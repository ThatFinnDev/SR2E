﻿using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
namespace SR2E.Commands;
<<<<<<< HEAD
public class PartyCommand : SR2Command
{
    public override string ID => "party";
    public override string Usage => "party";
    public override List<string> GetAutoComplete(int argIndex, string[] args)
    {
        return null;
    }
    internal static Volume defaultVolume = null;
    internal static ColorAdjustments myAdjustments = null;
=======
internal class PartyCommand : SR2ECommand
{
    public override string ID => "party";
    public override string Usage => "party";
    public override CommandType type => CommandType.Fun;
    static Volume defaultVolume = null;
    static ColorAdjustments myAdjustments = null;
>>>>>>> experimental
    public override bool Execute(string[] args)
    {
        if (!args.IsBetween(0,0)) return SendNoArguments();
        if (defaultVolume != null)
        {
            if (myAdjustments != null)
            { 
                myAdjustments.hueShift.value = 0;
                myAdjustments.hueShift.overrideState = false;
                defaultVolume = null;
                SendMessage(translation("cmd.party.success2")); 
                return true;
            }
        }

        Volume volume = Get<Volume>("Default Volume");
        if (volume != null)
            if (volume.isGlobal)
            {
                defaultVolume = volume;
                SendMessage(translation("cmd.party.success"));
                return true;
            }

<<<<<<< HEAD
        SendError(translation("cmd.party.novolume"));
        return false;
=======
        return SendError(translation("cmd.party.novolume"));
>>>>>>> experimental
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