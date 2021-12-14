using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2019
{
    class Day01_TheTyrannyOfTheRocketEquation : AoCodeModule
    {
        public Day01_TheTyrannyOfTheRocketEquation()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset();
        }
        public override void DoProcess()
        {
            
            string finalResult = "Not Set";
            ResetProcessTimer(true);
            long accumulator = 0; 
            foreach (string processingLine in inputFile)
            {
                accumulator += FuelRequired(Convert.ToDecimal(processingLine));
            }
            finalResult = "Fuel required for all modules: " + accumulator.ToString();
            AddResult(finalResult);
            ResetProcessTimer(true);
            accumulator = 0;
            foreach (string processingLine in inputFile)
            {
                accumulator += FuelRequiredUntilWishing(Convert.ToDecimal(processingLine));
            }
            finalResult = "Fuel required for all modules and their additional fuel: " + accumulator.ToString();
            AddResult(finalResult);

        }
        public int FuelRequired(decimal mass)
        {
            mass /= 3;
            int fuelRequired = ((int)mass) - 2;  //drop decimal porting.
            return (fuelRequired < 0) ? 0:fuelRequired;
        }
        public int FuelRequiredUntilWishing(decimal mass)
        {
            int fuelRequired = FuelRequired(mass);
            int fuelFuel = FuelRequired(fuelRequired);
            fuelRequired += fuelFuel;
            while (fuelFuel > 0)
            {
                fuelFuel = FuelRequired((decimal)fuelFuel);
                fuelRequired += fuelFuel;
            }
            return fuelRequired;
        }
    }
}
