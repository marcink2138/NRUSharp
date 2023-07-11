using System;
using System.Collections.Generic;
using NRUSharp.core.trafficGenerator.impl;
using SimSharp;

namespace NRUSharp.core.trafficGenerator{
    public class NodeQueue<TFrame> : Queue<TFrame>{
        public int MaxSize{ get; set; }
        public ITrafficGenerator TrafficGenerator{ private get; set; }
        private Func<Simulation, TFrame> _unitProvider;

        public NodeQueue(int capacity, Func<Simulation, TFrame> unitProvider) : base(capacity){
            MaxSize = capacity;
            _unitProvider = unitProvider;
            TrafficGenerator = new SimpleGenerator<TFrame>(this, _unitProvider);
        }

        public void Start(Simulation env){
            TrafficGenerator.SetSimulationEnvironment(env);
            env.Process(TrafficGenerator.Start());
        }

        public new void Enqueue(TFrame item){
            if (Count == MaxSize){
                return;
            }

            base.Enqueue(item);
        }

        public new TFrame Dequeue(){
            TrafficGenerator.Notify();
            return base.Dequeue();
        }
    }
}