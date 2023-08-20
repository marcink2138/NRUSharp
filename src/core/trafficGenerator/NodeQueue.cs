using System.Collections.Generic;
using NRUSharp.core.node;
using NRUSharp.core.trafficGenerator.impl;
using SimSharp;

namespace NRUSharp.core.trafficGenerator{
    public class NodeQueue<TFrame> : Queue<TFrame>{
        public int MaxSize{ get; set; }
        private readonly IQueueListener<TFrame> _itsNode;

        public ITrafficGenerator<TFrame> TrafficGenerator{ get; set; }

        public NodeQueue(int capacity, IQueueListener<TFrame> itsNode) :
            base(capacity){
            MaxSize = capacity;
            _itsNode = itsNode;
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
                _itsNode.HandleNewItem(item);
            }
        }

        public new TFrame Dequeue(){
            TrafficGenerator.Dequeue();
            return base.Dequeue();
        }
    }
}