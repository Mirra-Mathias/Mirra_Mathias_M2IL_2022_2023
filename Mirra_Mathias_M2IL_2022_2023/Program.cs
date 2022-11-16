using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace devoir
{
    class Program
    {
        public static ProgressViewer progressViewer = new ProgressViewer();
        static async Task Main(string[] args)
        {

            string directory = Environment.CurrentDirectory;
            if(directory.Contains("bin"))
            {
                directory = directory.Substring(0, directory.IndexOf("bin"));
            }
            directory += "Universe/";
            
            // Deserialize synchrone
            List<System> universe = DeserializeAll(directory);

            // Deserialize asynchrone
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Task<List<ClassedSystem>> tasks = DeserializeAllAsync(directory);
            await tasks;
            List<ClassedSystem> universe2 = tasks.Result;
            // foreach (ClassedSystem universe in universe2)
            // {
            //     Console.WriteLine(universe.toString());
            // }
            sw.Stop();
        }
        
        // Deserialize synchrone
        
        static List<System> DeserializeAll(string directoryPath)
        {
            List<System> systems = new List<System>();
            foreach (string folderPath in Directory.GetDirectories(directoryPath))
            {
                System system = DeserializeSystem(folderPath);
                systems.Add(system);
            }
            return systems;
        }
        
        static System DeserializeSystem(string folderPath)
        {
            System system = new System();
            system.name = folderPath.Substring(folderPath.LastIndexOf("/") + 1);
            system.planets = new List<Planet>();
            foreach (string filePath in Directory.GetFiles(folderPath))
            {
                 system.planets.Add(DeserializePlanet(filePath));
            }
            
            return  system;
        }
        
        static Planet DeserializePlanet(string filePath)
        {
            string data = File.ReadAllText(filePath);
            Planet result = JsonConvert.DeserializeObject<Planet>(data);
            result.name = filePath.Substring(filePath.LastIndexOf("/") + 1).Replace(".json", "");
            return result;
        }
        
        // Deserialize asynchrone
        
        static  async Task<List<ClassedSystem>> DeserializeAllAsync(string directoryPath)
        {
            List<ClassedSystem> systems = new List<ClassedSystem>();

            List<Task<System>> tasks = new List<Task<System>>();
            foreach (string folderPath in Directory.GetDirectories(directoryPath))
            {
                Task<System> task = Task.Run(() => DeserializeSystemAsync(folderPath));
                tasks.Add(task);
            }

            await Task.WhenAll<System>(tasks);

            foreach (Task<System> task in tasks)
            {
                systems.Add(new ClassedSystem(task.Result));
            }

            return systems;
        }
        
        static async Task<System> DeserializeSystemAsync(string folderPath)
        {
            System system = new System();
            List<Planet> planets = new List<Planet>();
            List<Task<Planet>> tasks = new List<Task<Planet>>();
            system.name = folderPath.Substring(folderPath.LastIndexOf("/") + 1);
            system.planets = new List<Planet>();
            foreach (string filePath in Directory.GetFiles(folderPath))
            {
                Task<Planet> task = Task.Run(() => DeserializePlanetAsync(filePath));
                tasks.Add(task);
                task.GetAwaiter().OnCompleted( () =>
                {
                    task.Result.startEvent();
                });
                system.planets.Add(DeserializePlanetAsync(filePath));
            }
            
            await Task.WhenAll<Planet>(tasks);

            foreach (Task<Planet> task in tasks)
            {
                // task.Result.startEvent();
                planets.Add(task.Result);
            }

            system.planets = planets;
            
            return  system;
        }
        
        static Planet DeserializePlanetAsync(string filePath)
        {
            string data = File.ReadAllText(filePath);
            Planet result = JsonConvert.DeserializeObject<Planet>(data);
            
            result.progressEvent += progressViewer.UpdateProgressEvent;
            result.name = filePath.Substring(filePath.LastIndexOf("/") + 1);

            return result;
        }
    }
}