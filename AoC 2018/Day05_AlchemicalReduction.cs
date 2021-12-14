using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2018
{
    class Day05_AlchemicalReduction : AoCodeModule
    {
        public Day05_AlchemicalReduction()
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
            //If Comma Delimited on a single input line
            //List<string> inputItems = inputFile[0].Split(new char[] { ',' }).ToList<string>();
            //string finalResult = "Not Set";
            ResetProcessTimer(true);// true also iterates the section marker
            string inputLine = "";
            foreach (string processingLine in inputFile)
            {
                inputLine = processingLine;
                bool done = false;
                int charPointer = 0;
                while (!done)
                {
                    int x = inputLine[charPointer];
                    int y = inputLine[charPointer + 1];
                    if (Math.Abs(x - y) == 32)
                    {
                        inputLine = inputLine.Remove(charPointer, 2);
                        if (charPointer > 0) charPointer--;
                    }
                    else
                    {
                        charPointer++;
                    }
                    done = charPointer == inputLine.Length - 1;
                }
            }
            AddResult(inputLine.Length.ToString()); // includes elapsed time from last ResetProcessTimer
            ResetProcessTimer(true);
            List<PolyResult> AllResults = new List<PolyResult>();
            FunUtilities.HackerScannerPrint hsp = new FunUtilities.HackerScannerPrint("Checking Result for X", 'X');
            hsp.Print("Checking Result for X", true);
            foreach (string processingLine in inputFile)
            {
                for (char alpha = 'A'; alpha <= 'Z'; alpha= (char)(alpha+1))
                {
                    hsp.Print("Checking Result for " + alpha.ToString()); // we set up to "hacker" random will any place with an "X", but.... we just want a straight display, so no padding with X
                    inputLine = processingLine.Replace(alpha.ToString(), "");
                    inputLine = inputLine.Replace(((char)(alpha+32)).ToString(),"");
                    bool done = false;
                    int charPointer = 0;
                    while (!done)
                    {
                        int x = inputLine[charPointer];
                        int y = inputLine[charPointer + 1];
                        if (Math.Abs(x - y) == 32)
                        {
                            inputLine = inputLine.Remove(charPointer, 2);
                            if (charPointer > 0) charPointer--;
                        }
                        else
                        {
                            charPointer++;
                        }
                        done = charPointer == inputLine.Length - 1;
                    }
                    AllResults.Add(new PolyResult(alpha, inputLine.Length));
                }
            }
            Console.WriteLine();
            AddResult(AllResults.OrderBy(x => x.length).First().length.ToString());
        }
    }
    class PolyResult
    {
        public char alpha { get; set; }
        public int length { get; set; }
        public PolyResult(char which, int howLong)
        {
            alpha = which;
            length = howLong;
        }
    }
}
