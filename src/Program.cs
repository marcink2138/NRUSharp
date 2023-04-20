using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;
using NRUSharp.common;
using NRUSharp.common.data;
using NRUSharp.simulationFramework;
using NRUSharp.simulationFramework.json;
using SimSharp;

namespace NRUSharp{
    class Simulator{
        static void Main(string[] args){
            // RngWrapper.Init(55555);
            // var config = new LoggingConfiguration();
            // var logfile = new FileTarget("logfile"){FileName = "logs.log"};
            // config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            // LogManager.Configuration = config;
            //
            // var simulation = new Simulation(defaultStep: TimeSpan.FromSeconds(1));
            // var channel = new Channel();
            // var fbeTimes = new FBETimes();
            // fbeTimes.Cca = 9;
            // fbeTimes.Cot = 2500;
            // fbeTimes.Ffp = 5000;
            // List<GreedyEnhancedFbe> list = new List<GreedyEnhancedFbe>();
            // for (int i = 0; i < 2; i++){
            //     list.Add(new GreedyEnhancedFbe($"EFBE {i}", simulation, channel, fbeTimes, 0, 2));
            // }
            //
            // foreach (GreedyEnhancedFbe station in list){
            //     simulation.Process(station.Start());
            // }
            //
            // simulation.Run(TimeSpan.FromSeconds(10_000_000));
            // var ev = simulation.ProcessedEvents;
            // Console.WriteLine($"Processed events: {ev}");
            // foreach (var station in list){
            //     Console.WriteLine($"Station name: {station.Name}");
            //     Console.WriteLine($"Successful transmissions: {station.Results.SuccessfulTransmissions}");
            //     Console.WriteLine($"Unsuccessful transmissions: {station.Results.FailedTransmissions}");
            //     Console.WriteLine($"Airtime: {station.Results.AirTime}");
            // }
            string path = @"C:\Users\marci\Desktop\inz\test\NRUSharp\NRUSharp\resources\scenario_exampl.json";
            var simulationObjectDescriptions = ScenarioCreator.GetScenarioDescription(path);
            Console.WriteLine(simulationObjectDescriptions);
        }
    }
}