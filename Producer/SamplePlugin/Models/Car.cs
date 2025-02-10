using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplePlugin.Models
{
    public class Car
    {
        // Properties
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public double EngineSize { get; set; } // In liters

        // Constructor
        public Car(string make, string model, int year, string color, double engineSize)
        {
            Make = make;
            Model = model;
            Year = year;
            Color = color;
            EngineSize = engineSize;
        }

        // Method to display car details
        public void DisplayCarInfo()
        {
            Console.WriteLine($"Car Information: ");
            Console.WriteLine($"Make: {Make}");
            Console.WriteLine($"Model: {Model}");
            Console.WriteLine($"Year: {Year}");
            Console.WriteLine($"Color: {Color}");
            Console.WriteLine($"Engine Size: {EngineSize} liters");
        }

        // Method to simulate starting the car
        public void StartEngine()
        {
            Console.WriteLine($"{Make} {Model}'s engine is now running...");
        }
    }
}
