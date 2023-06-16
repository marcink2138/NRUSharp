namespace NRUSharp.core.data{
    public class ChannelAccessDelay{
        public int LastSuccessfulTransmissionStartTime{ get; set; }
        public int CurrentDelaySum{ get; set; }

        private int _currentTransmissionStartTime;

        public double GetMean(int successfulTransmissionsNum){
            var result = decimal.Divide(new decimal(CurrentDelaySum), new decimal(successfulTransmissionsNum));
            return decimal.ToDouble(result);
        }

        public void TransmissionStarted(double simulationTimestamp){
            var converted = (int) simulationTimestamp;
            _currentTransmissionStartTime = converted;
        }

        public void Success(){
            if (LastSuccessfulTransmissionStartTime == 0){
                LastSuccessfulTransmissionStartTime = _currentTransmissionStartTime;
                return;
            }
            CurrentDelaySum += _currentTransmissionStartTime - LastSuccessfulTransmissionStartTime;
            LastSuccessfulTransmissionStartTime = _currentTransmissionStartTime;
        }
    }
}