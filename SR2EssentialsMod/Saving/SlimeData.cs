using System;
using Newtonsoft.Json;
namespace SR2E.Saving;

[Serializable]
public struct SR2ESlimeData
{
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public float scaleX = 1f;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public float scaleY = 1f;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public float scaleZ = 1f;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool zeroGrav = false;
    public SR2ESlimeData()
    {
        scaleX = 1f;
        scaleY = 1f;
        scaleZ = 1f;
        zeroGrav = false;
    }
}
