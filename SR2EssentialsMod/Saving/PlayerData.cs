using System;

namespace SR2E.Saving;

[Serializable]
public struct SR2EPlayerData
{
    public bool noclipState = false;
    public float size = 1;
    public float gravityLevel = 17;
    public float speed = 1;
    public VacModes vacMode = VacModes.NORMAL;

    public SR2EPlayerData()
    {
        noclipState = false;
        size = 1;
        gravityLevel = 17;
        speed = 1;
        vacMode = VacModes.NORMAL;
    }
}
