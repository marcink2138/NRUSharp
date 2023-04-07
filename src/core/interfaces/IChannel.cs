namespace NRUSharp.common.interfaces{
    public interface IChannel{
        public int GetTransmissionListSize();
        public void AddToCcaList(BaseStation baseStation);
        public void AddToTransmissionList(BaseStation baseStation);
        public void RemoveFromTransmissionList(BaseStation baseStation);
        public void RemoveFromCcaList(BaseStation baseStation);
        public void InterruptCca();
        public void InterruptOnGoingTransmissions();
    }
}