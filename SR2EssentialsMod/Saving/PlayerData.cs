using Newtonsoft.Json;
using System;

namespace SR2E.Saving;

[Serializable]
public struct SR2EPlayerData
{
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool noclipState = false;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public float size = 1;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public float gravityLevel = 17;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public float speed = 1;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public VacModes vacMode = VacModes.NORMAL;
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public Vector3Data velocity = new Vector3Data(0, 0, 0);

    public SR2EPlayerData()
    {
        noclipState = false;
        size = 1;
        gravityLevel = 17;
        speed = 1;
        vacMode = VacModes.NORMAL;
        velocity = new Vector3Data(0, 0, 0);
    }
}
