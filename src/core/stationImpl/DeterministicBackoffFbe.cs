using System.Collections.Generic;
using SimSharp;

namespace NRUSharp.core.stationImpl{
    public class DeterministicBackoffFbe:BaseStation{
        public override IEnumerable<Event> Start(){
            throw new System.NotImplementedException();
        }

        public override StationType GetStationType(){
            return StationType.DeterministicBackoffFbe;
        }
    }
}