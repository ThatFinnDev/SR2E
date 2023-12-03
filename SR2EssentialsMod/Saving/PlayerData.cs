using System;

namespace SR2E.Saving
{

    [Serializable]
    public struct SR2EPlayerData
    {
        public bool noclipState;
        public float size;
        public float gravityLevel;
        public float speed;

        public SR2EPlayerData()
        {
            noclipState = false;
            size = 1;
            gravityLevel = 17;
            speed = 1;
        }
    }
}
