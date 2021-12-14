using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2016
{
    class Day09_ExplosivesInCyberspace : AoCodeModule
    {
        public Day09_ExplosivesInCyberspace()
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
            string origFile = inputFile[0];
            string outputFile = "";
            bool done = false;
            char inc;
            int rptMarker, charCount, multiplier;
            string dataIndexer,rptSegment;
            string[] indexerParser;
            while (!done)
            {
                inc = origFile[0];
                if (inc != '(')
                {
                    outputFile += inc.ToString();
                    origFile = origFile.Substring(1);
                }
                else
                {
                    rptMarker = origFile.IndexOf(')')+1;
                    dataIndexer = origFile.Substring(0, rptMarker);
                    origFile = origFile.Substring(rptMarker);
                    indexerParser = dataIndexer.Split(new char[] {'(',')','x' }, StringSplitOptions.RemoveEmptyEntries);
                    charCount = int.Parse(indexerParser[0]);
                    multiplier = int.Parse(indexerParser[1]);
                    rptSegment = origFile.Substring(0, charCount);
                    origFile = origFile.Substring(charCount);
                    for (int x = 0; x < multiplier; x++)
                    {
                        outputFile += rptSegment;
                    }
                }
                done = origFile.Length==0;
            }
            AddResult(outputFile.Trim().Length.ToString()); // includes elapsed time from last ResetProcessTimer
            ResetProcessTimer(true);
            AddResult(ParseInput(inputFile[0]).ToString());
        }
        private long ParseInput(string parseable)
        {
            long length = 0;


            bool done = false;
            char inc;
            int rptMarker, charCount;
            long multiplier; // I'll be multiplying against a long, best to use the same data type.
            string dataIndexer, rptSegment;
            string[] indexerParser;
            while (!done)
            {
                inc = parseable[0];
                if (inc != '(')
                {
                    length++;
                    parseable = parseable.Substring(1);
                }
                else
                {
                    rptMarker = parseable.IndexOf(')') + 1;
                    dataIndexer = parseable.Substring(0, rptMarker);
                    parseable = parseable.Substring(rptMarker);
                    indexerParser = dataIndexer.Split(new char[] { '(', ')', 'x' }, StringSplitOptions.RemoveEmptyEntries);
                    charCount = int.Parse(indexerParser[0]);
                    multiplier = long.Parse(indexerParser[1]);
                    rptSegment = parseable.Substring(0, charCount);
                    parseable = parseable.Substring(charCount);
                    long segmentExpandedLength = ParseInput(rptSegment);
                    length += (segmentExpandedLength * multiplier);

                }
                done = parseable.Length == 0;
            }


            return length;
        }
    }
}
