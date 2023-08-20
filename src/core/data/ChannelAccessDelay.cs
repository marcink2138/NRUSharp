using System;
using NRUSharp.core.node.fbeImpl.data;

namespace NRUSharp.core.data{
    public class ChannelAccessDelay{
        private long LastSuccessfulChannelAccess{ get; set; }
        private long CurrentDelaySum{ get; set; }

        private int _currentChannelAccessTime;
        private long ChannelAccessCounter{ get; set; }

        public double GetMean(int simulationTime){
            if (ChannelAccessCounter < 2){
                return simulationTime;
            }

            CurrentDelaySum += simulationTime - LastSuccessfulChannelAccess;
            var result = decimal.Divide(new decimal(CurrentDelaySum), new decimal(ChannelAccessCounter));
            return decimal.ToDouble(result);
        }

        public void Collect(CotStatusDescription cotStatusDescription){
            if (cotStatusDescription.NumberOfSuccessfulTransmissions == 0){
                return;
            }
            _currentChannelAccessTime = cotStatusDescription.FirstSuccessfulTransmissionTimestamp;
            ChannelAccessCounter++;
            if (LastSuccessfulChannelAccess == 0){
                LastSuccessfulChannelAccess = _currentChannelAccessTime;
                return;
            }

            if (_currentChannelAccessTime - LastSuccessfulChannelAccess <= 0){
                Console.WriteLine("");
            }
            CurrentDelaySum += _currentChannelAccessTime - LastSuccessfulChannelAccess;
            LastSuccessfulChannelAccess = _currentChannelAccessTime;
        }
    }
}