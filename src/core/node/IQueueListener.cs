namespace NRUSharp.core.node{
    public interface IQueueListener<in T>{
        public void HandleNewItem(T item){}
    }
}