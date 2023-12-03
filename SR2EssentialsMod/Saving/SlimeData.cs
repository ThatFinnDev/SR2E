using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Saving
{
    [Serializable]
    public struct SR2ESlimeData
    {
        public float scaleX;
        public float scaleY;
        public float scaleZ;

        public SR2ESlimeData()
        {
            scaleX = 1f;
            scaleY = 1f;
            scaleZ = 1f;
        }
    }
}
