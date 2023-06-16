using Microsoft.Data.Analysis;

namespace NRUSharp.simulationFramework.interfaces{
    public interface IDataframeFactory{
        public DataFrame CreateStationDataFrame();
        public DataFrame CreateAggregatedDataFrame();
    }
}