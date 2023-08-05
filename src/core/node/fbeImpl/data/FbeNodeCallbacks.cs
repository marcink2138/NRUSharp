using System;
using System.Collections.Generic;

namespace NRUSharp.core.node.fbeImpl.data{
    public class FbeNodeCallbacks{
        public enum Type{
            SuccessfulCca,
            FailedCca
        }

        private readonly Dictionary<Type, List<Action>> _callbacks = new();

        public void AddCallback(Type type, Action action){
            if (!_callbacks.ContainsKey(type)){
                _callbacks.Add(type, new List<Action>());   
            }
            _callbacks[type].Add(action);
        }

        public void ExecuteCallbacks(Type type){
            if (_callbacks.ContainsKey(type)){
                _callbacks[type].ForEach(action => action.Invoke());   
            }
        }
    }
}