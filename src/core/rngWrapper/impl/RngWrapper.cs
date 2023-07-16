using System;
using System.ComponentModel;
using MersenneTwister;

namespace NRUSharp.core.rngWrapper.impl{
    public class RngWrapper : IRngWrapper{
        public Random Rng{ get; private set; }

        public void Init(int seed = -1){
            if (seed == -1){
                Rng = Randoms.Create(RandomType.FastestInt32);
                return;
            }

            Rng = Randoms.Create(seed, RandomType.FastestInt32);
        }

        public int GetInt(int start, int end){
            if (Rng == null){
                throw new WarningException("RngWrapper is not initialized");
            }

            return Rng.Next(start, end);
        }
    }
}