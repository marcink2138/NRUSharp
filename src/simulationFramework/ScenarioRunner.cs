using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Data.Analysis;
using NRUSharp.core;
using NRUSharp.core.interfaces;
using NRUSharp.simulationFramework.interfaces;
using SimSharp;

namespace NRUSharp.simulationFramework{
    public class ScenarioRunner : IScenarioRunner{
        private IDataframeFactory _dataframeFactory = new DataframeFactory();

        public void RunScenario(ScenarioDescription scenarioDescription){
            var resultsDf = _dataframeFactory.CreateDataFrame();
            for (int i = 0; i < scenarioDescription.Repetitions; i++){
                PerformScenario(scenarioDescription.SimulationTime, scenarioDescription.ScenarioMatrix, resultsDf);
            }
            
            DataFrame.SaveCsv(resultsDf, "C:\\Users\\marci\\Desktop\\inz\\test\\NRUSharp\\NRUSharp\\tests\\results\\first.csv", separator:'|');
        }

        private void PerformScenario(int simulationTime, List<List<IStation>> scenarioMatrix, DataFrame dataFrame){
            foreach (var stationList in scenarioMatrix){
                var env = new Simulation(defaultStep: TimeSpan.FromSeconds(1));
                var channel = new Channel();
                PrepareEnvironment(stationList, env, channel);
                env.Run(TimeSpan.FromSeconds(simulationTime));
                CollectResults(dataFrame, stationList);
                ResetStations(stationList);
            }
        }

        private void PrepareEnvironment(List<IStation> stations, Simulation simulation, IChannel channel){
            foreach (var station in stations){
                station.SetSimulationEnvironment(simulation);
                station.SetChannel(channel);
                simulation.Process(station.Start());
            }
        }

        private void CollectResults(DataFrame dataFrame, List<IStation> stations){
            foreach (var station in stations){
                dataFrame.Append(station.FetchResults(), true);
            }
        }

        private void ResetStations(List<IStation> stations){
            foreach (var station in stations){
                station.ResetStation();
            }
        }
    }
}