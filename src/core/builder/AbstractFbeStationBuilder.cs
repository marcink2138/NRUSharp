using NRUSharp.core.interfaces;
using SimSharp;

namespace NRUSharp.core.builder{
    public abstract class AbstractFbeStationBuilder : IStationBuilder{
        protected int Offset;
        protected string Name;
        protected IRngWrapper RngWrapper;
        protected int Ffp;
        protected int Cot;
        protected int Cca;
        protected int SimulationTime;
        protected IChannel Channel;
        protected Simulation Env;

        public abstract IStation Build(bool reset = false);

        public AbstractFbeStationBuilder WithOffset(int offset){
            Offset = offset;
            return this;
        }

        public AbstractFbeStationBuilder WithName(string name){
            Name = name;
            return this;
        }

        public AbstractFbeStationBuilder WithRngWrapper(IRngWrapper rngWrapper){
            RngWrapper = rngWrapper;
            return this;
        }

        public AbstractFbeStationBuilder WithFfp(int ffp){
            Ffp = ffp;
            return this;
        }

        public AbstractFbeStationBuilder WithSimulationTime(int simulationTime){
            SimulationTime = simulationTime;
            return this;
        }

        public AbstractFbeStationBuilder WithEnv(Simulation env){
            Env = env;
            return this;
        }

        public AbstractFbeStationBuilder WithCot(int cot){
            Cot = cot;
            return this;
        }

        public AbstractFbeStationBuilder WithCca(int cca){
            Cca = cca;
            return this;
        }

        public AbstractFbeStationBuilder WithChannel(IChannel channel){
            Channel = channel;
            return this;
        }

        public virtual void Reset(){
            Offset = 0;
            Name = null;
            RngWrapper = null;
            Ffp = 0;
            Cot = 0;
            SimulationTime = 0;
            Channel = null;
            Env = null;
        }
    }
}