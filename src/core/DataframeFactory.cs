using Microsoft.Data.Analysis;
using NRUSharp.common.constants;
using NRUSharp.common.interfaces;

namespace NRUSharp.common{
    public class DataframeFactory : IDataframeFactory{
        public DataFrame CreateDataFrame(){
            var nameColumn = new StringDataFrameColumn(DfColumns.Name);
            var airTimeColumn = new PrimitiveDataFrameColumn<int>(DfColumns.Airtime);
            var successfulTransmissionsColumn = new PrimitiveDataFrameColumn<int>(DfColumns.SuccessfulTransmissions);
            var failedTransmissionsColumn = new PrimitiveDataFrameColumn<int>(DfColumns.FailedTransmissions);
            var ffpColumn = new PrimitiveDataFrameColumn<int>(DfColumns.Ffp);
            var cotColumn = new PrimitiveDataFrameColumn<int>(DfColumns.Cot);
            var fbeVersionColumn = new StringDataFrameColumn(DfColumns.FbeVersion);

            return new DataFrame(nameColumn,
                airTimeColumn,
                successfulTransmissionsColumn,
                failedTransmissionsColumn,
                ffpColumn,
                cotColumn,
                fbeVersionColumn);
        }
    }
}