using System;
using System.Collections.Generic;
using NLog;
using NRUSharp.core.node;

namespace NRUSharp.core.channel.impl{
    public class Channel : IChannel{
        private readonly List<INode> _ccaList;
        private readonly List<INode> _transmissionList;
        private readonly Logger _logger = LogManager.GetLogger("Channel");

        public Channel(){
            _ccaList = new List<INode>();
            _transmissionList = new List<INode>();
        }

        public int GetTransmissionListSize(){
            return _transmissionList.Count;
        }

        public void AddToCcaList(INode baseNode){
            _ccaList.Add(baseNode);
        }

        public void AddToTransmissionList(INode baseNode){
            _transmissionList.Add(baseNode);
        }

        public void RemoveFromTransmissionList(INode baseNode){
            _transmissionList.Remove(baseNode);
        }

        public void RemoveFromCcaList(INode baseNode){
            _ccaList.Remove(baseNode);
        }

        public void InterruptCca(){
            foreach (var station in _ccaList){
                try{
                    if (station.Cca.IsAlive && station.Cca.IsOk){
                        _logger.Debug($"Interrupting CCA of station: {station.Name}");
                        station.Cca.Interrupt();
                    }
                    else{
                        _logger.Debug(
                            $"Interruption of CCA process of station: {station.Name} failed. IsAlive = {station.Cca.IsAlive}, IsOk= {station.Cca.IsOk}");
                    }
                }
                catch (InvalidOperationException){
                    _logger.Warn("Exception caught during {} station CCA process interruption", station.Name);
                }
            }
        }

        public void InterruptOnGoingTransmissions(){
            foreach (var station in _transmissionList){
                try{
                    if (station.Transmission.IsAlive && station.Transmission.IsOk){
                        _logger.Debug($"Interrupting transmission of station: {station.Name}");
                        station.Transmission.Interrupt();
                    }
                    else{
                        _logger.Debug(
                            $"Interruption of transmission process of station: {station.Name} failed. IsAlive = {station.Transmission.IsAlive}, IsOk= {station.Transmission.IsOk}");
                    }
                }
                catch (InvalidOperationException){
                    _logger.Warn("Exception caught during {} station transmission process interruption", station.Name);
                }
            }
        }

        public void ResetChannel(){
            _ccaList.Clear();
            _transmissionList.Clear();
        }
    }
}