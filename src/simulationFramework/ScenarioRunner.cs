using System;
using System.Collections.Generic;
using Microsoft.Data.Analysis;
using NLog;
using NRUSharp.core;
using NRUSharp.core.interfaces;
using NRUSharp.simulationFramework.constants;
using NRUSharp.simulationFramework.interfaces;
using SimSharp;

namespace NRUSharp.simulationFramework{
    public class ScenarioRunner : IScenarioRunner{
        private readonly IDataframeFactory _dataframeFactory = new DataframeFactory();
        private readonly Logger _logger;

        public ScenarioRunner(){
            var loggerName = $"{LogManagerWrapper.ConsoleLoggerPrefix}ScenarioRunner";
            _logger = LogManager.GetLogger(loggerName);
        }

        public void RunScenario(ScenarioDescription scenarioDescription){
            _logger.Info("Running scenario");
            var resultsDf = _dataframeFactory.CreateDataFrame();
            for (int i = 0; i < scenarioDescription.Repetitions; i++){
                PerformScenario(scenarioDescription.SimulationTime, scenarioDescription.ScenarioMatrix, resultsDf);
                _logger.Info($"Run {i + 1} of {scenarioDescription.Repetitions} performed");
            }

            _logger.Info("Saving simulation results...");
            DataFrame.SaveCsv(resultsDf,
                $"C:\\Users\\marci\\Desktop\\inz\\test\\NRUSharp\\NRUSharp\\tests\\results\\{scenarioDescription.ResultsFileName}",
                separator: '|');
        }

        private void PerformScenario(int simulationTime, List<List<IStation>> scenarioMatrix, DataFrame dataFrame){
            for (int i = 0; i < scenarioMatrix.Count; i++){
                var stationList = scenarioMatrix[i];
                var env = new Simulation(defaultStep: TimeSpan.FromSeconds(1));
                var channel = new Channel();
                PrepareEnvironment(stationList, env, channel);
                env.Run(TimeSpan.FromSeconds(simulationTime));
                _logger.Info($"Number of processed events in current run: {env.ProcessedEvents}");
                CollectResults(dataFrame, stationList, i + 1);
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

        private void CollectResults(DataFrame dataFrame, List<IStation> stations, int simulationRun){
            var simulationRunColumn = new KeyValuePair<string, object>(DfColumns.SimulationRun, simulationRun);
            foreach (var station in stations){
                var keyValuePairs = station.FetchResults();
                keyValuePairs.Add(simulationRunColumn);
                dataFrame.Append(keyValuePairs, true);
            }
        }

        private void ResetStations(List<IStation> stations){
            foreach (var station in stations){
                station.ResetStation();
            }
        }
    }
}