namespace NRUSharp.core.data{
    public class StationResults{
        public int SuccessfulTransmissions{ get; set; }

        public int FailedTransmissions{ get; set; }

        public int AirTime{ get; set; }

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