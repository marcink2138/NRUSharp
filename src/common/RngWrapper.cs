using System;
using System.ComponentModel;
using MersenneTwister;

namespace NRUSharp.common{
    public static class RngWrapper{
        private static Random _rng;

        public static void Init(int seed){
            if (_rng != null){
                throw new WarningException("RngWrapper is already initialized");
            }
            _rng = Randoms.Create(seed, RandomType.FastestInt32);
        }

        public static void InitRandom(){
            if (_rng != null){
                throw new WarningException("RngWrapper is already initialized");
            }
            _rng = Randoms.FastestInt32;
        }

        public static int GetInt(int start, int end){
            if (_rng == null){
                throw new WarningException("RngWrapper is not initialized");
            }
            return _rng.Next(start, end);
        }
        
    }
}