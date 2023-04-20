using System;
using System.Collections.Generic;
using NLog;
using NRUSharp.core.interfaces;

namespace NRUSharp.core{
    public class Channel : IChannel{
        private readonly List<BaseStation> _ccaList;
        private readonly List<BaseStation> _transmissionList;
        private readonly Logger _logger = LogManager.GetLogger("Channel");

        public Channel(){
            _ccaList = new List<BaseStation>();
            _transmissionList = new List<BaseStation>();
        }

        public int GetTransmissionListSize(){
            return _transmissionList.Count;
        }

        public void AddToCcaList(BaseStation baseStation){
            _ccaList.Add(baseStation);
        }

        public void AddToTransmissionList(BaseStation baseStation){
            _transmissionList.Add(baseStation);
        }

        public void RemoveFromTransmissionList(BaseStation baseStation){
            _transmissionList.Remove(baseStation);
        }

        public void RemoveFromCcaList(BaseStation baseStation){
            _ccaList.Remove(baseStation);
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
                catch (InvalidOperationException e){
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
                catch (InvalidOperationException e){
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