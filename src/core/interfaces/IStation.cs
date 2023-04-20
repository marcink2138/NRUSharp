using System.Collections.Generic;
using SimSharp;

namespace NRUSharp.core.interfaces{
    public interface IStation{
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
        public void SetSimulationEnvironment(Simulation simulation);
        public void SetChannel(IChannel channel);
    }
}