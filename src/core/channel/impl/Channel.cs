using System;
using System.Collections.Generic;
using NLog;
using NRUSharp.core.node.fbeImpl;

namespace NRUSharp.core.channel.impl{
    public class Channel : IChannel{
        private readonly List<BaseNode> _ccaList;
        private readonly List<BaseNode> _transmissionList;
        private readonly Logger _logger = LogManager.GetLogger("Channel");

        public Channel(){
            _ccaList = new List<BaseNode>();
            _transmissionList = new List<BaseNode>();
        }

        public int GetTransmissionListSize(){
            return _transmissionList.Count;
        }

        public void AddToCcaList(BaseNode baseNode){
            _ccaList.Add(baseNode);
        }

        public void AddToTransmissionList(BaseNode baseNode){
            _transmissionList.Add(baseNode);
        }

        public void RemoveFromTransmissionList(BaseNode baseNode){
            _transmissionList.Remove(baseNode);
        }

        public void RemoveFromCcaList(BaseNode baseNode){
            _ccaList.Remove(baseNode);
        }

        public void InterruptCca(){
            foreach (var station in _ccaList){
                try{
                    if (station.CcaProcess.IsAlive && station.CcaProcess.IsOk){
                        _logger.Debug($"Interrupting CCA of station: {station.Name}");
                        station.CcaProcess.Interrupt();
                    }
                    else{
                        _logger.Debug(
                            $"Interruption of CCA process of station: {station.Name} failed. IsAlive = {station.CcaProcess.IsAlive}, IsOk= {station.CcaProcess.IsOk}");
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
                    if (station.TransmissionProcess.IsAlive && station.TransmissionProcess.IsOk){
                        _logger.Debug($"Interrupting transmission of station: {station.Name}");
                        station.TransmissionProcess.Interrupt();
                    }
                    else{
                        _logger.Debug(
                            $"Interruption of transmission process of station: {station.Name} failed. IsAlive = {station.TransmissionProcess.IsAlive}, IsOk= {station.TransmissionProcess.IsOk}");
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