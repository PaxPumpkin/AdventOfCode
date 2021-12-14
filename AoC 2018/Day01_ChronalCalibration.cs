using System;
using System.Collections.Generic;
using AoC_2018.FunUtilities;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2018
{
    class Day01_ChronalCalibration : AoCodeModule
    {
        public Day01_ChronalCalibration()
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
            ResetProcessTimer(true);
            string finalResult = "Not Set";
            int initialValue = 0;
            foreach (string processingLine in inputFile)
            {
                initialValue += Int32.Parse(processingLine);// no trycatch to let errors bounce if I messed up the input file.
            }
            finalResult = initialValue.ToString();
            //AddResult("Result of the " + this.GetType().Name+ " part 1:" + finalResult );
            AddResult(finalResult);

            ResetProcessTimer(true);
            // not efficient to actually start over and do this all the way twice, but until I get the swing of things again...
            initialValue = 0;
            bool foundDuplicate = false;
            int counter=0; //just for monitoring
            List<int> frequencies = new List<int>();

            FunUtilities.HackerScannerPrint hsp = new HackerScannerPrint("Processing list...round xxx", 'x');
            hsp.Print("Processing list...round " + counter.ToString().PadLeft(3, '0'), true); //not doing the funky "hacker fill-in", otherwise pad with 'x'
            while (!foundDuplicate)
            {
                counter++;
                hsp.Print("Processing list...round " + counter.ToString().PadLeft(3, '0'));
                foreach (string processingLine in inputFile)
                {
                    initialValue += Int32.Parse(processingLine);// no trycatch to let errors bounce
                    if (frequencies.Contains(initialValue))
                    {
                        foundDuplicate = true;
                        Console.WriteLine();
                        break;
                    }
                    else
                    {
                        frequencies.Add(initialValue);
                    }
                }
            }
            finalResult = initialValue.ToString();
            AddResult(finalResult);
        }
    }
}
