// using System;
// using System.Collections.Generic;
// using NRUSharp.common;
// using NRUSharp.common.data;
// using NRUSharp.core;
// using NRUSharp.core.data;
// using NRUSharp.core.stationImpl;
// using SimSharp;
// using Xunit;
// using Xunit.Abstractions;
//
// namespace NRUSharp.tests{
//     public class StandardFbeTests{
//         private readonly ITestOutputHelper output;
//
//         public StandardFbeTests(ITestOutputHelper output)
//         {
//             this.output = output;
//
//         }
//         [Fact]
//         public void FloatingCotFixedFfpTest(){
//             var ffp = 5000;
//             var cotArray = new int[]{
//                 250, 750, 1250, 1750, 2250, 2750, 3250, 3750, 4250, 4750
//             };
//             var results = new List<StationResults>();
//             var rngWrapper = new RngWrapper();
//             rngWrapper.Init(55555);
//             foreach (var cot in cotArray){
//                 var simulation = new Simulation(defaultStep: TimeSpan.FromSeconds(1));
//                 var fbeTimes = new FbeTimes(9, cot, ffp);
//                 var channel = new Channel();
//                 var station = new StandardFbe("STANDARD FBE", simulation, channel, fbeTimes, 0, rngWrapper);
//                 simulation.Process(station.Start());
//                 simulation.Run(TimeSpan.FromSeconds(1_000_000));
//                 results.Add(station.Results);
//             }
//
//             foreach (var result in results){
//                 output.WriteLine("AirTime: {0}, SuccTrans: {1}, FailedTrans{2}", result.AirTime, result.SuccessfulTransmissions, result.FailedTransmissions);
//             }
//         }
//         
//         [Fact]
//         public void WithOffset(){
//             var ffp = 10000;
//             var cotArray = new int[]{
//                 1000,2000,3000,4000,5000,6000,7000,8000,9000
//             };
//             var results = new List<StationResults>();
//             var rngWrapper = new RngWrapper();
//             rngWrapper.Init(55555);
//             foreach (var cot in cotArray){
//                 var simulation = new Simulation(defaultStep: TimeSpan.FromSeconds(1));
//                 var fbeTimes = new FbeTimes(9, cot, ffp);
//                 var channel = new Channel();
//                 var station1 = new StandardFbe("STANDARD FBE 1", simulation, channel, fbeTimes, 0, rngWrapper);
//                 var station2 = new StandardFbe("STANDARD FBE 1", simulation, channel, fbeTimes, 2500, rngWrapper);
//                 var station3 = new StandardFbe("STANDARD FBE 1", simulation, channel, fbeTimes, 5000, rngWrapper);
//                 var station4 = new StandardFbe("STANDARD FBE 1", simulation, channel, fbeTimes, 7500, rngWrapper);
//                 simulation.Process(station1.Start());
//                 simulation.Process(station2.Start());
//                 simulation.Process(station3.Start());
//                 simulation.Process(station4.Start());
//                 simulation.Run(TimeSpan.FromSeconds(1_000_000));
//                 results.Add(station1.Results);
//                 results.Add(station2.Results);
//                 results.Add(station3.Results);
//                 results.Add(station4.Results);
//             }
//
//             foreach (var result in results){
//                 output.WriteLine("AirTime: {0}, SuccTrans: {1}, FailedTrans{2}", result.AirTime, result.SuccessfulTransmissions, result.FailedTransmissions);
//             }
//         }
//         
//     }
// }