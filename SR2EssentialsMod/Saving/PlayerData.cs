using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
