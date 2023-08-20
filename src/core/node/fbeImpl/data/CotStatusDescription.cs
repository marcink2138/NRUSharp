namespace NRUSharp.core.node.fbeImpl.data{
    public class CotStatusDescription{
        public int NumberOfSuccessfulTransmissions{ get; set; }
        public int NumberOfFailedTransmissions{ get; set; }

        public int Airtime{
            get => _airtime;
            set => _airtime += value;
        }

        private int _airtime;
        private int _firstSuccessfulTransmissionTimestamp;

        public int FirstSuccessfulTransmissionTimestamp{
            get => _firstSuccessfulTransmissionTimestamp;
            set{
                if (NumberOfSuccessfulTransmissions > 0){
                    return;
                }

                _firstSuccessfulTransmissionTimestamp = value;
            }
        }
    }
}