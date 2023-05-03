﻿using NRUSharp.core.data;
using NRUSharp.core.interfaces;
using NRUSharp.core.stationImpl;

namespace NRUSharp.core.builder{
    public class FixedMutingFbeBuilder : AbstractFbeStationBuilder{
        protected int MutedPeriods;

        public FixedMutingFbeBuilder WithMutedPeriods(int mutedPeriods){
            MutedPeriods = mutedPeriods;
            return this;
        }

        public override IStation Build(bool reset = false){
            var fbeTimes = new FbeTimes(Cca, Cot, Ffp);
            var simulationParams = new SimulationParams{
                SimulationTime = SimulationTime,
                OffsetRangeTop = OffsetTop,
                OffsetRangeBottom = OffsetBottom
            };
            var station = new FixedMutingFbe(Name, Env, Channel, fbeTimes, RngWrapper, MutedPeriods,
                simulationParams);
            if (reset){
                Reset();
            }

            return station;
        }

        public override void Reset(){
            base.Reset();
            MutedPeriods = 0;
        }
    }
}