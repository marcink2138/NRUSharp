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
        public IEnumerable<Event> StartTransmission();
        public IEnumerable<Event> FinishTransmission(bool isSuccessful, double timeLeft);
        public IEnumerable<Event> Start();
        public void FailedTransmission();
        public void SuccessfulTransmission();
        public IEnumerable<Event> StartCca();
        public IEnumerable<Event> FinishCca(bool isSuccessful, double timeLeft);
        public (bool isSuccessful, double timeLeft) DeterminateTransmissionStatus();
        public (bool isSuccessful, double timeLeft) DeterminateCcaStatus();
        public IEnumerable<Event> PerformInitOffset();
        public IEnumerable<Event> PerformCca();
        public IEnumerable<Event> PerformTransmission();
        public void ResetStation();
        public List<KeyValuePair<string, object>> FetchResults();
        public StationType GetStationType();
        public void MountTrafficGenerator(ITrafficGenerator<Frame> trafficGenerator);
    }
}