using System;
using NRUSharp.core;
using NRUSharp.core.interfaces;
using NRUSharp.core.stationImpl;

namespace NRUSharp.simulationFramework{
    public class StationFactory{
        public IStation CreateStation(StationType stationType){
            switch (stationType){
                case StationType.StandardFbe:
                    return new StandardFbe();
                case StationType.FixedMutingFbe:
                    return new FixedMutingFbe();
                case StationType.RandomMutingFbe:
                    return new RandomMutingFbe();
                case StationType.FloatingFbe:
                    return new FloatingFbe();
                case StationType.DeterministicBackoffFbe:
                    return new DeterministicBackoffFbe();
                case StationType.GreedyEnhancedFbe:
                    return new GreedyEnhancedFbe();
                case StationType.EnhancedFbe:
                    return new EnhancedFbe(false);
                case StationType.BitrFbe:
                    return new EnhancedFbe(true);
                default:
                    throw new ArgumentOutOfRangeException(nameof(stationType), stationType, null);
            }
        }
    }
}