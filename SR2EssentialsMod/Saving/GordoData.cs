using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Saving
{

    [Serializable]
    public struct SR2EGordoData
    {
        public float baseSize;

        public SR2EGordoData()
        {
            baseSize = 4f;
        }
    }
}
