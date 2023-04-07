using System;
using System.Collections.Generic;
using NRUSharp.common;
using NRUSharp.common.data;
using NRUSharp.impl;
using SimSharp;
using Xunit;
using Xunit.Abstractions;

namespace NRUSharp.tests{
    public class FixedMutingFbeTests{
        private readonly ITestOutputHelper _output;

        public FixedMutingFbeTests(ITestOutputHelper output){
            _output = output;
        }

        [Fact]
        public void FloatingCotFixedFfpTest(){
            var ffp = 5000;
            var cotArray = new[]{
                250, 750, 1250, 1750, 2250, 2750, 3250, 3750, 4250, 4750
            };
            var results = new List<StationResults>();
            foreach (var cot in cotArray){
                var simulation = new Simulation(defaultStep: TimeSpan.FromSeconds(1));
                var fbeTimes = new FBETimes(9, cot, ffp);
                var channel = new Channel();
                var rngWrapper = new RngWrapper();
                rngWrapper.Init(55555);
                var station = new FixedMutingFbe("FLOATING FBE", simulation, channel, fbeTimes, 0, rngWrapper, 1);
                simulation.Process(station.Start());
                simulation.Run(TimeSpan.FromSeconds(1_000_000));
                results.Add(station.Results);
            }

            foreach (var result in results){
                _output.WriteLine("AirTime: {0}, SuccTrans: {1}, FailedTrans: {2}", result.AirTime,
                    result.SuccessfulTransmissions, result.FailedTransmissions);
            }
        }

        [Fact]
        public void WithOffset(){
            var ffp = 10000;
            var cotArray = new[]{
                1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000
            };
            var results = new List<StationResults>();
            var rngWrapper = new RngWrapper();
            rngWrapper.Init(55555);
            foreach (var cot in cotArray){
                var simulation = new Simulation(defaultStep: TimeSpan.FromSeconds(1));
                var fbeTimes = new FBETimes(9, cot, ffp);
                var channel = new Channel();
                var station1 = new FixedMutingFbe("STANDARD FBE 1", simulation, channel, fbeTimes, 0, rngWrapper, 1);
                var station2 = new FixedMutingFbe("STANDARD FBE 1", simulation, channel, fbeTimes, 2500, rngWrapper, 1);
                var station3 = new FixedMutingFbe("STANDARD FBE 1", simulation, channel, fbeTimes, 5000, rngWrapper, 1);
                var station4 = new FixedMutingFbe("STANDARD FBE 1", simulation, channel, fbeTimes, 7500, rngWrapper, 1);
                simulation.Process(station1.Start());
                simulation.Process(station2.Start());
                simulation.Process(station3.Start());
                simulation.Process(station4.Start());
                simulation.Run(TimeSpan.FromSeconds(1_000_000));
                results.Add(station1.Results);
                results.Add(station2.Results);
                results.Add(station3.Results);
                results.Add(station4.Results);
            }

            foreach (var result in results){
                _output.WriteLine("AirTime: {0}, SuccTrans: {1}, FailedTrans{2}", result.AirTime,
                    result.SuccessfulTransmissions, result.FailedTransmissions);
            }
        }
    }
}