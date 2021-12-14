using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2019
{
   class Day09_SensorBoost : AoCodeModule
   {
      bool usingTestInput = false;
      public Day09_SensorBoost()
      {
         // sample inputs to test with. Should set usingTestInput = true in order to see them
         // 109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99 takes no input and produces a copy of itself as output.
         // 1102,34915192,34915192,7,4,7,99,0 should output a 16 - digit number.
         // 104,1125899906842624,99 should output the large number in the middle.
         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput();
         OutputFileReset();
      }
      public override void DoProcess()
      {
         //** > Result for Day09_SensorBoost part 1: BOOST Diagnostic Code 2955820355 (Process: 0.987 ms)
         //** > Result for Day09_SensorBoost part 2: Coordinates of Ceres Distress Signal 46643 (Process: 680.3736 ms)
         ResetProcessTimer(true);
         List<long> intCodeProgram = new List<long>();
         foreach (string processingLine in inputFile)
         {
            string[] programNumbers = processingLine.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string pn in programNumbers)
            {
               intCodeProgram.Add(Convert.ToInt64(pn));
            }
         }
         IntCodeComputer compy = new IntCodeComputer(intCodeProgram);

         compy.ResetProgram();
         compy.CompyIsVerbose = usingTestInput;
         compy.CompyDisplayAllDeliveredOutput = usingTestInput; // useful for test programs on Day 9 set to true, for echoing out itself versus just the "Last" output.
         compy.SetOutputWithText(usingTestInput); //just the facts, ma'am
         if (usingTestInput)
         {
            compy.RunProgram();
            AddResult("Done Testing.");
            ResetProcessTimer(true);
         }
         else
         {
            compy.AddPreparedProgramInput("1"); //1 is "test mode"
            compy.RunProgram();
            AddResult("BOOST Diagnostic Code " + compy.GetLastProgramOutput());
            ResetProcessTimer(true);
            compy.ClearProgram();
            compy.LoadProgram(intCodeProgram);
            compy.ResetProgram();
            compy.AddPreparedProgramInput("2"); //2 is "sensor boost mode"
            compy.RunProgram();
            AddResult("Coordinates of Ceres Distress Signal " + compy.GetLastProgramOutput());
            ResetProcessTimer(true);
         }
      }
   }
}
