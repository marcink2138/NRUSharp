namespace NRUSharp.core.data{
    public struct FbeTimes{
        public readonly int Cca;
        public readonly int Cot;
        public readonly int Ffp;
        public readonly int IdleTime;

        public FbeTimes(int cca, int cot, int ffp){
            Cca = cca;
            Cot = cot;
            Ffp = ffp;
            IdleTime = ffp - cot;
        }
    }
}