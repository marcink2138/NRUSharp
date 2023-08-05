using System;
using System.Collections.Generic;
using NRUSharp.core.node;
using NRUSharp.core.trafficGenerator.impl;
using SimSharp;

namespace NRUSharp.core.trafficGenerator{
    public class NodeQueue<TFrame> : Queue<TFrame>{
        public int MaxSize{ get; set; }
        private IQueueListener<TFrame> itsNode;
        private ITrafficGenerator<TFrame> _trafficGenerator;

        public ITrafficGenerator<TFrame> TrafficGenerator{
            private get{ return _trafficGenerator; }
            set{
                _trafficGenerator = value;
                _trafficGenerator.GeneratorUnitProvider = _unitProvider;
            }
        }

        private readonly Func<Simulation, TFrame> _unitProvider;

        public NodeQueue(int capacity, Func<Simulation, TFrame> unitProvider, IQueueListener<TFrame> itsNode) : base(capacity){
            MaxSize = capacity;
            _unitProvider = unitProvider;
            this.itsNode = itsNode;
            TrafficGenerator = new SimpleTrafficGenerator<TFrame>(_unitProvider);
        }

        public void Start(Simulation env){
            TrafficGenerator.Env = env;
            env.Process(TrafficGenerator.Start(this));
        }

        public new void Enqueue(TFrame item){
            if (Count == MaxSize){
                return;
            }
            base.Enqueue(item);
            if (Count == 1){
                // Notify only when currently queued item is first in the queue
                itsNode.HandleNewItem(item);   
            }
        }

        public new TFrame Dequeue(){
            TrafficGenerator.Dequeue();
            return base.Dequeue();
        }
    }
}