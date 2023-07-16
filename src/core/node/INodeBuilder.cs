namespace NRUSharp.core.node{
    public interface IStationBuilder{
        public INode Build(bool reset = false);
        public void Reset();
    }
}