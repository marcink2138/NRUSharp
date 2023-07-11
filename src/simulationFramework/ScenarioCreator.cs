using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NRUSharp.core;
using NRUSharp.core.interfaces;
using NRUSharp.simulationFramework.json;

namespace NRUSharp.simulationFramework{
    //TODO currently not working
    public class ScenarioCreator{
        public List<List<INode>> GetScenarioSimulationMatrix(List<SimulationObjectDescription> objectDescriptions){
            var scenarioSimulationNum = objectDescriptions.Max(objDesc => objDesc.ParamsDescription.Count);

            //Matrix creation
            var scenarioMatrix = new List<List<INode>>();
            for (int i = 0; i < scenarioSimulationNum; i++){
                scenarioMatrix.Add(new List<INode>());
            }

            foreach (var objDesc in objectDescriptions){
                CreateStations(objDesc, scenarioMatrix);
            }

            return null;
        }

        private void CreateStations(SimulationObjectDescription objDesc, List<List<INode>> scenarioMatrix){
            var counter = 0;
            string station = null;
            //TODO create by abstract builder
            foreach (var keyValue in objDesc.ParamsDescription){ }
        }

        public static List<SimulationObjectDescription> GetScenarioDescription(string path){
            var scenario = FetchScenarioFromJson(path);
            var descriptions = new List<SimulationObjectDescription>();

            try{
                foreach (var simulationObject in scenario.EnhancedFbe ?? Enumerable.Empty<BaseFbeParamsJson>()){
                    var simulationObjectDescription = CreateObjectDescription(simulationObject);
                    simulationObjectDescription.StationType = StationType.EnhancedFbe;
                    descriptions.Add(simulationObjectDescription);
                }

                foreach (var simulationObject in scenario.FloatingFbe ?? Enumerable.Empty<BaseFbeParamsJson>()){
                    var simulationObjectDescription = CreateObjectDescription(simulationObject);
                    simulationObjectDescription.StationType = StationType.FloatingFbe;
                    descriptions.Add(simulationObjectDescription);
                }

                foreach (var simulationObject in scenario.StandardFbe ?? Enumerable.Empty<BaseFbeParamsJson>()){
                    var simulationObjectDescription = CreateObjectDescription(simulationObject);
                    simulationObjectDescription.StationType = StationType.StandardFbe;
                    descriptions.Add(simulationObjectDescription);
                }

                foreach (var simulationObject in scenario.FixedMutingFbe ?? Enumerable.Empty<BaseFbeParamsJson>()){
                    var simulationObjectDescription = CreateObjectDescription(simulationObject);
                    simulationObjectDescription.StationType = StationType.FixedMutingFbe;
                    descriptions.Add(simulationObjectDescription);
                }

                foreach (var simulationObject in scenario.RandomMutingFbe ?? Enumerable.Empty<BaseFbeParamsJson>()){
                    var simulationObjectDescription = CreateObjectDescription(simulationObject);
                    simulationObjectDescription.StationType = StationType.RandomMutingFbe;
                    descriptions.Add(simulationObjectDescription);
                }

                foreach (var simulationObject in scenario.GreedyEnhancedFbe ?? Enumerable.Empty<BaseFbeParamsJson>()){
                    var simulationObjectDescription = CreateObjectDescription(simulationObject);
                    simulationObjectDescription.StationType = StationType.GreedyEnhancedFbe;
                    descriptions.Add(simulationObjectDescription);
                }
            }
            catch (ArgumentException e){
                Console.WriteLine(e);
                Environment.Exit(404);
            }

            return descriptions;
        }

        private static SimulationObjectDescription CreateObjectDescription(object obj){
            var fields = obj.GetType().GetProperties();
            var objDesc = new SimulationObjectDescription();
            var dict = new Dictionary<string, List<int>>();
            foreach (var field in fields){
                if (field.GetValue(obj) == null){
                    throw new ArgumentException(
                        $"Cannot create scenario description!. Field {field.Name} is null for {obj.GetType()}");
                }

                if (field.Name.Equals("Name")){
                    objDesc.Name = (string) field.GetValue(obj);
                    continue;
                }

                var param = ((string) field.GetValue(obj))?.Split(";");
                var numeric = ConvertToNumeric(param);
                dict.Add(field.Name, numeric);
            }

            var maxSize = 0;
            foreach (var value in dict.Values){
                if (value.Count > maxSize){
                    maxSize = value.Count;
                }
            }

            foreach (var value in dict.Values){
                if (value.Count > maxSize){
                    continue;
                }

                var last = value.Last();
                for (int i = 0; i < maxSize - value.Count; i++){
                    value.Add(last);
                }
            }

            objDesc.ParamsDescription = dict;
            return objDesc;
        }

        private static List<int> ConvertToNumeric(string[] param){
            var intParams = new List<int>();
            for (var i = 0; i < param.Length; i++){
                if (!param[i].Contains("..")){
                    intParams.Add(int.Parse(param[i]));
                    continue;
                }

                var range = param[i].Split("..");
                if (range.Length > 2){
                    throw new ArgumentException(
                        $"Cannot create scenario description!. Cannot parse random param: {param[i]}");
                }

                var bottom = int.Parse(range[0]);
                var top = int.Parse(range[1]);
                if (bottom > top){
                    throw new ArgumentException(
                        $"Cannot create scenario description!. Wrong random param range --> bottom = {bottom}, top = {top}");
                }

                intParams.Add(new Random().Next(bottom, top + 1));
            }

            return intParams;
        }

        private static Scenario FetchScenarioFromJson(string path){
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Scenario>(json);
        }
    }
}