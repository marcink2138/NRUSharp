using NRUSharp.core.interfaces;

namespace NRUSharp.core.builder{
    public interface IStationBuilder{
        public IStation Build(bool reset = false);
        public void Reset();
    }
}