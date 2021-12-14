using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace AoC_2019
{
   class Day11_SpacePolice : AoCodeModule
   {
      public Day11_SpacePolice()
      {
         // If you always save input file in the /InputFiles/ subfolder and name it the same as the class processing it, this will work.
         // if you put it elsewhere or name it differently, just change below. 
         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput(); //base class method
         OutputFileReset(); // output file will be in the same location as input, with ".output.txt" appended to the name. This clears previous output from file.
                            //Print("Did Something");// outputs to console immediately
                            //Print("DidSomethingElse", FileOutputAlso); // both console and output file
                            //Print("Another Thing", FileOutputOnly); // output file only.
      }
      public override void DoProcess()
      {
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
         compy.CompyIsVerbose = false;
         compy.CompyDisplayAllDeliveredOutput = true; 
         compy.SetOutputWithText(false); //just the facts, ma'am
         compy.SetInputAutomaticWait(true);
         compy.AddPreparedProgramInput("0"); 
         compy.RunProgram();

         // Compy will output twice for each input it looks like.
         // need to mod compy to queue output. 
         while(compy.CompyIsRunning)
         {
            while(compy.CompyIsRunning && !compy.CompyHasOutputToRead) // maybe compy.WantsInputYouButthead?
            {
               Thread.Yield();
               if (!compy.CompyIsRunning) break;
            }
            if (compy.CompyHasOutputToRead)
            {

            }
         }
         AddResult("Painted at least once" + compy.GetLastProgramOutput());
         ResetProcessTimer(true);




      }
   }
}
