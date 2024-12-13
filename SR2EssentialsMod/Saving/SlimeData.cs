/*using System;
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
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public Vector3Data velocity = new Vector3Data(0,0,0);


    public SR2ESlimeData()
    {
        scaleX = 1f;
        scaleY = 1f;
        scaleZ = 1f;
        zeroGrav = false;
        velocity = new Vector3Data(0, 0, 0);
    }
}*/
//Broken as of SR2 0.6.0