using System;

namespace Development
{
    class Program
    {
        static void Main(string[] args)
        {
            var canalGate = new CanalLock();

            // O estado deve ser portas fechadas com agua em low
            Console.WriteLine(canalGate);

            canalGate.SetLowGate(open : true);
            Console.WriteLine($"Open the lower gate: {canalGate}");

            Console.WriteLine("Boat enters lock from lower gate");
            
            canalGate.SetLowGate(open: false);
            Console.WriteLine($"Close the lower gate: {canalGate}");

            canalGate.SetWaterLevel(WaterLevel.High);
            Console.WriteLine($"Raise the water level: {canalGate}");
            Console.WriteLine(canalGate);

            canalGate.SetHighGate(open: true);
            Console.WriteLine($"Open the higher gate: {canalGate}");

            Console.WriteLine("Boat exits lock at upper gate");
            Console.WriteLine("Boat enters lock from upper gate");

            canalGate.SetHighGate(open: false);
            Console.Write($"Close the higher gate: {canalGate}");

            canalGate.SetWaterLevel(WaterLevel.Low);
            Console.WriteLine($"Lower the water level: {canalGate}");

            canalGate.SetLowGate(open: true);
            Console.WriteLine($"Open the lower gate: {canalGate}");

            Console.WriteLine("Boat exits lock at upper gate");
            
            canalGate.SetLowGate(open: false);
            Console.WriteLine($"Close the lower gate: {canalGate}");

            Console.WriteLine("========================");
            Console.WriteLine("         Test invalid commands");

            // Open "wrong" gate (2 tests)
            try
            {
                canalGate = new CanalLock();
                canalGate.SetHighGate(open: true);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Invalid operation: Can't open the high gate. Water is low.");
            }
            Console.WriteLine($"Try to open upper gate: {canalGate}");
        }
    }
}
