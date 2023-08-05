using System.Collections.Generic;
using NRUSharp.core.channel;
using NRUSharp.core.data;
using NRUSharp.core.rngWrapper;
using NRUSharp.core.trafficGenerator;
using SimSharp;

namespace NRUSharp.core.node{
    public interface INode{
        public string Name{ get; init; }
        public Simulation Env{ get; set; }
        public IChannel Channel{ get; set; }
        public Process Transmission{ get; set; }
        public Process Cca{ get; set; }
        public IRngWrapper RngWrapper{ get; init; }
        public int QueueCapacity{ get; init; }
        public SimulationParams SimulationParams{ get; init; }
        public IEnumerable<Event> Start();
        public void ResetNode();
        public List<KeyValuePair<string, object>> FetchResults();
        public NodeType GetNodeType();
        public void MountTrafficGenerator(ITrafficGenerator<Frame> trafficGenerator);
    }
}