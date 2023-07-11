using NRUSharp.core.node.fbeImpl;

namespace NRUSharp.core.channel{
    public interface IChannel{
        public int GetTransmissionListSize();
        public void AddToCcaList(BaseNode baseNode);
        public void AddToTransmissionList(BaseNode baseNode);
        public void RemoveFromTransmissionList(BaseNode baseNode);
        public void RemoveFromCcaList(BaseNode baseNode);
        public void InterruptCca();
        public void InterruptOnGoingTransmissions();
        public void ResetChannel();
    }
}