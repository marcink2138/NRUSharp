using System;
using System.Collections.Generic;
using System.Linq;
using NRUSharp.simulationFramework.constants;

namespace NRUSharp.simulationFramework{
    public static class ResultsProcessor{
        public static decimal CalculateFairness(List<int> airTimes, int stationNum){
            try{
                var airTimeSum = airTimes.Sum();
                var fairnessDenominator = new decimal(0);
                airTimes.ForEach(airTime => {
                    var pow = decimal.Multiply(airTime, airTime);
                    fairnessDenominator = decimal.Add(fairnessDenominator, pow);
                });
                var fairnessNominator = decimal.Multiply(airTimeSum, airTimeSum);
                fairnessDenominator = decimal.Multiply(fairnessDenominator, stationNum);
                return decimal.Divide(fairnessNominator, fairnessDenominator);
            }
            catch (DivideByZeroException){
                return decimal.Zero;
            }
        }

        public static decimal CalculateChannelEfficiency(List<int> airTimes, int simulationTime){
            var airTimeSum = airTimes.Sum();
            return decimal.Divide(airTimeSum, simulationTime);
        }

        public static List<KeyValuePair<string, object>> CreateAggregatedDfRow(int simulationRun, double fairness,
            double channelEfficiency){
            return new List<KeyValuePair<string, object>>{
                new(DfColumns.SimulationRun, simulationRun),
                new(DfColumns.FairnessIndex, fairness),
                new(DfColumns.ChannelEfficiency, channelEfficiency)
            };
        }
    }
}