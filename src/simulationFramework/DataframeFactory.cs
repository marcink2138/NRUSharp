using Microsoft.Data.Analysis;
using NRUSharp.core.interfaces;
using NRUSharp.simulationFramework.constants;
using NRUSharp.simulationFramework.interfaces;

namespace NRUSharp.simulationFramework{
    public class DataframeFactory : IDataframeFactory{
        public DataFrame CreateDataFrame(){
            var nameColumn = new StringDataFrameColumn(DfColumns.Name);
            var airTimeColumn = new PrimitiveDataFrameColumn<int>(DfColumns.Airtime);
            var successfulTransmissionsColumn = new PrimitiveDataFrameColumn<int>(DfColumns.SuccessfulTransmissions);
            var failedTransmissionsColumn = new PrimitiveDataFrameColumn<int>(DfColumns.FailedTransmissions);
            var ffpColumn = new PrimitiveDataFrameColumn<int>(DfColumns.Ffp);
            var cotColumn = new PrimitiveDataFrameColumn<int>(DfColumns.Cot);
            var stationVersionColumn = new StringDataFrameColumn(DfColumns.StationVersion);

            return new DataFrame(nameColumn,
                airTimeColumn,
                successfulTransmissionsColumn,
                failedTransmissionsColumn,
                ffpColumn,
                cotColumn,
                stationVersionColumn);
        }
    }
}