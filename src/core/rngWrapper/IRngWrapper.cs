using System;

namespace NRUSharp.core.rngWrapper{
    public interface IRngWrapper{
        public Random Rng{ get; }
        public void Init(int seed = 0);
        public int GetInt(int start, int end);
    }
}