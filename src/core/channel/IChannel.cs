using NRUSharp.core.node;

namespace NRUSharp.core.channel{
    public interface IChannel{
        public int GetTransmissionListSize();
        public void AddToCcaList(INode baseNode);
        public void AddToTransmissionList(INode baseNode);
        public void RemoveFromTransmissionList(INode baseNode);
        public void RemoveFromCcaList(INode baseNode);
        public void InterruptCca();
        public void InterruptOnGoingTransmissions();
        public void ResetChannel();
    }
}