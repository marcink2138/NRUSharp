﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Analysis;
using NLog;
using NRUSharp.core;
using NRUSharp.core.channel;
using NRUSharp.core.channel.impl;
using NRUSharp.core.node;
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
            var stationDf = _dataframeFactory.CreateStationDataFrame();
            var aggregatedDf = _dataframeFactory.CreateAggregatedDataFrame();
            for (var i = 0; i < scenarioDescription.Repetitions; i++){
                PerformScenario(scenarioDescription.SimulationTime, scenarioDescription.ScenarioMatrix, stationDf,
                    aggregatedDf);
                _logger.Info($"Run {i + 1} of {scenarioDescription.Repetitions} performed");
            }

            _logger.Info("Saving simulation results...");
            DataFrame.SaveCsv(stationDf,
                $"{scenarioDescription.ResultsFileName}.csv",
                separator: '|');
            DataFrame.SaveCsv(aggregatedDf,
                $"{scenarioDescription.ResultsFileName + "_aggregated.csv"}",
                '|');
        }

        private void PerformScenario(int simulationTime, IReadOnlyList<List<INode>> scenarioMatrix,
            DataFrame stationDf,
            DataFrame aggregatedDf){
            for (var i = 0; i < scenarioMatrix.Count; i++){
                var stationList = scenarioMatrix[i];
                /*
                 * Workaround - in the case of this simulator, basic simulation clock unit is micro second [us] (not available in .NET 5.0)
                 */
                var env = new Simulation(defaultStep: TimeSpan.FromSeconds(1));
                var channel = new Channel{Env = env};
                PrepareEnvironment(stationList, env, channel);
                env.Run(TimeSpan.FromSeconds(simulationTime));
                _logger.Info($"Number of processed events in current run: {env.ProcessedEvents}");
                CollectResults(stationDf, aggregatedDf, stationList, i + 1, simulationTime);
                ResetStations(stationList);
            }
        }

        private void PrepareEnvironment(List<INode> stations, Simulation simulation, IChannel channel){
            foreach (var station in stations){
                station.Env = simulation;
                station.Channel = channel;
                simulation.Process(station.Start());
            }
        }

        private void CollectResults(DataFrame stationDf, DataFrame aggregatedDf, List<INode> stations,
            int simulationRun, int simulationTime){
            var simulationRunColumn = new KeyValuePair<string, object>(DfColumns.SimulationRun, simulationRun);
            var airTimes = new List<int>();
            foreach (var keyValuePairs in stations.Select(station => station.FetchResults())){
                var airTime = keyValuePairs.Find(keyValuePair => keyValuePair.Key == DfColumns.Airtime).Value;
                airTimes.Add((int) airTime);
                keyValuePairs.Add(simulationRunColumn);
                stationDf.Append(keyValuePairs, true);
            }

            var fairness = ResultsProcessor.CalculateFairness(airTimes, stations.Count);
            var channelEfficiency = ResultsProcessor.CalculateChannelEfficiency(airTimes, simulationTime);

            var aggregatedDfRow = ResultsProcessor.CreateAggregatedDfRow(simulationRun, decimal.ToDouble(fairness),
                decimal.ToDouble(channelEfficiency));
            aggregatedDf.Append(aggregatedDfRow, true);
        }

        private void ResetStations(List<INode> stations){
            foreach (var station in stations){
                station.ResetNode();
            }
        }
    }
}