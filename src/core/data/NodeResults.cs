namespace NRUSharp.core.data{
    public class NodeResults{
        public NodeResults(){
            ChannelAccessDelay = new ChannelAccessDelay();
        }

        public int SuccessfulTransmissions{ get; private set; }

        public int FailedTransmissions{ get; private set; }

        public int AirTime{ get; private set; }

        public double MeanChannelAccessDelay{
            get{
                if (SuccessfulTransmissions -1 <= 0){
                    return -1;
                }

                return ChannelAccessDelay.GetMean(SuccessfulTransmissions - 1);
            }
        }

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
    }
}