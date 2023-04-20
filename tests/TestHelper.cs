using System.Collections.Generic;
using NRUSharp.core.interfaces;

namespace NRUSharp.tests{
    public class TestHelper{
        public static List<List<IStation>> CreateScenarioMatrix(int simulationNumber){
            var list = new List<List<IStation>>();
            for (int i = 0; i < simulationNumber; i++){
                list.Add(new List<IStation>());
            }

            return list;
        }
    }
}