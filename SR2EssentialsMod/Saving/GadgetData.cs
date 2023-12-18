using System;
using Newtonsoft.Json;
namespace SR2E.Saving;

[Serializable]
public struct SR2EGadgetData
{
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public float scaleX = 1f;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public float scaleY = 1f;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public float scaleZ = 1f;


    public SR2EGadgetData()
    {
        scaleX = 1f;
        scaleY = 1f;
        scaleZ = 1f;
    }
}