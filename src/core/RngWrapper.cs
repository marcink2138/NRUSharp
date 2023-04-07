using System;
using System.ComponentModel;
using MersenneTwister;
using NRUSharp.common.interfaces;

namespace NRUSharp.common{
    public class RngWrapper : IRngWrapper{
        private Random _rng;

        public void Init(int seed = 0){
            if (_rng != null){
                throw new WarningException("RngWrapper is already initialized");
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