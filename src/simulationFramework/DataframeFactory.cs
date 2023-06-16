using Microsoft.Data.Analysis;
using NRUSharp.core.interfaces;
using NRUSharp.simulationFramework.constants;
using NRUSharp.simulationFramework.interfaces;

namespace NRUSharp.simulationFramework{
    public class DataframeFactory : IDataframeFactory{
        public DataFrame CreateStationDataFrame(){
            var nameColumn = new StringDataFrameColumn(DfColumns.Name);
            var airTimeColumn = new PrimitiveDataFrameColumn<int>(DfColumns.Airtime);
            var successfulTransmissionsColumn = new PrimitiveDataFrameColumn<int>(DfColumns.SuccessfulTransmissions);
            var failedTransmissionsColumn = new PrimitiveDataFrameColumn<int>(DfColumns.FailedTransmissions);
            var ffpColumn = new PrimitiveDataFrameColumn<int>(DfColumns.Ffp);
            var cotColumn = new PrimitiveDataFrameColumn<int>(DfColumns.Cot);
            var offsetColumn = new PrimitiveDataFrameColumn<int>(DfColumns.Offset);
            var simulationRunColumn = new PrimitiveDataFrameColumn<int>(DfColumns.SimulationRun);
            var stationVersionColumn = new StringDataFrameColumn(DfColumns.StationVersion);
            var meanChannelAccessDelay = new PrimitiveDataFrameColumn<double>(DfColumns.MeanChannelAccessDelay);

            return new DataFrame(nameColumn,
                airTimeColumn,
                successfulTransmissionsColumn,
                failedTransmissionsColumn,
                ffpColumn,
                cotColumn,
                offsetColumn,
                simulationRunColumn,
                stationVersionColumn,
                meanChannelAccessDelay);
        }

        public DataFrame CreateAggregatedDataFrame(){
            var simulationRunColumn = new PrimitiveDataFrameColumn<int>(DfColumns.SimulationRun);
            var fairnessIndexColumn = new PrimitiveDataFrameColumn<double>(DfColumns.FairnessIndex);
            var channelEfficiencyColumn = new PrimitiveDataFrameColumn<double>(DfColumns.ChannelEfficiency);
            return new DataFrame(simulationRunColumn, channelEfficiencyColumn, fairnessIndexColumn);
        }
    }
}