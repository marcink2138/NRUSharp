using System;
using System.ComponentModel;
using MersenneTwister;
using NRUSharp.core.interfaces;

namespace NRUSharp.core.rngWrapper.impl{
    public class RngWrapper : IRngWrapper{
        private Random _rng;

        public void Init(int seed = -1){
            if (seed == -1){
                _rng = Randoms.Create(RandomType.FastestInt32);
                return;
            }

            _rng = Randoms.Create(seed, RandomType.FastestInt32);
        }

        public int GetInt(int start, int end){
            if (_rng == null){
                throw new WarningException("RngWrapper is not initialized");
            }

            return _rng.Next(start, end);
        }
    }
}