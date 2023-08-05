namespace NRUSharp.core.data{
    public class ChannelAccessDelay{
        private int LastSuccessfulChannelAccess{ get; set; }
        private int CurrentDelaySum{ get; set; }

        private int _currentChannelAccessTime;
        private int ChannelAccessCounter{ get; set; }

        public double GetMean(int simulationTime){
            if (ChannelAccessCounter < 2){
                return simulationTime;
            }

            CurrentDelaySum += simulationTime - LastSuccessfulChannelAccess;
            var result = decimal.Divide(new decimal(CurrentDelaySum), new decimal(ChannelAccessCounter));
            return decimal.ToDouble(result);
        }

        public void ChannelAccessed(double simulationTimestamp){
            var converted = (int) simulationTimestamp;
            _currentChannelAccessTime = converted;
            ChannelAccessCounter++;
            if (LastSuccessfulChannelAccess == 0){
                LastSuccessfulChannelAccess = _currentChannelAccessTime;
                return;
            }
            CurrentDelaySum += _currentChannelAccessTime - LastSuccessfulChannelAccess;
            LastSuccessfulChannelAccess = _currentChannelAccessTime;
        }
    }
}