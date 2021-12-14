using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2019
{
    class Day05_SunnyWithAChanceOfAsteroids : AoCodeModule
    {
        public Day05_SunnyWithAChanceOfAsteroids()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset(); 
        }
        public override void DoProcess()
        {
            /* For each part, program must be run. Could be programmatically set to avoid this requiring keyboard input, but.... whatever.
             * 
             * PART 1 - compy outputs directly to screen vs supplying an answer to put in string. All outputs until the last one should be ZERO to show that the compy is running correctly.
             * PART 1 - value 1 is the air conditioner
             *  Enter a value:
                1
                Explicit value output: 0
                Value in memory address 224 is: 0
                Value in memory address 224 is: 0
                Value in memory address 224 is: 0
                Value in memory address 224 is: 0
                Value in memory address 224 is: 0
                Value in memory address 224 is: 0
                Value in memory address 224 is: 0
                Value in memory address 224 is: 0
                Value in memory address 223 is: 9431221
                ** > Result for Day05_SunnyWithAChanceOfAsteroids part 1:Value in memory address 223 is: 9431221(Process: 707 ms)
             */

            /*PART 2 - direct output to screen. Only output is the diagnostic code (answer)
             * PART 2 - value 5 is thermal radiator
             *  Enter a value:
                5
                Value in memory address 223 is: 1409363
                ** > Result for Day05_SunnyWithAChanceOfAsteroids part 1:Value in memory address 223 is: 1409363(Process: 800 ms)
             * 
             */
            ResetProcessTimer(true);
            List<long> intCodeProgram = new List<long>();
            foreach (string processingLine in inputFile) // should just be 1
            {
                string[] programNumbers = processingLine.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string pn in programNumbers)
                {
                    intCodeProgram.Add(Convert.ToInt64(pn));
                }
            }

            IntCodeComputer compy = new IntCodeComputer(intCodeProgram);
            compy.RunProgram();
            AddResult(" " + compy.GetLastProgramOutput()); 
        }
    }
}
