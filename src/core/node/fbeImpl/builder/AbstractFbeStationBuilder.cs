using NRUSharp.core.channel;
using NRUSharp.core.interfaces;
using NRUSharp.core.rngWrapper;
using SimSharp;

namespace NRUSharp.core.node.fbeImpl.builder{
    public abstract class AbstractFbeStationBuilder : IStationBuilder{
        protected string Name;
        protected int OffsetBottom;
        protected int OffsetTop;
        protected IRngWrapper RngWrapper;
        protected int Ffp;
        protected int Cot;
        protected int Cca;
        protected int SimulationTime;
        protected IChannel Channel;
        protected Simulation Env;

        public abstract INode Build(bool reset = false);

        public AbstractFbeStationBuilder WithOffsetBottom(int bottom){
            OffsetBottom = bottom;
            return this;
        }

        public AbstractFbeStationBuilder WithOffsetTop(int top){
            OffsetTop = top;
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
            OffsetBottom = 0;
            OffsetTop = 0;
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