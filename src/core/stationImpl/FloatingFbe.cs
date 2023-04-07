using System.Collections.Generic;
using NRUSharp.common;
using NRUSharp.common.data;
using NRUSharp.common.interfaces;
using SimSharp;

namespace NRUSharp.impl{
    public class FloatingFbe : BaseStation{
        private readonly int _offsetSlotsNum;
        private int _selectedSlotsNum;

        public FloatingFbe(string name, Simulation env, IChannel channel, FBETimes fbeTimes, int offset, IRngWrapper rngWrapper) : base(name,
            env, channel, fbeTimes, offset, rngWrapper){
            _offsetSlotsNum = (fbeTimes.Ffp - fbeTimes.Cot - fbeTimes.Cca) / fbeTimes.Cca;
        }

        public override IEnumerable<Event> Start(){
            Logger.Info("{}|Starting station -> {}", Env.NowD, Name);
            yield return Env.Process(PerformInitOffset());
            while (true){
                yield return Env.Process(HandleRandomOffsetBeforeCca());
                yield return Env.Process(PerformCca());
                if (IsChannelIdle){
                    yield return Env.Process(PerformTransmission());
                    var timeUntilNextFfp = FbeTimes.Ffp - FbeTimes.Cot - (_selectedSlotsNum + 1) * FbeTimes.Cca;
                    Logger.Debug("{}|Time until next ffp -> {}", Env.NowD, timeUntilNextFfp);
                    yield return Env.TimeoutD(timeUntilNextFfp);
                }
                else{
                    var timeUntilNextFfp = FbeTimes.Ffp - (_selectedSlotsNum + 1) * FbeTimes.Cca;
                    Logger.Debug("{}|Skipping transmission. Time until next ffp -> {}", Env.NowD, timeUntilNextFfp);
                    yield return Env.TimeoutD(timeUntilNextFfp);
                }
            }
        }

        private IEnumerable<Event> HandleRandomOffsetBeforeCca(){
            _selectedSlotsNum = SelectRandomNumber(_offsetSlotsNum, 0);
            var offset = _selectedSlotsNum * FbeTimes.Cca;
            Logger.Debug("{}|Selected offset before CCA -> {}", Env.NowD, offset);
            yield return Env.TimeoutD(_selectedSlotsNum * FbeTimes.Cca);
        }
    }
}