﻿using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
namespace SR2E.Commands;
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

        SendError(translation("cmd.party.novolume"));
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