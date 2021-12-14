using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyHeadquarters
{
    class AoCodeItem : AoCodeModule
    {
        public AoCodeItem()
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
            
            List<string> inputItems = inputFile[0].Split(new char[] { ',' }).ToList<string>();
            string finalResult = "Not Set";
            foreach (string processingLine in inputItems)
            {

            }
            FinalOutput.Add("Result of the " + this.GetType().ToString() + ":" + finalResult); // output as part of the completion message, add as much as necessary
        }
    }
}
