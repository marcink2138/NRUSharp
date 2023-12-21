namespace NRUSharp.core.data{
    public class NodeResults{
        public int SuccessfulTransmissions{ get; set; }

        public int FailedTransmissions{ get; set; }

        public int AirTime{ get; set; }

        public ChannelAccessDelay ChannelAccessDelay{ get; } = new();


        public double GetMeanChannelAccessDelay(int simulationTime){
            return ChannelAccessDelay.GetMean(simulationTime);
        }
    }
}