using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace devoir
{
    public class Planet
    {
        public int size { get; set; }
        public int usability { get; set; }
        public int orbit { get; set; }
        public string name { get; set; }
        
        public event EventHandler progressEvent;

        public Planet()
        {
        }
        
        public string toString()
        {
            return name + " - " + size + " - " + usability+ " - " + orbit;
        }

        public void startEvent()
        {
            progressEvent?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public class System
    {
        public string name { get; set; }
        public List<Planet> planets { get; set; }
    }
    
    public class ClassedSystem : System
    {
        public ClassedSystem Class;
        
        public ClassedSystem(System system)
        {
            name = system.name;
            planets = system.planets;
        }
        
        public string toString()
        {
            return name + " - " + planets;
        }
    }

    public class ClassedPlanet : Planet
    {
        public  ClassedPlanet Class;
        
        public ClassedPlanet(Planet planet)
        {
            size = planet.size;
            usability = planet.usability;
            orbit = planet.orbit;
            name = planet.name;

        }
        
        public string toString()
        {
            return name;
        }
        
        
    }
}