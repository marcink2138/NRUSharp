using System.Collections.Generic;
using SimSharp;

namespace NRUSharp.core.trafficGenerator.impl{
    public class SimpleTrafficGenerator<T> : AbstractTrafficGenerator<T>{
        public override IEnumerable<Event> Start(NodeQueue<T> receiver){
            ItsQueue = receiver;
            Logger.Debug(
                "Put single unit right now, exit generation process and finally put traffic only after notyfication");
            ItsQueue.Enqueue(GenerateUnit());
            yield break;
        }

        public override void Dequeue(){
            Logger.Debug("Notification recieved");
            ItsQueue.Enqueue(GenerateUnit());
        }
    }
}