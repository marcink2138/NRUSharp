using NRUSharp.core.node;
using SimSharp;

namespace NRUSharp.core.channel{
    public interface IChannel{
        public Simulation Env{ get; set; }
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