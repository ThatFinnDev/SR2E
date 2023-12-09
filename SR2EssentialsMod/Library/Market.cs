namespace CottonLibrary
{
    internal struct CottonMarketData
    {
        public readonly float SAT;
        public readonly float VAL;

        internal CottonMarketData(float s, float v)
        {
            VAL = v;
            SAT = s;
        }
    }
}
