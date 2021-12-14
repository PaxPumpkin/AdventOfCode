using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2018
{
    class Day08_MemoryManeuver : AoCodeModule
    {
        public Day08_MemoryManeuver()
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
            }
            List<int> processing = inputLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList().Select(x => int.Parse(x)).ToList();
            licenseNode rootNode= licenseNode.ProcessNodes(processing).Item2;// the processing string should now be empty.

            AddResult(rootNode.SumMetaData().ToString()); // includes elapsed time from last ResetProcessTimer
            ResetProcessTimer(true);
            AddResult(rootNode.GetValue().ToString());
        }
    }
    public class licenseNode
    {
        public List<licenseNode> childNodes = new List<licenseNode>();
        public List<int> metaData = new List<int>();
        public static Tuple<List<int>,licenseNode> ProcessNodes(List<int> processing)
        {
            int nodeChildCount = processing[0];
            int nodeMetaDataCount = processing[1];
            processing.RemoveRange(0, 2);
            licenseNode nextNode = new licenseNode();
            //process now contains all data for all child nodes.
            for (int i = 0; i < nodeChildCount; i++)
            {
                Tuple<List<int>, licenseNode> result = ProcessNodes(processing);
                processing = result.Item1;
                nextNode.childNodes.Add(result.Item2);
            }
            processing.GetRange(0, nodeMetaDataCount).ForEach(x => nextNode.metaData.Add(x));
            processing.RemoveRange(0, nodeMetaDataCount);
            return new Tuple<List<int>, licenseNode>(processing, nextNode);
        }
        public int SumMetaData()
        {
            int mySum = this.metaData.Sum();
            childNodes.ForEach(x => mySum += x.SumMetaData());
            return mySum;
        }
        public int GetValue()
        {
            int myValue = 0;
            if (this.childNodes.Count == 0)
            {
                myValue = this.metaData.Sum();
            }
            else
            {
                metaData.ForEach(x =>
                {
                    if (x != 0 && x <= this.childNodes.Count)
                    {
                        myValue += childNodes[x - 1].GetValue();
                    }
                });
            }
            return myValue;
        }
    }
}
