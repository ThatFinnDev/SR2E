using System;

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
