using System;
using System.Collections.Generic;
using NLog;
using NRUSharp.core.node;
using SimSharp;

namespace NRUSharp.core.channel.impl{
    public class Channel : IChannel{
        private readonly List<INode> _ccaList = new();
        private readonly List<INode> _transmissionList = new();
        private readonly Logger _logger = LogManager.GetLogger("Channel");

        public Simulation Env{ get; set; }

        public int GetTransmissionListSize(){
            return _transmissionList.Count;
        }

        public void AddToCcaList(INode baseNode){
            _logger.Debug($"{Env.NowD}|Appending node: {baseNode.Name} to CCA list");
            _ccaList.Add(baseNode);
        }

        public void AddToTransmissionList(INode baseNode){
            _logger.Debug($"{Env.NowD}|Appending node: {baseNode.Name} to transmission list");
            _transmissionList.Add(baseNode);
        }

        public void RemoveFromTransmissionList(INode baseNode){
            _logger.Debug($"{Env.NowD}|Removing node: {baseNode.Name} from transmission list");
            _transmissionList.Remove(baseNode);
        }

        public void RemoveFromCcaList(INode baseNode){
            _logger.Debug($"{Env.NowD}|Removing node: {baseNode.Name} from CCA list");
            _ccaList.Remove(baseNode);
        }

        public void InterruptCca(){
            foreach (var station in _ccaList){
                try{
                    if (station.Cca.IsAlive && station.Cca.IsOk){
                        _logger.Debug($"{Env.NowD}|Interrupting CCA of station: {station.Name}");
                        station.Cca.Interrupt();
                    }
                    else{
                        _logger.Debug(
                            $"{Env.NowD}|Interruption of CCA process of station: {station.Name} failed. IsAlive = {station.Cca.IsAlive}, IsOk= {station.Cca.IsOk}");
                    }
                }
                catch (InvalidOperationException){
                    _logger.Warn("{Env.NowD}|Exception caught during {} station CCA process interruption", station.Name);
                }
            }
        }

        public void InterruptOnGoingTransmissions(){
            foreach (var station in _transmissionList){
                try{
                    if (station.Transmission.IsAlive && station.Transmission.IsOk){
                        _logger.Debug($"{Env.NowD}|Interrupting transmission of station: {station.Name}");
                        station.Transmission.Interrupt();
                    }
                    else{
                        _logger.Debug(
                            $"{Env.NowD}|Interruption of transmission process of station: {station.Name} failed. IsAlive = {station.Transmission.IsAlive}, IsOk= {station.Transmission.IsOk}");
                    }
                }
                catch (InvalidOperationException){
                    _logger.Warn("{Env.NowD}|Exception caught during {} station transmission process interruption", station.Name);
                }
            }
        }

        public void ResetChannel(){
            _ccaList.Clear();
            _transmissionList.Clear();
        }
    }
}