using System.Collections.Generic;
using NRUSharp.core.node;

namespace NRUSharp.tests{
    public class TestHelper{
        public static List<List<INode>> CreateScenarioMatrix(int simulationNumber){
            var list = new List<List<INode>>();
            for (int i = 0; i < simulationNumber; i++){
                list.Add(new List<INode>());
            }

            return list;
        }
    }
}