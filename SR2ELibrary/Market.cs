namespace SR2E.Library
{
    internal struct ModdedMarketData
    {
        public readonly float SAT;
        public readonly float VAL;

        internal ModdedMarketData(float s, float v)
        {
            VAL = v;
            SAT = s;
        }
    }
}
