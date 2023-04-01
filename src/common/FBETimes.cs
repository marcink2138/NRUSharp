namespace NRUSharp.common{
    public struct FBETimes{
        public int Cca;
        public int Cot;
        public int Ffp;
        public int IdleTime;

        public FBETimes(int cca, int cot, int ffp){
            Cca = cca;
            Cot = cot;
            Ffp = ffp;
            IdleTime = ffp - cot;
        }
    }
}