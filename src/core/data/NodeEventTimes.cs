namespace NRUSharp.core.data{
    public class NodeEventTimes{
        public double CcaStart{ get; set; }
        public double CcaEnd{ get; set; }
        public double TransmissionStart{ get; set; }
        public double TransmissionEnd{ get; set; }
        public double CotStart{ get; set; }
        public double CotEnd{ get; set; }

        public void Reset(){
            CcaStart = 0;
            CcaEnd = 0;
            TransmissionEnd = 0;
            TransmissionStart = 0;
            CotStart = 0;
            CotEnd = 0;
        }
        
    }
}