namespace NRUSharp.core.data{
    public class NodeResults{
        public NodeResults(){
            ChannelAccessDelay = new ChannelAccessDelay();
        }

        public int SuccessfulTransmissions{ get; private set; }

        public int FailedTransmissions{ get; private set; }

        public int AirTime{ get; private set; }

        public ChannelAccessDelay ChannelAccessDelay{ get; }

        public void IncrementSuccessfulTransmissions(){
            SuccessfulTransmissions++;
        }

        public void IncrementFailedTransmissions(){
            FailedTransmissions++;
        }

        public void IncrementAirTime(int airTime){
            AirTime += airTime;
        }

        public double GetMeanChannelAccessDelay(int simulationTime){
            return ChannelAccessDelay.GetMean(simulationTime);
        }
        
    }
}