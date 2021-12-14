using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2016
{
    class Day06_SignalsAndNoise : AoCodeModule
    {
        public Day06_SignalsAndNoise()
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
            Signal main = null;
            foreach (string processingLine in inputFile)
            {
                main = new Signal(processingLine);
            }
            AddResult(Signal.GetMessage(true));
            ResetProcessTimer(true);
            AddResult(Signal.GetMessage(false));
        }
    }
    public class Signal
    {
        private static List<char>[] signals = new List<char>[] {new List<char>(), new List<char>(), new List<char>(), new List<char>(), new List<char>(), new List<char>(), new List<char>(), new List<char>() };
        private string signalRecd = "";
        public Signal(string line)
        {
            signalRecd = line;
            int index = 0;
            line.ToCharArray().ToList().ForEach(x => { signals[index].Add(x); index++; });
        }
        public static string GetMessage(bool getMostCommon)
        {
            string output = "";
            signals.ToList().ForEach(x =>
            {
                var thisGroup = x.GroupBy(y => y).Select(y => new { k = y.Key, cnt = y.Count() }).OrderByDescending(y => y.cnt).ToList();
                output += thisGroup[(getMostCommon?0:thisGroup.Count-1)].k.ToString();
            });
            return output;
        }
    }
}
