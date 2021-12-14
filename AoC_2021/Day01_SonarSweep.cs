using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2021
{
   class Day01_SonarSweep : AoCodeModule
   {
      public Day01_SonarSweep()
      {
         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput(); 
         OutputFileReset(); 
      }
      public override void DoProcess()
      {

         //** > Result for Day01_SonarSweep part 1:Number of increases: 1688(Process: 0.2405 ms)
         //** > Result for Day01_SonarSweep part 2:Number of 3 measurement window increases: 1728(Process: 0.7089 ms)
         ResetProcessTimer(true);
         long previousReading = long.Parse(inputFile[0]); //init first measurement so that it doesn't register as an "increase" as per puzzle instructions
         long thisReading;
         int increaseCounter = 0;
         foreach (string processingLine in inputFile)
         {
            thisReading = long.Parse(processingLine);
            increaseCounter += (previousReading < thisReading)?1:0;
            previousReading = thisReading;
         }
         AddResult("Number of increases: " + increaseCounter.ToString()); 
         ResetProcessTimer(true);
         increaseCounter = 0;
         int windowIndex = 1; // pointer for set of three, offset to next set of three for first reading.
         int maxWindowIndex = inputFile.Count - 3;
         long window1Sum;
         long window2Sum;
         window1Sum = long.Parse(inputFile[0]) + long.Parse(inputFile[1]) + long.Parse(inputFile[2]);
         while (windowIndex<=maxWindowIndex)
         {
            window2Sum = long.Parse(inputFile[windowIndex]) + long.Parse(inputFile[windowIndex + 1]) + long.Parse(inputFile[windowIndex + 2]);
            increaseCounter += window2Sum > window1Sum ? 1 : 0;
            window1Sum = window2Sum;
            windowIndex++;
         }
         AddResult("Number of 3 measurement window increases: " + increaseCounter.ToString());

      }
   }
}
